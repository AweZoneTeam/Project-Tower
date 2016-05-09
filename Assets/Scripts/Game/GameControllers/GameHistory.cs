using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Скрипт, ведущий запись игровой истории и реагирующий на значимые игровые события. Отвечает за скриптовые моменты и за правильное течение событий игры.
/// </summary>
public class GameHistory : MonoBehaviour
{

    #region fields

    public JournalScriptStock journalEvents = new JournalScriptStock();

    public void Awake()
    {
        journalEvents.Initialize();
    }

    #endregion //fields

}

/// <summary>
/// Особый список журнальных сриптов, в котором добавлены ф-ции при добавлении новых событий
/// </summary>
[System.Serializable]
public class JournalScriptStock
{

    #region eventHandlers

    public EventHandler<JournalRefreshEventArgs> NewJournalDataEvent;
    public EventHandler<JournalRefreshEventArgs> QuestCompletedEvent;

    #endregion //eventHandlers

    #region fields

    public List<JournalScriptInitializer> initList = new List<JournalScriptInitializer>();
    public List<JournalEventScript> journalList = new List<JournalEventScript>();


    public delegate void jDataActionDelegate(JournalEventAction _action);
    public delegate void jDataConditionDelegate(JournalEventScript _script, GameObject obj);

    private Dictionary<string, jDataActionDelegate> jActionBase=new Dictionary<string, jDataActionDelegate>();//действие
    private Dictionary<string, jDataConditionDelegate> jConditionBase=new Dictionary<string, jDataConditionDelegate>();//подписка
    private Dictionary<string, jDataConditionDelegate> jDeInitBase = new Dictionary<string, jDataConditionDelegate>();//отписка

    #endregion fields

    #region interface

    //то, что вызывается в самом начале
    public void Initialize()
    {
        FormJournalBase();

        foreach (JournalEventScript _script in journalList)
        {
            InitializeScript(_script);
        }
    }

    /// <summary>
    /// Сформировать базы данных
    /// </summary>
    void FormJournalBase()
    {
        jActionBase.Clear();
        jConditionBase.Clear();
        jDeInitBase.Clear();

        jActionBase.Add("add", SendNewData);
        jActionBase.Add("completeQuest", QuestComplete);

        jConditionBase.Add("startGame", GameBeginInit);
        jConditionBase.Add("doorOpened", DoorOpenInit);
        jConditionBase.Add("enemyIsDead", EnemyDeathInit);

        jDeInitBase.Add("startGame", GameBeginDeInit);
        jDeInitBase.Add("doorOpened", DoorOpenDeInit);
        jDeInitBase.Add("enemyIsDead", EnemyDeathDeInit);

    }

    /// <summary>
    /// "Сказать" объекту jDataTarget, что он является необходимым для свершения журнального скрипта, то есть подписаться на его события 
    /// </summary>
    public void InitializeScript(JournalEventScript _script)
    {
        _script.jList = this;
        GameObject jTarget=null;
        foreach (JournalScriptInitializer init in initList)
        {
            if (init.jScript == _script)
            {
                jTarget = init.obj;
                break;
            }
        }

        //В первую очередь, подпишемся на журнальные объекты
        if (jConditionBase.ContainsKey(_script.jDataConditionName))
        {
            string s = _script.jDataConditionName;
            jConditionBase[s].Invoke(_script, jTarget);
        }

        foreach (JournalEventAction _action in _script.jActions)
        {
            if (jActionBase.ContainsKey(_action.jDataActionName))
            {
                string s = _action.jDataActionName;
                _action.jDataAction = jActionBase[s].Invoke;
            }
        }
    }

    /// <summary>
    /// "Сказать" объекту jDataTarget, что он БОЛЬШЕ НЕ является необходимым для свершения журнального скрипта, то есть отписаться от его событий 
    /// </summary>
    public void DeInitializeScript(JournalEventScript _script)
    {
        GameObject jTarget = null;
        foreach (JournalScriptInitializer init in initList)
        {
            if (init.jScript == _script)
            {
                jTarget = init.obj;
                break;
            }
        }

        //В первую очередь, подпишемся на журнальные объекты
        if (jDeInitBase.ContainsKey(_script.jDataConditionName))
        {
            string s = _script.jDataConditionName;
            jDeInitBase[s].Invoke(_script, jTarget);
        }

    }

