using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, в котором собрана вся информация о комнатах, их содержимом, и как эти комнаты между собой связанны.
/// Также этот класс ответственене за нахождение кратчайшего пути из любой комнаты в любую.
/// </summary>
public class Map : MonoBehaviour
{

    /// <summary>
    /// Функция, прокладывающая маршрут из вэйпоинтов
    /// </summary>
    /// <param name="местоположение персонажа, на момент запроса"></param>
    /// <param name="начальная комната"></param>
    /// <param name="конечная цель, которую хочет достичь персонаж"></param>
    /// <param name="конечная комната"></param>
    public List<TargetWithCondition> GetWay(Vector3 currentPos, AreaClass currentArea, TargetWithCondition target, AreaClass targetArea)
    {
        //Первый простейший прототип этой функции
        List<TargetWithCondition> waypoints = new List<TargetWithCondition>();
        if (target != null)
        {
            GameObject roomConnection;
            if ((roomConnection = currentArea.GetDoor(targetArea)) != null)
            {
                if (roomConnection.GetComponent<DoorClass>() != null)
                {
                    waypoints.Add(new TargetWithCondition(roomConnection, "door"));
                }
                else if (roomConnection.GetComponent<EnterClass>() != null)
                {
                    waypoints.Add(new TargetWithCondition(roomConnection, "enter"));
                }
                waypoints.Add(target);
            }
            else if (currentArea == targetArea)
            {
                waypoints.Add(target);
            }
            return waypoints;
        }
        else
        {
            return null;
        }
    }

}
