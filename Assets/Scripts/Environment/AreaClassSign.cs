using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Специальный класс, который дублирует все поля AreaClass, но не является Monobehaviour
/// </summary>
public class AreaClassSign 
{

    #region fields

    public AreaID id = new AreaID();

    public List<DoorPointClass> doorPoints = new List<DoorPointClass>();//Как связаны двери и проходы в комнате между собой. Этот массив используется для создания маршрутов в башне.

    #endregion //fields

    /// <summary>
    /// Копирующий конструктор
    /// </summary>
    public AreaClassSign(AreaClass _area)
    {
        id = _area.id;
        doorPoints = new List<DoorPointClass>();
        foreach (DoorPointClass doorPoint in _area.doorPoints)
        {
            doorPoints.Add(new DoorPointClass(doorPoint.doorPoint, doorPoint.connections, doorPoint.connectionNumbs));
        }
        foreach (DoorPointClass doorPoint in doorPoints)
        {
            for (int i = 0; i < doorPoint.connectionNumbs.Count; i++)
            {
                doorPoint.connections[i].nextPoint = doorPoints[doorPoint.connectionNumbs[i]];
            }
        }

        //Проделаем некоторые изменения над копией пространства
        DoorPointClass _doorPoint, nextPoint;
        List<DoorPointClass> checkedPoints = new List<DoorPointClass>();
        foreach (DoorPointClass doorPoint in _area.doorPoints)
        {
            _doorPoint = GetDoorPoint(doorPoint.doorPoint);
            checkedPoints.Add(_doorPoint);
            foreach (DoorConnectionClass doorConnection in doorPoint.connections)
            {
                nextPoint = GetDoorPoint(doorConnection.nextPoint.doorPoint);
                if (!checkedPoints.Contains(nextPoint))
                {
                    nextPoint.connections.Add(new DoorConnectionClass(_doorPoint, doorConnection.connectionType, doorConnection.connectionObject));
                }
            }
        }//Таким образом мы создали двухсторонние связи в копии. Поэтому можно не создавать одни и те же DoorConnection дважды, и при этом Map будет работать нормально.
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
}