    /// <summary>
    /// Добавить в список новый журнальный скрипт
    /// </summary>
    public void AddJournalScript(JournalEventScript _script)
    {
        journalList.Add(_script);
        InitializeScript(_script);
    }

    /// <summary>
    /// Убрать из списка журнальный скрипт
    /// </summary>
    public void RemoveJournalScript(JournalEventScript _script)
    {
        if (journalList.Contains(_script))
        {
            journalList.Remove(_script);
            DeInitializeScript(_script);
        }
    }

    #endregion //interface

    #region journalActions

    /// <summary>
    /// Послать новую информацию в журнал
    /// </summary>
    public void SendNewData(JournalEventAction _action)
    {
        this.OnNewJournalData(new JournalRefreshEventArgs(_action.jData));
    }

    /// <summary>
    /// Пометить задание как выполненное
    /// </summary>
    /// <param name="_action"></param>
    public void QuestComplete(JournalEventAction _action)
    {
        QuestData data = (QuestData)_action.jData;
        data.passed = true;
        this.OnQuestCompleted(new JournalRefreshEventArgs(data));
    }

    #endregion //journalActions

    #region initFunctions

    /// <summary>
    /// Вызвать событие в начале игры
    /// </summary>
    void GameBeginInit(JournalEventScript _script, GameObject obj)
    {
        obj.GetComponent<GameStatistics>().GameStatsJournalEvent += _script.HandleJournalEvent;
    }

    /// <summary>
    /// Вызвать событие при открытии определённой двери
    /// </summary>
    void DoorOpenInit(JournalEventScript _script, GameObject obj)
    {
        obj.GetComponent<DoorClass>().DoorOpenJournalEvent += _script.HandleJournalEvent;
    }

    /// <summary>
    /// Вызвать событие при смерти определённого монстра
    /// </summary>
    void EnemyDeathInit(JournalEventScript _script, GameObject obj)
    {
        obj.GetComponent<EnemyController>().EnemyDieJournalEvent += _script.HandleJournalEvent;
    }

    #endregion //initFunctions

    #region deInitFunctions

    /// <summary>
    /// Отписка от события "начало игры"
    /// </summary>
    void GameBeginDeInit(JournalEventScript _script, GameObject obj)
    {
        obj.GetComponent<GameStatistics>().GameStatsJournalEvent -= _script.HandleJournalEvent;
    }

    /// <summary>
    /// Отписка от события "открытие двери"
    /// </summary>
    void DoorOpenDeInit(JournalEventScript _script, GameObject obj)
    {
        obj.GetComponent<DoorClass>().DoorOpenJournalEvent -= _script.HandleJournalEvent;
    }

    /// <summary>
    /// Отписка от события "смерть персонажа"
    /// </summary>
    void EnemyDeathDeInit(JournalEventScript _script, GameObject obj)
    {
        obj.GetComponent<EnemyController>().EnemyDieJournalEvent -= _script.HandleJournalEvent;
    }

    #endregion //deInitFunctions

    #region events

    /// <summary>
    /// Событие, вызываемое при добавлении в журнал новых данных
    /// </summary>
    public void OnNewJournalData(JournalRefreshEventArgs e)
    {
        EventHandler<JournalRefreshEventArgs> handler = NewJournalDataEvent;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    /// <summary>
    /// Событие, вызываемое при выполнении какого-либо задания
    /// </summary>
    public void OnQuestCompleted(JournalRefreshEventArgs e)
    {
        EventHandler<JournalRefreshEventArgs> handler = QuestCompletedEvent;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    #endregion //events

}


/// <summary>
/// Класс, необходимый для инициализации журнальных скриптов
/// </summary>
[System.Serializable]
public class JournalScriptInitializer
{
    public JournalEventScript jScript;
    public GameObject obj;
}
