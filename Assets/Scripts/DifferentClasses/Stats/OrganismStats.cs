using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Тип статов, который есть у тех игровых объектов, которым можно нанести урон
/// </summary>
[System.Serializable]
public class OrganismStats
{

    #region consts

    public float defVelocity
    {
        get { return 2.5f; }
        set { }
    }

    #endregion //consts

    #region timers
    public float stunTimer;
    #endregion //timers 

    public float health;//Здоровье
    public float maxHealth;//Максимальное кол-во здоровья, которое может быть у персонажа
    public float velocity;//Скорость персонажа, модификатор, который ускоряет или замедляет его
    public DefenceClass defence;//Защита
    public DefenceClass addDefence;//Дополнительная защита
    public int critResistance;//Сопротивляемость к критическим ударам. Каков шанс, что нанесённый урон не будет критическим.
    public hittedEnum hitted;//Насколько "сбит" персонаж с ног влетевшей атакой. 
                             //0-атака не сбила его либо её вообще не было, 1-атака привела его в микростан, 2-атака сильно сбила его
    public float microStun, macroStun;//Как долго персонаж бездействует при hitted=1 и hitted>1 соотвественно

    #region eventHandlers

    public event EventHandler<OrganismEventArgs> HealthChangedEvent;//Событие, вызываемое при изменении здоровья
    public event EventHandler<OrganismEventArgs> ParametersChangedEvent;//Событие, вызываемое при изменении параметров

    #endregion //eventHandlers

    public OrganismStats()
    {
        maxHealth = 100f;
        health = 100f;
        velocity = 2.5f;
        defence = new DefenceClass();
        defence.stability = 1;
        addDefence = new DefenceClass();
        critResistance = 80;
    }

    public IEnumerator Stunned(float time)//Процесс стана
    {
        float _time = time;
        while (stunTimer > 0f)
        {
            yield return new WaitForSeconds(_time);
            stunTimer -= _time;
            _time = stunTimer;
        }
        hitted = hittedEnum.noHit;
    }

    #region events

    public virtual void OnHealthChanged(OrganismEventArgs e)
    {
        EventHandler<OrganismEventArgs> handler = HealthChangedEvent;
        if (handler != null)
        {
            e.HP = health;
            e.MAXHP = maxHealth;
            handler(this, e);
        }
    }

    public virtual void OnParametersChanged(OrganismEventArgs e)
    {
        EventHandler<OrganismEventArgs> handler = ParametersChangedEvent;
        if (handler != null)
        {
            e.HP = health;
            e.DEFENCE = defence;
            e.VELOCITY = velocity;
            e.MAXHP = maxHealth;
            handler(this, e);
        }
    }

    #endregion //events

}

