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

    protected const float groundRadius = 1f;
    protected const float preGroundRadius = 2f;

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
    protected GroundChecker frontWallCheck;//Индикатор, которым персонаж определяет непроходимое препятствие

    #endregion //indicators
    
    #region fields

    /*private*/
    [SerializeField]protected EnvironmentStats envStats;
    [SerializeField]protected BagClass bag;
    protected BuffsList bList;//Список баффов, навешанных на персонажа.

    public BuffsList buffList
    {
        get { return bList; }
        set { bList = value; }
    }

    protected Rigidbody rigid;

    protected PersonActions pActions;
    protected PersonVisual cAnim;
    protected PreInteractionChecker interactions;
    protected HitController hitBox;//То, чем персонаж атакует

    protected InterObjActions interactionObject;//Объект, с которым взаимодействует персонаж. Это поле используется в особых случаях.

    #endregion //fields

    #region parametres

    public AreaClass currentRoom;//В какой комнате находится персонаж в данный момент

    protected bool fallDamaged = false;
    protected float fallDamage = 0f;

    [SerializeField]protected int employment;

    public LayerMask whatIsGround;
    protected LayerMask whatIsWall=LayerMask.GetMask("door", "ground");
    public LayerMask whatIsInteractable;

    #endregion //parametres

    #region Interface

    public override void Initialize()
    {
        Transform indicators = transform.FindChild("Indicators");
        if (indicators != null)
        {
            groundCheck = indicators.FindChild("GroundCheck");
            interCheck = indicators.FindChild("InterCheck");
            precipiceCheck = indicators.FindChild("PrecipiceCheck");
            sight = transform.FindChild("Sight");
            frontWallCheck = indicators.FindChild("FrontWallCheck")!=null? indicators.FindChild("FrontWallCheck").GetComponent<GroundChecker>():null;
        }
        bList = new BuffsList(this);
        rigid = GetComponent<Rigidbody>();
        if (direction == null)
        {
            direction = new Direction();
        }
        if (orgStats == null)
        {
            orgStats = new OrganismStats();
        }
        if (envStats == null)
        {
            envStats = new EnvironmentStats();
        }
        bList.OrgStats = orgStats;
        pActions = GetComponent<PersonActions>();
        if (pActions != null)
        {
            SetAction();
        }
        cAnim = GetComponentInChildren<PersonVisual>();
        if (cAnim != null)
        {
            SetVisual();
        }
        interactions = transform.FindChild("Indicators").gameObject.GetComponentInChildren<PreInteractionChecker>();
        if (interactions != null)
        {
            SetInteractions();
        }
        if (currentRoom != null)
        {
            currentRoom.container.Add(this);
        }
        hitBox = GetComponentInChildren<HitController>();
    }
    


    protected override void SetAction()
    {
        pActions.Initialize();
        pActions.SetDirection(direction);
        pActions.SetOrgStats(orgStats);
        pActions.SetEnvStats(envStats);
    }

    protected override void SetVisual()
    {
        cAnim.SetDirection(direction);
        cAnim.SetOrgStats(orgStats);
        cAnim.SetEnvStats(envStats);
    }

    protected virtual void SetInteractions()
    {
        interactions.Person = this;
        interactions.Rigid = rigid;
        interactions.Initialize();
    }

    public virtual AreaClass GetRoomPosition()
    {
        return currentRoom;
    }

    public virtual void SetRoomPosition(AreaClass _room)
    {
        currentRoom = _room;
    }

    public virtual BagClass GetEquipment()
    {
        return bag;
    }

    public EnvironmentStats GetEnvStats()
    {
        if (envStats == null)
        {
            envStats = new EnvironmentStats();
        }
        return envStats;
    }

    public void SetInteractionObject(InterObjActions _interactionObject)
    {
        interactionObject = _interactionObject;
        if (pActions != null)
        {
            pActions.InteractionObject = _interactionObject;
        }
    }

    /// <summary>
    /// Если к такому объекту подойдёт персонаж и будет взаимодействовать, то контроллер предпримет нужные действия
    /// </summary>
    public override void Interact(InterObjController interactor)
    {
        if (pActions != null)
        {
            pActions.SetInteractor(interactor);
            pActions.Interact();
        }
    }

    /// <summary>
    /// Использовать активный предмет инвентаря
    /// </summary>
    protected virtual void UseItem()
    {
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
        if (interactions != null)
        {
            interactions.ZCoordinate=0;
        }
    }

    /// <summary>
    /// Сменить комнату
    /// </summary>
    protected virtual void ChangeRoom(AreaClass nextRoom)
    {
        if (currentRoom != null)
        {
            currentRoom.container.Remove(this);
            currentRoom = nextRoom;
            currentRoom.container.Add(this);
        }
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
        CheckObstacles();
    }

    /// <summary>
    /// Функция, определяющая, где персонаж находится по отношению к твёрдой поверхности 
    /// </summary>
    protected virtual void DefineGroundness()
    {
        if (Physics.OverlapSphere(groundCheck.position,groundRadius,whatIsGround).Length>0)
        {
            if (envStats.groundness!=groundnessEnum.crouch)
            {
                envStats.groundness = groundnessEnum.grounded;
            }
        }
        else if (Physics.OverlapSphere(groundCheck.position, groundRadius, whatIsGround).Length > 0)
        {
            envStats.groundness = groundnessEnum.preGround;
        }
        else
        {
            envStats.groundness = groundnessEnum.inAir;
        }
    }

    /// <summary>
    /// Функция, определяющая, был ли нанесён персонажу урон из-за падения с большой высоты
    /// </summary>
    protected virtual void DefineFallDamage()
    {
        if (rigid != null)
        {
            if ((rigid.velocity.y < -1f * minDmgFallSpeed)&&(envStats.groundness==groundnessEnum.inAir))
            {
                fallDamage = dmgPerSpeed * Mathf.Abs(rigid.velocity.y + minDmgFallSpeed);
            }
            else if (envStats.groundness==groundnessEnum.inAir)
            {
                fallDamage = 0f;
            }
            if ((fallDamage > 0f) && ((int)envStats.groundness < 4))
            {
                orgStats.health -= fallDamage;
                orgStats.OnHealthChanged(new OrganismEventArgs(0f));
                fallDamage = 0f;
            }
        }
    }

    /// <summary>
    /// Оценить, находится ли спереди пропасть
    /// </summary>
    protected virtual void DefinePrecipice()
    {
        pActions.PrecipiceIsForward = (!(Physics.OverlapSphere(precipiceCheck.position, precipiceRadius, whatIsGround).Length > 0) && (envStats.groundness == groundnessEnum.grounded));
    }

    /// <summary>
    /// Определить, с каким особым перемещением персонаж имеет дело
    /// </summary>
    protected virtual void DefineInteractable()
    {
    }

    /// <summary>
    /// Учесть препятствия, ставшие на пути персонажа
    /// </summary>
    protected virtual void CheckObstacles()
    {
        if (frontWallCheck!= null? frontWallCheck.GetCount() > 0:false)
        {
            envStats.obstacleness = obstaclenessEnum.wall;
        }
        else
        {
            envStats.obstacleness = obstaclenessEnum.noObstcl;
        }
    }

    /// <summary>
    /// Поменять коллайдеру z-координатам.
    /// </summary>
    public virtual void ChangeColliderZCoordinate(float z)
    {
        BoxCollider[] cols = GetComponents<BoxCollider>();
        List<Transform> wallChecks = new List<Transform>();
        Vector3 center;
        
        for (int i = 0; i < cols.Length; i++)
        {
            center = cols[i].center;
            cols[i].center = new Vector3(center.x, center.y, z);
        }
        Transform indicators;
        if ((indicators=transform.FindChild("Indicators"))!= null)
        {
            indicators.localPosition = new Vector3(indicators.localPosition.x, indicators.localPosition.y, z);
        }
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
    private Direction direction;
    private OrganismStats orgStats;
    private EnvironmentStats envStats;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PersonController obj = (PersonController)target;
        direction = obj.GetDirection();
        orgStats = obj.GetOrgStats();
        envStats = obj.GetEnvStats();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.EnumMaskField("direction", direction.dir);
        //orgStats.maxHealth = EditorGUILayout.FloatField("Max Health", orgStats.maxHealth);
        //EditorGUILayout.FloatField("Health", orgStats.health);
    }
}
#endif