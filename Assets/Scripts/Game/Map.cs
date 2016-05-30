using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, в котором собрана вся информация о комнатах, их содержимом, и как эти комнаты между собой связанны.
/// Также этот класс ответственене за нахождение кратчайшего пути из любой комнаты в любую.
/// </summary>
public class Map : MonoBehaviour
{

    #region consts

    protected const int maxMapValue = 100000;

    #endregion //consts

    #region dictionaries

    //то, как оценивают пути соединения между комнатами различные виды искусственного интеллекта. Например, летающие противники не видят проблемы в перелёте воздушных препятствий, 
    //в то время как ходящим персонажам это кажется непреодолимым
    protected Dictionary<Type, Dictionary<doorConnectionEnum, int>> connectionValues = new Dictionary<Type, Dictionary<doorConnectionEnum, int>> { { typeof(AIController), new Dictionary<doorConnectionEnum, int>{ {doorConnectionEnum.air,1000},
                                                                                                                                                                                                                    {doorConnectionEnum.ground, 3 },
                                                                                                                                                                                                                    {doorConnectionEnum.ladder, 5 },
                                                                                                                                                                                                                    {doorConnectionEnum.stairs, 4 },
                                                                                                                                                                                                                    {doorConnectionEnum.zero, 1 },
                                                                                                                                                                                                                    {doorConnectionEnum.obstacle,1001} } },
                                                                                                                                                    { typeof(SpiderController),new Dictionary<doorConnectionEnum, int> { {doorConnectionEnum.air,6},
                                                                                                                                                                                                                    {doorConnectionEnum.ground, 3 },
                                                                                                                                                                                                                    {doorConnectionEnum.ladder, 6 },
                                                                                                                                                                                                                    {doorConnectionEnum.stairs, 4 },
                                                                                                                                                                                                                    {doorConnectionEnum.zero, 1 },
                                                                                                                                                                                                                    {doorConnectionEnum.obstacle,1001}} } };

    #endregion //dictionaries

    #region fields

    public List<AreaClass> rooms=new List<AreaClass>();//Какие комнаты составляют карту уровня

    public List<MapPointClass> mapPoints=new List<MapPointClass>();//Точки, через которые карта прокладывает маршруты.

    #endregion //fields

    public void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Инициализация карты
    /// </summary>
    public void Initialize()
    {
        FormMapPoints();
    }

