using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Класс, представляющий собой бафф, который навешивается на персонажа и творит с ним всё, что угодно
/// </summary>
[System.Serializable]
public class BuffClass : ScriptableObject
{

    #region delegates

    delegate void BuffAction(string id, int argument);

    #endregion //delegates

    #region enums

    public enum BuffTypeEnum { stuckable, timeStuckable, notStuckable };

    #endregion //enums

    #region fields

    public List<ReactionSign> actionList = new List<ReactionSign>();

    public string buffName;
    public Sprite buffImage;
    public float buffTime = 0f;//Сколько времени длится бафф. Если 0, то сколько угодно.
    public bool removable;//Если true, то этот бафф можно снять с помощью чекпоинта
    public bool armorSetBuff;//Если true, то этот бафф накладывается при одевании особого сета

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
        Dictionary<string, BuffAction> contrActionBase = new Dictionary<string, BuffAction>();

        actionBase.Add("addStats",AddStats);
        actionBase.Add("poison", Poison);

        contrActionBase.Add("addStats", RemoveStats);

        foreach (ReactionSign aSign in actionList)
        {
            if (contrActionBase.ContainsKey(aSign.actionName))
            {
                string s = aSign.actionName;
                aSign.reAction = contrActionBase[s].Invoke;
            }
        }

        foreach (ReactionSign aSign in actionList)
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
        foreach (ReactionSign act in actionList)
        {
            act.aiAction(act.id, act.argument);
        }
        yield return new WaitForSeconds(buffTime);
        if (buffTime != 0)
        {
            buffList.RemoveBuff(this);
        }
    }

    /// <summary>
    /// Функция, что вызывается при снятии баффа
	/// </summary>
	public virtual void Deactivate()
	{
		foreach (ReactionSign act in actionList)
		{
			if(act.reAction!=null)
				act.reAction(act.id, act.argument);
		}
	}

	#region interface

	/// <summary>
    /// Увеличить статы персонажу
    /// </summary>
    public void AddStats(string id, int argument)
    {
        OrganismStats orgStats = buffList.OrgStats;
        if (string.Equals(id, "physicDefence"))
        {
            orgStats.defence.pDefence += argument;
        }
        else if (string.Equals(id, "fireDefence"))
        {
            orgStats.defence.fDefence += argument;
        }
        else if (string.Equals(id, "acidDefence"))
        {
            orgStats.defence.aDefence += argument;
        }
        else if (string.Equals(id, "darknessDefence"))
        {
            orgStats.defence.dDefence += argument;
        }
        else if (string.Equals(id, "stability"))
        {
            orgStats.defence.stability += argument;
        }
        else if (string.Equals(id, "velocity"))
        {
            orgStats.velocity += argument/10f;
        }

    }
    /// <summary>
    /// Уменьшить статы персонажу
    /// </summary>
    public void RemoveStats(string id, int argument)
    {
        OrganismStats orgStats = buffList.OrgStats;
        if (string.Equals(id, "physicDefence"))
        {
            orgStats.defence.pDefence -= argument;
        }
        else if (string.Equals(id, "fireDefence"))
        {
            orgStats.defence.fDefence -= argument;
        }
        else if (string.Equals(id, "acidDefence"))
        {
            orgStats.defence.aDefence -= argument;
        }
        else if (string.Equals(id, "darknessDefence"))
        {
            orgStats.defence.dDefence -= argument;
        }
        else if (string.Equals(id, "stability"))
        {
            orgStats.defence.stability -= argument;
        }
        else if (string.Equals(id, "velocity"))
        {
            orgStats.velocity -= argument / 10f;
        }

    }
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
        OrganismStats orgStats = _buffList.OrgStats;   
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
            orgStats.health -= (100 - orgStats.defence.aDefence) / 100f * dmg;
            orgStats.OnHealthChanged(new OrganismEventArgs(0f));
        }
    }

    #endregion //interface

}
