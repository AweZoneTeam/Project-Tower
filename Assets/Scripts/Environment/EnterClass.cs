using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Класс, представляющий собой проход из одной комнаты в другую - если персонаж коснётся этого объекта, он перейдёт в соседнюю комнату
//о/ </summary>
public class EnterClass : MonoBehaviour
{

    #region eventHandlers

    public EventHandler<JournalEventArgs> EnterUseJournalEvent;

    #endregion //eventHandlers

    #region fields

    public doorEnum enterType;//Тип прохода
    public AreaClass nextRoom;// Комната, в которую ведёт проход
    public AreaClass subRoom;//Подпространство, в которое ведёт проход.
    public EnterClass pairEnter;//Парный проход из комнаты, в которую ведёт данный проход

    #endregion //fields

    #region events

    /// <summary>
    /// Событие, что вызывается при использовании прохода
    /// </summary>
    public void OnEnterUse(JournalEventArgs e)
    {
        EventHandler<JournalEventArgs> handler = EnterUseJournalEvent;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    #endregion //events
}