    /// <summary>
    /// Сформировать граф карты уровня
    /// </summary>
    public void FormMapPoints()
    {
        List<GameObject> doors=new List<GameObject>();
        List<AreaClassSign> _rooms = new List<AreaClassSign>();
        Dictionary<AreaID, AreaClassSign> roomsDict = new Dictionary<AreaID, AreaClassSign>();//Особый список комнат, нужный для создания мэппоинтов
        foreach (AreaClass room in rooms)
        {
            AreaClassSign _room = new AreaClassSign(room);
            _rooms.Add(_room);
            roomsDict.Add(_room.id, _room);
        }
        foreach (AreaClass room in rooms)
        {
            if (roomsDict.ContainsKey(room.id))
            {
                AreaClassSign _room = roomsDict[room.id];
                foreach (DoorPointClass doorPoint in _room.doorPoints)
                {
                    if (!doors.Contains(doorPoint.doorPoint))
                    {
                        MapPointClass mapPoint = new MapPointClass();
                        GameObject door = doorPoint.doorPoint;
                        GameObject pairDoor = null;
                        if (door.GetComponent<DoorClass>() != null)
                        {
                            DoorClass doorClass = door.GetComponent<DoorClass>();
                            pairDoor = doorClass.roomPath.GetDoor(room);
                            doorClass.roomPath.GetDoorPoint(pairDoor).mapPoint = mapPoint;
                            mapPoint.doorPoint2 = roomsDict[doorClass.roomPath.id].GetDoorPoint(pairDoor);
                            mapPoint.doorPoint2.mapPoint = mapPoint;
                        }
                        else if (door.GetComponent<EnterClass>() != null)
                        {
                            EnterClass enterClass = door.GetComponent<EnterClass>();
                            pairDoor = enterClass.nextRoom.GetDoor(room);
                            enterClass.nextRoom.GetDoorPoint(pairDoor).mapPoint = mapPoint;
                            mapPoint.doorPoint2 = roomsDict[enterClass.nextRoom.id].GetDoorPoint(pairDoor);
                            mapPoint.doorPoint2.mapPoint = mapPoint;
                        }
                        doors.Add(door);
                        mapPoint.doorPoint1 = doorPoint;
                        mapPoint.doorPoint1.mapPoint = mapPoint;
                        room.GetDoorPoint(door).mapPoint = mapPoint;
                        if (pairDoor != null)
                        {
                            doors.Add(pairDoor);
                        }
                        mapPoints.Add(mapPoint);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Функция, прокладывающая маршрут из вэйпоинтов
    /// </summary>
    /// <param name="местоположение персонажа, на момент запроса"></param>
    /// <param name="начальная комната"></param>
    /// <param name="конечная цель, которую хочет достичь персонаж"></param>
    /// <param name="конечная комната"></param>
    public List<TargetWithCondition> GetWay(PersonController person, AreaClass currentArea, TargetWithCondition target, AreaClass targetArea)
    {
        //Второй прототип этой функции, основанный на алгоритме Дийкстры

        List<TargetWithCondition> waypoints= new List<TargetWithCondition>();

        //Сначала надо найти начальные и конечные мэппоинты.
       MapPointClass beginPoint = currentArea.GetDoorPoint(currentArea.GetNearestDoor(person.transform.position)).mapPoint;
        GameObject targetObj = null;
        if (target.target != null)
        {
            targetObj = target.target;
        }
        else
        {
            targetObj = new GameObject("WayPoint");
            targetObj.transform.position = target.position;
            target.target = targetObj;
        }
        MapPointClass endPoint = targetArea.GetDoorPoint(targetArea.GetNearestDoor(targetObj.transform.position)).mapPoint;

        //Затем проходит обсчёт всех мэйпоинтов, для нахождения оптимального маршрута среди них
        PrepareMapPointsByDiykstra(person, beginPoint, endPoint);

        //Расчитав мэппоинты, можно составить сам маршрут из вэйпоинтов
        beginPoint = new MapPointClass(new DoorConnectionClass(currentArea.GetDoorPoint(currentArea.GetNearestDoor(person.transform.position)),doorConnectionEnum.zero,null), beginPoint, true);
        endPoint = new MapPointClass(null, endPoint, false);
        endPoint.prevPoint.connection = new DoorConnectionClass(new DoorPointClass(target.target, null, null), doorConnectionEnum.zero, null);
        endPoint.prevPoint.connection.nextPoint.mapPoint = endPoint;
        MapPointClass currentPoint = beginPoint;
        while ((currentPoint != endPoint)&&(currentPoint!=null))
        {
            doorConnectionEnum doorConnectionType = currentPoint.connection.connectionType;
            if ((doorConnectionType == doorConnectionEnum.ladder) ||
                (doorConnectionType == doorConnectionEnum.stairs))
            {
                waypoints.Add(new TargetWithCondition(currentPoint.connection.connectionObject, DoorConnectionToString(doorConnectionType)));
            }
            DoorPointClass nextDoorPoint = currentPoint.connection.nextPoint;
            if (nextDoorPoint.mapPoint != endPoint)
            {
                if (nextDoorPoint.mapPoint.connection.nextPoint.mapPoint != endPoint)
                {
                    if (!nextDoorPoint.connections.Contains(nextDoorPoint.mapPoint.connection))
                    {
                        DoorClass door = null;
                        EnterClass enter = null;
                        if ((door = nextDoorPoint.doorPoint.GetComponent<DoorClass>()) != null)
                        {
                            waypoints.Add(new TargetWithCondition(door.gameObject, "door"));
                        }
                        else if ((enter = nextDoorPoint.doorPoint.GetComponent<EnterClass>()) != null)
                        {
                            waypoints.Add(new TargetWithCondition(enter.gameObject, "enter"));
                        }
                    }
                    else
                    {
                        waypoints.Add(new TargetWithCondition(nextDoorPoint.doorPoint, "waypoint", 1));
                    }
                }
                else
                {
                    DoorClass door = null;
                    EnterClass enter = null;
                    if ((door = nextDoorPoint.doorPoint.GetComponent<DoorClass>()) != null)
                    {
                        if (door.roomPath == target.areaPosition)
                        {
                            waypoints.Add(new TargetWithCondition(door.gameObject, "door"));
                        }
                        else
                        {
                            waypoints.Add(new TargetWithCondition(door.gameObject, "waypoint", 1));
                        }
                    }
                    else if ((enter = nextDoorPoint.doorPoint.GetComponent<EnterClass>()) != null)
                    {
                        if (enter.nextRoom == target.areaPosition)
                        {
                            waypoints.Add(new TargetWithCondition(enter.gameObject, "enter"));
                        }
                        else
                        {
                            waypoints.Add(new TargetWithCondition(enter.gameObject, "waypoint", 1));
                        }
                    }
                }
            }
            currentPoint = nextDoorPoint.mapPoint;
        }
        waypoints.Add(target);

        return waypoints;
    }

    /// <summary>
    /// Подготовить граф из мэппоинтов для расчёта оптимального пути, используя алгоритм Дийкстры
    /// </summary>
    protected void PrepareMapPointsByDiykstra(PersonController person, MapPointClass beginPoint, MapPointClass endPoint)
    {
        List<MapPointClass> diykstraPoints=new List<MapPointClass>();
        foreach (MapPointClass mapPoint in mapPoints)
        {
            diykstraPoints.Add(mapPoint);
            mapPoint.value = maxMapValue;
            mapPoint.passed = false;
            mapPoint.connection = null;
            mapPoint.prevPoint = null;
            foreach (DoorConnectionClass doorConnection in mapPoint.doorPoint1.connections)
            {
                doorConnection.connectionValue = connectionValues[person.GetType()][doorConnection.connectionType];
            }
            foreach (DoorConnectionClass doorConnection in mapPoint.doorPoint2.connections)
            {
                doorConnection.connectionValue = connectionValues[person.GetType()][doorConnection.connectionType];
            }
        }
        beginPoint.value = 0;
        MapPointClass currentPoint = null;
        int minValue = maxMapValue+1;
        while (diykstraPoints.Count > 0)
        {
            foreach (MapPointClass mapPoint in diykstraPoints)
            {
                if (mapPoint.value < minValue)
                {
                    minValue = mapPoint.value;
                    currentPoint = mapPoint;
                }
            }
            currentPoint.passed = true;
            MapPointClass nextPoint;
            foreach (DoorConnectionClass doorConnection in currentPoint.doorPoint1.connections)
            {
                nextPoint = doorConnection.nextPoint.mapPoint;
                if ((nextPoint.value > currentPoint.value + doorConnection.connectionValue)&&(!nextPoint.passed))
                {
                    nextPoint.value = currentPoint.value + doorConnection.connectionValue;
                    nextPoint.connection = doorConnection;
                    nextPoint.prevPoint = currentPoint;
                }
            }
            foreach (DoorConnectionClass doorConnection in currentPoint.doorPoint2.connections)
            {
                nextPoint = doorConnection.nextPoint.mapPoint;
                if ((nextPoint.value > currentPoint.value + doorConnection.connectionValue) && (!nextPoint.passed))
                {
                    nextPoint.value = currentPoint.value + doorConnection.connectionValue;
                    nextPoint.connection = doorConnection;
                    nextPoint.prevPoint = currentPoint;
                }
            }
            diykstraPoints.Remove(currentPoint);
            minValue = maxMapValue + 1;
        }
        currentPoint = endPoint;
        DoorConnectionClass nextConnection = currentPoint.connection;
        endPoint.connection = null;
        DoorConnectionClass prevConnection = null;
        while (currentPoint != beginPoint)
        {
            prevConnection = currentPoint.prevPoint.connection;
            currentPoint.prevPoint.connection = nextConnection;
            nextConnection = prevConnection;
            currentPoint = currentPoint.prevPoint;
        }

        

    }

    public string DoorConnectionToString(doorConnectionEnum _doorConnection)
    {
        switch (_doorConnection)
        {
            case doorConnectionEnum.air:
                {
                    return "air";
                }
            case doorConnectionEnum.ground:
                {
                    return "ground";
                }
            case doorConnectionEnum.ladder:
                {
                    return "ladder";
                }
            case doorConnectionEnum.stairs:
                {
                    return "stairs";
                }
            case doorConnectionEnum.zero:
                {
                    return "zero";
                }
            case doorConnectionEnum.obstacle:
                {
                    return "obstacle";
                }
            default:
                {
                    return "";
                }
        }
        
    }

    public AreaClass GetArea(string areaName)
    {
        foreach (AreaClass room in rooms)
        {
            if (string.Equals(room.id.areaName, areaName))
            {
                return room;
            }
        }
        return null;
    }

}

/// <summary>
/// Класс, представляющий собой данные о том, как связаны между собой экземпляры DoorPointClass
/// </summary>
[System.Serializable]
public class DoorConnectionClass
{
    public DoorPointClass nextPoint;
    public doorConnectionEnum connectionType;//Тип связи, проложенной между комнатами
    public GameObject connectionObject;//Посредством какого объекта осуществляется связь

    public int connectionValue; //Какой вес имеет это ребро при расчёте алгоритмом Дийкстры

    public DoorConnectionClass(DoorPointClass _nextPoint, doorConnectionEnum _connectionType, GameObject _connectionObject)
    {
        nextPoint = _nextPoint;
        connectionType = _connectionType;
        connectionObject = _connectionObject;
    }

}

/// <summary>
/// Класс, что представляет собой значимую для карты точку - дверь, имеющая некоторые пути сообщения с другими верями
/// </summary>
[System.Serializable]
public class DoorPointClass
{
    public GameObject doorPoint; //Это будет либо проход, либо дверь, что связывает между собой пространства игры 
    public List<DoorConnectionClass> connections=new List<DoorConnectionClass>();//Связи этой точки с другими точками.
    public List<int> connectionNumbs = new List<int>();//Так как в инспекторе нельзя указать дверную точку, то мы указываем номер дверной точки в массиве в экземпляре AreaClass

    public MapPointClass mapPoint = null;//К какой точке графа карты уровня относится этот экземпляр DoorPointClass

    public DoorPointClass(GameObject _doorPoint, List<DoorConnectionClass> _connections, List<int> _numbs)
    {
        doorPoint = _doorPoint;
        connections = _connections;
        connectionNumbs = _numbs;
    }

    public bool HasConnection(DoorPointClass _doorPoint)
    {
        foreach (DoorConnectionClass doorConnection in connections)
        {
            if (doorConnection.nextPoint == _doorPoint)
            {
                return true;
            }
        }
        return false;
    }
}

/// <summary>
/// Класс, представляющий собой точки графа, который и будет картой уровня в представлении ИИ
/// </summary>
public class MapPointClass
{
    public DoorPointClass doorPoint1, doorPoint2;//Так как двери нередко (всегда) бывают парными и каждая из них имеет связи только с дверями той же комнаты, то нужен класс, который объедини парные двери и по сути свяжет комнаты друг  другом. 
    public int value;//Какое значение принял этот узел при расчёте алгоритма Дийкстры?
    public bool passed = false;//Была ли пройдена эта вершина при обходе алгоритмом Дийкстры?
    public DoorConnectionClass connection;//Какая связь наиболее выгодна, чтобы добраться до этой точки?
    public MapPointClass prevPoint;//С какой точкой связываем эту

    /// <summary>
    /// Определить обратный путь по заданному пути в этот мэппоинт
    /// </summary>
    public DoorConnectionClass GetReverseConnection(DoorConnectionClass _doorConnection)
    {
        return null;
    }

    /// <summary>
    /// Пустой дефолтный конструктор
    /// </summary>
    public MapPointClass()
    { }

    public MapPointClass(DoorConnectionClass _connection, MapPointClass point, bool next)
    {
        if (next)
        {
            connection = _connection;
            connection.nextPoint.mapPoint = point;
        }
        else
        {
            prevPoint = point;
        }
    }

}