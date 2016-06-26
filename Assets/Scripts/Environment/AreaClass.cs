using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, представляющий собой пространство - особая область, которая знает, что находится внутри неё и за пределами которой камера выйти не может.
/// Такие области имеют определённый размер (3D коллайдер) и все объекты комнаты находятся внутри него.
/// Также пространства могут иметь подпространства и соседние пространства.
/// </summary>
public class AreaClass : MonoBehaviour
{

    #region consts

    protected const float defDayIntensity = 1f;
    protected const float defNightIntensity = .2f;

    protected const float zCoordinateOffset = .02f;//Насколько отличаются 2 соседние z-координаты 
    protected const int maxCount = 100;//Сколько всего содержится эксклюзивных координат в комнате
    protected const float zEps = .01f;

    #endregion //consts

    #region fields

    public AreaID id=new AreaID();
    public List<RoomConnection> subAreas = new List<RoomConnection>();//какие пространства являются подчинёнными
    public List<RoomConnection> neigbAreas = new List<RoomConnection>();//какие пространства соседствуют с этим

    public List<DoorPointClass> doorPoints = new List<DoorPointClass>();//Как связаны двери и проходы в комнате между собой. Этот массив используется для создания маршрутов в башне.

    public List<InterObjController> container=new List<InterObjController>();//Что содержит в себе это пространство. Содержание подпространств не учитывается.
    public List<DropClass> drops = new List<DropClass>();//Какие дропающиеся предметы лежат в этой комнате?
    public List<GameObject> lightSources = new List<GameObject>();//Источники освещения в данном пространстве

    [SerializeField]protected float dayAmbIntensity=defDayIntensity, nightAmbIntensity=defNightIntensity;//Интенсивность окружающего (рассеянного) света в помещении днём и ночью
    public float DayAmbIntensity {get { return dayAmbIntensity; }}
    public float NightAmbIntensity {get { return nightAmbIntensity; }}
     
    public List<GameObject> hideForeground = new List<GameObject>();//Какие объекты исчезают из поля зрения камеры, при входе в область
    public Vector3 position;//координаты центра пространства (пространство по форме обязательно представляет собой параллелепипед)
    public Vector3 size;//Каковы размеры пространства: длина, глубина и высота

    public List<float> zCoordinates = new List<float>();//Какие z-координаты занимаются персонажами. (1 персонаж в комнате - 1 соответствующая ему z-координата )

    protected int maxRegistrationNumber; //Сколько объектов уже зарегестрировалось в этой комнате
    public int MaxRegistrationNumber {get { maxRegistrationNumber++; return maxRegistrationNumber; }set { maxRegistrationNumber = value; } }

    #endregion //fields

    /// <summary>
    /// Копирующий конструктор
    /// </summary>
    public AreaClass(AreaClass _area)
    {
        id = _area.id;
        subAreas = _area.subAreas;
        neigbAreas = _area.neigbAreas;
        doorPoints = new List<DoorPointClass>();
        foreach (DoorPointClass doorPoint in _area.doorPoints)
        {
            doorPoints.Add(new DoorPointClass(doorPoint.doorPoint, doorPoint.connections, doorPoint.connectionNumbs));
        }
        foreach (DoorPointClass doorPoint in doorPoints)
        {
            for(int i=0;i<doorPoint.connectionNumbs.Count;i++)
            {
                doorPoint.connections[i].nextPoint = doorPoints[doorPoint.connectionNumbs[i]];
            }
        }

        //Проделаем некоторые изменения над копией пространства
        DoorPointClass _doorPoint;
        foreach (DoorPointClass doorPoint in _area.doorPoints)
        {
            _doorPoint = GetDoorPoint(doorPoint.doorPoint);
            foreach (DoorConnectionClass doorConnection in doorPoint.connections)
            {
                GetDoorPoint(doorConnection.nextPoint.doorPoint).connections.Add(new DoorConnectionClass(_doorPoint, doorConnection.connectionType, doorConnection.connectionObject));
            }
        }//Таким образом мы создали двухсторонние связи в копии. Поэтому можно не создавать одни и те же DoorConnection дважды, и при этом Map будет работать нормально.
    }

