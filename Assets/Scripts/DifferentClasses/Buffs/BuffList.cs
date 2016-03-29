using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, представляющий коллекцию баффов, навешанных на персонажа.
/// </summary>
public class BuffsList : List<BuffClass>
{

    #region eventHandlers

    public EventHandler<BuffsChangedEventArgs> BuffsChangedEvent;

    #endregion //eventHandlers

    #region fields

    private OrganismStats orgStats;

    public PersonController person;//Носитель этих баффов

    public OrganismStats OrgStats
    {
        get { return orgStats; }
        set { orgStats = value; }
    }

    #endregion //fields

    /// <summary>
    /// Конструктор, вызываемый носителем баффов
    /// </summary>
    public BuffsList(PersonController _person)
    {
        person = _person;
    }

    /// <summary>
    /// Добавление баффа 
    /// </summary>
    public void AddBuff(BuffClass _buff)
    {
        bool k = false;
        if (_buff.buffType == BuffClass.BuffTypeEnum.notStuckable)
        {
            foreach (BuffClass buff in this)
            {
                if (string.Equals(buff.buffName, _buff.buffName))
                {
                    k = true;
                    break;
                }
            }
        }
        else if (_buff.buffType == BuffClass.BuffTypeEnum.timeStuckable)
        {
            foreach (BuffClass buff in this)
            {
                if (string.Equals(buff.buffName, _buff.buffName))
                {
                    RemoveBuff(buff);
                    break;
                }
            }
        }
        if (!k)
        {
            BuffClass newBuff = new BuffClass(_buff, this);
            this.Add(newBuff);
            newBuff.Activate();
            OnBuffsChanged(new BuffsChangedEventArgs(newBuff));
        }
    }

    /// <summary>
    /// Удаление баффа из списка
    /// </summary>
    public void RemoveBuff(BuffClass _buff)
    {
        if (this.Contains(_buff))
        {
            this.Remove(_buff);
        }
        _buff.Deactivate();

        OnBuffsChanged(new BuffsChangedEventArgs(_buff));
    }

    #region events

    public void OnBuffsChanged(BuffsChangedEventArgs e)
    {
        EventHandler<BuffsChangedEventArgs> handler = BuffsChangedEvent;

        if (handler != null)
        {
            handler(this, e);
        }
    }

    #endregion //events

}
