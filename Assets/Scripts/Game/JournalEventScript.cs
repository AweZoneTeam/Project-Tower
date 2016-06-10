using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, который объединяет все журнальные события по условию, из-за которого все они должны произойти 
/// </summary>
[System.Serializable]
public class JournalEventScript : ScriptableObject
{

    #region fields

    private JournalScriptStock _list;//Ссылка на список, в котором это журнальное событие находится
    public JournalScriptStock jList
    {
        get { return _list; }
        set { _list = value; }
    }

    public string jScriptName;
    public List<JournalEventAction> jActions = new List<JournalEventAction>();//Журнальные действия
    public string jDataConditionName;//Имя журнального условия
    public string id;    //Параметры, что будет использовать 
    public int argument;//функция журнального условия

    public List<JournalEventScript> consequences = new List<JournalEventScript>();//К каким последствиям приведёт данный журнальный скрипт 
                                                                                  //(как будет развиваться история, какие новые журнальные скрипты будут использованы в дальнейшем)
    #endregion //fields

    public void HandleJournalEvent(object sender, JournalEventArgs e)
    {
        foreach (JournalEventAction _action in jActions)
        {
            _action.jDataAction.Invoke(_action);
        }
        _list.RemoveJournalScript(this);
        foreach (JournalEventScript _script in consequences)
        {
            _list.AddJournalScript(_script);
        }
    }

}

/// <summary>
/// Класс, который представляет собой структурную единицу обработчика журнальных событий и который соответствует одной журнальной записи.
/// </summary>
[System.Serializable]
public class JournalEventAction
{
    public delegate void jDataActionDelegate(JournalEventAction _action);
    public string jDataName;
    public JournalData jData;//Журнальное событие
    public string jDataActionName;//имя действия над журнальным событием
    public string id;    //параметры, что использует
    public int argument;//данное действие
    public GameObject obj;//с каким префабом произвести действие
    public jDataActionDelegate jDataAction;//ссылка на функцию, которая соответствует названию выше

}