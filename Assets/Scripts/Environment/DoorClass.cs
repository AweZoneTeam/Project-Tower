using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Двери - тип объектов, которые связывают комнаты. Взаимодействуя с дверью, персонаж переносится в заданное место в другой комнате.
/// </summary>
[System.Serializable]
public class DoorClass: MonoBehaviour
{

    #region consts

    public const float changeRoomOffsetX = 5f;//Добавка к изменению расстояния при переходе через дверь.
    public const float changeRoomOffsetY = -6f;

    #endregion //consts

    #region eventHandlers

    public EventHandler<JournalEventArgs> DoorOpenJournalEvent;

    #endregion //eventHandlers

    #region fields

    public doorEnum doorType;//Тип двери
    public AreaClass roomPath;//Куда ведёт данная дверь
    public DoorClass pairDoor;//Дверь с противоположной стороны стены... По сути та же самая дверь
    public LockScript locker;//Какой замок стоит на двери

    #endregion //fields

    public DoorClass(doorEnum _type, AreaClass _path, LockScript _locker)
    {
        doorType = _type; roomPath = _path; locker = _locker;
    }

    #region events

    public void OnDoorOpen(JournalEventArgs e)
    {
        EventHandler<JournalEventArgs> handler = DoorOpenJournalEvent;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    #endregion //events

}