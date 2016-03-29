using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Контроллер персонажей
/// </summary>
public class PersonController : DmgObjController, IPersonWatching
{

    #region consts

    protected const float groundRadius = 0.2f;
    protected const float preGroundRadius = 0.7f;

    protected const float interRadius = 1f;

    protected const float minDmgFallSpeed = 80f;
    protected const float dmgPerSpeed = 5f;

    protected const int maxEmployment = 10;

    protected const float precipiceRadius = 0.2f;

    #endregion //consts

    #region eventHandlers

    public event EventHandler<RoomChangedEventArgs> RoomChangedEvent;
      
    #endregion //eventHandlers

    #region indicators

    protected Transform groundCheck; //Индикатор, оценивающий расстояние до земли
    protected Transform precipiceCheck;//Индикатор, проверяющий, находится ли впереди пропасть
    protected Transform interCheck;//Индикатор, определяющий, где находятся средства особого перемещения (заросли, верёвки...)
    protected Transform sight;//Откуда персонаж смотрит

    #endregion //indicators
    
    #region fields
    /*private*/
    protected Stats stats;
    protected BuffsList bList;//Список баффов, навешанных на персонажа.
    public BuffsList buffList
    {
        get { return bList; }
        set { bList = value; }
    }

    protected Rigidbody rigid;
    [SerializeField]protected EquipmentClass equip;//Экипировка персонажа
    protected PersonActions actions;
   
    #endregion //fields

    #region parametres

    public AreaClass currentRoom;//В какой комнате находится персонаж в данный момент

    protected bool fallDamaged = false;
    protected float fallDamage = 0f;

    protected int employment;

    public LayerMask whatIsGround;
    public LayerMask whatIsInteractable;

    #endregion //parametres

    #region Interface

    public override void Initialize()
    {
        groundCheck = transform.FindChild("Indicators").FindChild("GroundCheck");
        interCheck = transform.FindChild("Indicators").FindChild("InterCheck");
        precipiceCheck = transform.FindChild("Indicators").FindChild("PrecipiceCheck");
        sight= transform.FindChild("Indicators").FindChild("Sight");
        bList = new BuffsList(this);
        bList.OrgStats = stats;
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
    /// Организуется подписка на заданные события
    /// </summary>
    public virtual void WatchYou(PersonController person)
    {
        person.RoomChangedEvent += TargetChangeRoom;
    }

    /// <summary>
    /// Функция, что реализует тот факт, что какой-то персонаж перестал наблюдать за действиями данного
    /// </summary>
    public virtual void StopWatchingYou(PersonController person)
    {
        person.RoomChangedEvent -= TargetChangeRoom;
    }

    /// <summary>
    /// Функция взаимодействия с дверями. В данном классе Исполняется та часть, что сообщает "подписчикам" данного контроллера (то есть персонажам, что следят за носителем этого контроллера), мол, я открываю дверь
    /// </summary>
    protected virtual void DoorInteraction()
    {
        OnRoomChanged(new RoomChangedEventArgs(currentRoom));
    }
    
    /// <summary>
    /// Как контроллеры, следящие за целью должны отреагировать на совершение данного объекта действия
    /// </summary>
    public virtual void TargetMakeAnAction(string actionName)
    {

    }

    /// <summary>
    /// Что произойдёт, если цель уйдёт в другую комнату
    /// </summary>
    protected virtual void TargetChangeRoom(object sender, RoomChangedEventArgs e)
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
            if (stats.groundness!=groundnessEnum.crouch)
            {
                stats.groundness = groundnessEnum.grounded;
            }
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
                stats.OnHealthChanged(new OrganismEventArgs(0f));
                fallDamage = 0f;
            }
        }
    }

    /// <summary>
    /// Оценить, находится ли спереди пропасть
    /// </summary>
    protected virtual void DefinePrecipice()
    {
        actions.PrecipiceIsForward = (!(Physics.OverlapSphere(precipiceCheck.position, precipiceRadius, whatIsGround).Length > 0) && (stats.groundness == groundnessEnum.grounded));
    }

    /// <summary>
    /// Определить, с каким особым перемещением персонаж имеет дело
    /// </summary>
    protected virtual void DefineInteractable()
    {
    }

    /// <summary>
    /// Поменять коллайдеру z-координатам.
    /// </summary>
    public virtual void ChangeColliderZCordinate(float z)
    {
        BoxCollider[] cols = GetComponents<BoxCollider>();
        Vector3 center;
        for (int i = 0; i < cols.Length; i++)
        {
            center = cols[i].center;
            cols[i].center = new Vector3(center.x, center.y, z);
        }
        groundCheck.position = new Vector3(groundCheck.position.x, groundCheck.position.y, z);
        precipiceCheck.position = new Vector3(precipiceCheck.position.x, precipiceCheck.position.y, z);
    }

    #endregion //Interface

    #region events

    public virtual void OnRoomChanged(RoomChangedEventArgs e)
    {
        EventHandler<RoomChangedEventArgs> handler = RoomChangedEvent;
        if (handler != null)
        {
            e.Room = currentRoom;
            handler(this, e);
        }
    }

    #endregion //events

}

#if UNITY_EDITOR
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
#endif