using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Класс, представляющий собой бафф, который навешивается на персонажа и творит с ним всё, что угодно
/// </summary>
[System.Serializable]
public class BuffClass: ScriptableObject
{

    #region delegates

    delegate void BuffAction(string id, int argument);

    #endregion //delegates

    #region enums

    public enum BuffTypeEnum { stuckable, timeStuckable, notStuckable };

    #endregion //enums

    #region fields

    public List<ActionSign> actionList;

    public string buffName;
    public Sprite buffImage;
    public float buffTime = 0f;//Сколько времени длится бафф. Если 0, то сколько угодно.
    
    public BuffTypeEnum buffType;

    private BuffsList buffList;//Ссылка на список бафов, в который данный бафф находится

    #endregion //fields

    /// <summary>
    ///Конструктор
    /// </summary>
    public BuffClass(BuffClass _buff, BuffsList _buffList)
    {
        buffName = _buff.buffName;
        buffImage = _buff.buffImage;
        buffTime = _buff.buffTime;
        buffType = _buff.buffType;

        buffList = _buffList;

        actionList = _buff.actionList;

        Initialize();
    }


    /// <summary>
    /// Функция инициализации, в которой создаются ссылки на функции, производимые данным баффом
    /// </summary>
    void Initialize()
    {
        Dictionary<string, BuffAction> actionBase = new Dictionary<string, BuffAction>();

        actionBase.Add("poison", Poison);

        foreach (ActionSign aSign in actionList)
        {
            if (actionBase.ContainsKey(aSign.actionName))
            {
                string s = aSign.actionName;
                aSign.aiAction = actionBase[s].Invoke;
            }
        }
    }

    /// <summary>
    /// Функция, что вызывается при навешивании баффа
    /// </summary>
    public virtual void Activate()
    {
        buffList.person.StartCoroutine(BuffProcess());
    }

    IEnumerator BuffProcess()
    {
        foreach (ActionSign act in actionList)
        {
            act.aiAction(act.id, act.argument); 
        }
        yield return new WaitForSeconds(buffTime);
        buffList.RemoveBuff(this);
    }

    /// <summary>
    /// Функция, что вызывается при снятии баффа
    /// </summary>
    public virtual void Deactivate()
    {
    }

    #region interface

    /// <summary>
    /// Отравить
    /// </summary>
    public void Poison(string id, int argument)
    {
        buffList.person.StartCoroutine(PoisonProcess(argument * 1f, this, buffList));
    }


    /// <summary>
    /// Процесс получения яда
    /// </summary>
    IEnumerator PoisonProcess(float dmg, BuffClass _buff, BuffsList _buffList)
    {
        OrganismStats stats = _buffList.OrgStats;   
        for (float i = 0f; i < buffTime; i += 1f)
        {
            if (this == null)
            {
                break;
            }
            if (!_buffList.Contains(this))
            {
                break;
            }
            yield return new WaitForSeconds(1f);
            stats.health -= (100 - stats.aDefence) / 100f * dmg;
            stats.OnHealthChanged(new OrganismEventArgs(0f));
        }
    }

    #endregion //interface

}