    /// <summary>
    /// Возвращает дверь в указанную комнату, если она является соседней
    /// </summary>
    public GameObject GetDoor(AreaClass room)
    {
        for (int i = 0; i < subAreas.Count; i++)
        {
            if (subAreas[i].room == room)
            {
                return subAreas[i].door;

            }
        }
        for (int i = 0; i < neigbAreas.Count; i++)
        {
            if (neigbAreas[i].room == room)
            {
                return neigbAreas[i].door;
            }
        }
        return null;
    }

    /// <summary>
    /// Определить ближайшую к заданной точке дверь в этой комнате
    /// </summary>
    public GameObject GetNearestDoor(Vector3 pos)
    {
        float minDistance = 100000f;
        GameObject targetObj = null;
        foreach (RoomConnection roomConnection in subAreas)
        {
            if (Vector3.Distance(roomConnection.door.transform.position, pos) < minDistance)
            {
                minDistance = Vector3.Distance(roomConnection.door.transform.position, pos);
                targetObj = roomConnection.door;
            }
        }
        foreach (RoomConnection roomConnection in neigbAreas)
        {
            if (Vector3.Distance(roomConnection.door.transform.position, pos) < minDistance)
            {
                minDistance = Vector3.Distance(roomConnection.door.transform.position, pos);
                targetObj = roomConnection.door;
            }
        }
        return targetObj;
    }

    /// <summary>
    /// Узнать по объекту в комнате, с какими другими объектами он связан
    /// </summary>
    public DoorPointClass GetDoorPoint(GameObject _door)
    {
        foreach (DoorPointClass doorPoint in doorPoints)
        {
            if (doorPoint.doorPoint == _door)
            {
                return doorPoint;
            }
        }
        return null;
    }

    /// <summary>
    /// Получить доступную z-координату
    /// </summary>
    public float GetZCoordinate()
    {
        if (zCoordinates.Count > 100)
        {
            return 0f;
        }
        if (zCoordinates.Count == 0)
        {
            zCoordinates.Add(0f);
            return 0f;
        }
        else
        {
            if (zCoordinates[0] != 0f)
            {
                zCoordinates.Insert(0, 0f);
                return 0f;
            }
            else if (zCoordinates.Count == 1)
            {
                zCoordinates.Add(zCoordinateOffset);
                return zCoordinateOffset;
            }
            else
            {
                for (int i = 0; i < zCoordinates.Count - 1; i++)
                {
                    if (zCoordinates[i + 1] - zCoordinates[i] > zCoordinateOffset+zEps)
                    {
                        zCoordinates.Insert(i+1, zCoordinates[i] + zCoordinateOffset);
                        return zCoordinates[i + 1];
                    }
                }
                zCoordinates.Add(zCoordinates[zCoordinates.Count - 1] + zCoordinateOffset);
                return zCoordinates[zCoordinates.Count - 1];
            }
        }
        return 0f;
    }

    /// <summary>
    /// Освободить z-координату при уходе персонажа
    /// </summary>
    public void RemoveZCoordinate(float z1)
    {
        if (zCoordinates.Contains(z1))
        {
            zCoordinates.Remove(z1);
        }
    }

    /// <summary>
    /// Зарегестрировать все объекты, что находятся в контейнере
    /// </summary>
    public virtual void RegisterContainer()
    {
        maxRegistrationNumber = -1;
        foreach (InterObjController obj in container)
        {
            obj.RegisterObject(this, false);
        }
    }

    public int GetMaxRegistrationNumber()
    {
        return maxRegistrationNumber;
    }

}

/// <summary>
/// Набор параметров, идентифицирующих пространство, в котором персонаж находится
/// </summary>
[System.Serializable]
public class AreaID
{
    public string areaName; // имя пространства (если у него нет родителей), или имя родительского пространства (например, в случае комнат)
    public int floor, room;//Этаж и номер комнаты или пространства
    public string plane;//план, на котором расположено пространство.
    public float coordZ; //z-координата, на которой должен располагаться персонаж, если он перешёл в данное пространство.

    /// <summary>
    /// Дефолтный конструктор
    /// </summary>
    public AreaID()
    {
        areaName = "defName";
        floor = 0;
        room = 1;
        plane = "A";
        coordZ = 0f;
    }
}