﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// Контроллер персонажей
/// </summary>
public class PersonController : DmgObjController, IPersonWatching
{

    #region consts
    protected const float groundRadius = 0.2f;
    protected const float preGroundRadius = 0.7f;

    protected const float minDmgFallSpeed = 80f;
    protected const float dmgPerSpeed = 5f;

    protected const float precipiceRadius = 0.2f;
    #endregion //consts

    #region indicators

    protected Transform groundCheck; //Индикатор, оценивающий расстояние до земли
    protected Transform precipiceCheck;//Индикатор, проверяющий, находится ли впереди пропасть

    #endregion //indicators

    #region fields
    /*private*/
    protected Stats stats;
    protected Rigidbody rigid;
    [SerializeField]protected EquipmentClass equip;//Экипировка персонажа
    protected PersonActions actions;

    protected List<PersonController> whoWatchesMe=new List<PersonController>();
   
    #endregion //fields

    #region parametres

    public AreaClass currentRoom;//В какой комнате находится персонаж в данный момент

    protected bool fallDamaged = false;
    protected float fallDamage = 0f;

    public LayerMask whatIsGround;

    #endregion //parametres

    #region Interface

    public override void Initialize()
    {
        groundCheck = transform.FindChild("Indicators").FindChild("GroundCheck");
        precipiceCheck = transform.FindChild("Indicators").FindChild("PrecipiceCheck");
    }

    public virtual AreaClass GetRoomPosition()
    {
        return currentRoom;
    }

    public virtual void SetRoomPosition(AreaClass _room)
    {
        currentRoom = _room;
    }

    public virtual EquipmentClass GetEquipment()
    {
        return equip;
    }

    public override Prestats GetStats()
    {
        if (stats == null)
        {
            stats = new Stats();
        }
        return stats;
    }

    /// <summary>
    /// Функция, что реализует тот факт, что какой-то персонаж стал наблюдать за действиями данного
    /// </summary>
    public virtual void WatchYou(PersonController person)
    {
        if (!whoWatchesMe.Contains(person))
        {
            whoWatchesMe.Add(person);
        }
    }

    /// <summary>
    /// Функция, что реализует тот факт, что какой-то персонаж перестал наблюдать за действиями данного
    /// </summary>
    public virtual void StopWatchingYou(PersonController person)
    {
        if (whoWatchesMe.Contains(person))
        {
            whoWatchesMe.Remove(person);
        }
    }

    /// <summary>
    /// Функция взаимодействия с дверями. В данном классе Исполняется та часть, что сообщает "подписчикам" данного контроллера (то есть персонажам, что следят за носителем этого контроллера), мол, я открываю дверь
    /// </summary>
    protected virtual void DoorInteraction()
    {
        for (int i = 0; i < whoWatchesMe.Count; i++)
        {
            ExecuteEvents.Execute<IPersonWatching>(whoWatchesMe[i].gameObject, null, (x, y) => x.TargetMakeAnAction("DoorInteraction"));
        }
    }
    
    /// <summary>
    /// Как контроллеры, следящие за целью должны отреагировать на совершение данного объекта действия
    /// </summary>
    public virtual void TargetMakeAnAction(string actionName)
    {

    }

    //Функция, что анализирует обстановку, окружающую персонажа
    protected virtual void AnalyzeSituation()
    {
        DefineGroundness();
        DefineFallDamage();
        DefinePrecipice();
    }

    /// <summary>
    /// Функция, определяющая, где персонаж находится по отношению к твёрдой поверхности 
    /// </summary>
    protected virtual void DefineGroundness()
    {
        if (Physics.OverlapSphere(groundCheck.position,groundRadius,whatIsGround).Length>0)
        {
            stats.groundness = groundnessEnum.grounded;
        }
        else if (Physics.OverlapSphere(groundCheck.position, groundRadius, whatIsGround).Length > 0)
        {
            stats.groundness = groundnessEnum.preGround;
        }
        else
        {
            stats.groundness = groundnessEnum.inAir;
        }
    }

    /// <summary>
    /// Функция, определяющая, был ли нанесён персонажу урон из-за падения с большой высоты
    /// </summary>
    protected virtual void DefineFallDamage()
    {
        if (rigid != null)
        {
            if ((rigid.velocity.y < -1f * minDmgFallSpeed)&&(stats.groundness==groundnessEnum.inAir))
            {
                fallDamage = dmgPerSpeed * Mathf.Abs(rigid.velocity.y + minDmgFallSpeed);
            }
            else if (stats.groundness==groundnessEnum.inAir)
            {
                fallDamage = 0f;
            }
            if ((fallDamage > 0f) && ((int)stats.groundness < 4))
            {
                stats.health -= fallDamage;
                fallDamage = 0f;
            }
        }
    }

    /// <summary>
    /// Оценить, находится ли спереди пропасть
    /// </summary>
    protected virtual void DefinePrecipice()
    {
    }


    #endregion //Interface
}

/// <summary>
/// Редактор контроллеров персонажей
/// </summary>
[CustomEditor(typeof(PersonController))]
public class PersonEditor : DmgObjEditor
{
    private Stats stats;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PersonController obj = (PersonController)target;
        stats = (Stats)obj.GetStats();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.IntField("direction", (int)stats.direction);
        stats.maxHealth = EditorGUILayout.FloatField("Max Health", stats.maxHealth);
        EditorGUILayout.FloatField("Health", stats.health);
    }
}