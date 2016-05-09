using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GAF.Core;

/// <summary>
/// Набор действий сложных персонажей, которые сильно меняют игровой процесс
/// </summary>
public class PersonActions : DmgObjActions
{

    #region epsilons

    protected const float velEps = 1f;

    #endregion //epsilons

    #region consts

    protected const int maxEmployment = 10;

    protected float jumpDownTime=1f;
    protected float jumpDownOffset = 4f;

    #endregion //consts

    #region fields

    protected Rigidbody rigid;

    protected PersonVisual cAnim;

    protected PreInteractionChecker interactions;

    public Transform platformCheck;//Индикатор, использующийся для взаимодействия с платформами

    protected Transform sight;//Индикатор, использующийся для зрения персонажа.
    public Transform Sight
    { get { return sight; } }

    public HitClass hitData = null;//Какими параметрами атаки персонаж пользуется в данный момент
    protected HitController hitBox;//хитбокс оружия персонажа

    public Transform target;

    protected EnvironmentStats envStats;

    protected InterObjActions interactionObject;
    public InterObjActions InteractionObject {set { interactionObject = value;}}

    protected float chargeValue = 0f;//Переменная, отвечающая за "заряжающиеся" действия
    public float ChargeValue
    { get { return chargeValue; } set { chargeValue = 0; } }

    protected delegate void itemAction(PersonActions person, ActionData aData, float _chargeValue);
    protected static Dictionary<string, itemAction> itemActionList = new Dictionary<string, itemAction> { { "setUp", SetUpItem }, { "throw", ThrowItem} };

    #endregion //fields

    #region parametres

    public orientationEnum movingDirection;
    protected bool moving;
    protected bool death=false;
    protected Vector2 climbingDirection = new Vector2(0f, 0f);
	//ДОБАВИЛ
	float _hitTime;

    #region speeds

    protected float currentMaxSpeed;

    public float defRunSpeed=0f;
    public float defFastRunSpeed = 0f;
    public float defCrouchSpeed = 0f;
    public float defStairSpeed = 0f;
    public float defRopeSpeed = 0f;
    public float defThicketSpeed = 0f;
    public float defLedgeSpeed = 0f;
    public float defBoxSpeed = 0f;

    protected float runSpeed=0f;
    protected float fastRunSpeed=0f;
    protected float crouchSpeed = 25f;
    protected float ladderSpeed = 0f;
    protected float ropeSpeed = 0f;
    protected float thicketSpeed = 0f;
    protected float ledgeSpeed = 0f;
    protected float boxSpeed = 0f;
    
    public float CurrentSpeed { get { return currentMaxSpeed; } set { } }
    public float RunSpeed { get { return runSpeed;} set { } }
    public float FastRunSpeed { get { return fastRunSpeed; } set { } }

    #endregion //speeds

    public float acceleration;
    public float jumpForce;

    protected bool precipiceIsForward = false;
    public bool PrecipiceIsForward { set { precipiceIsForward = value; } }

    protected bool obstacleIsForward = false;
    public bool ObstacleIsForward {set { obstacleIsForward = value; } }

    protected bool jumpIsPossible = true;
    public bool JumpIsPossible { set { jumpIsPossible = value; } }

    public int employment=maxEmployment;

    #endregion //parametres

    public override void Initialize()
    {
        cAnim = GetComponentInChildren<PersonVisual>();
        platformCheck = transform.FindChild("Indicators").FindChild("PlatformCheck");
        sight = transform.FindChild("Sight");
        climbingDirection = new Vector2(0f, 0f);
    }

    public void SetEnvStats(EnvironmentStats _envStats)
    {
        envStats = _envStats;
    }

    #region Interface

    /// <summary>
    /// Посмотреть в указанном направлении
    /// </summary>
    public virtual void Observe(Vector2 sightDirection)
    {
    }

    /// <summary>
    /// Повернуть персонажа 
    /// </summary>
    /// <param name="direction"></param>
	public virtual void Turn(ActionClass a)
    {
        direction.dir = a.dir;
        Vector3 newScale = this.gameObject.transform.localScale;
        if (SpFunctions.RealSign(newScale.x) != (int)a.dir)
        {
            newScale.x *= -1;
            this.gameObject.transform.localScale = newScale;
        }
    }

    /// <summary>
    /// Повернуться
    /// </summary>
    public virtual void Turn(orientationEnum _direction)
    {
        direction.dir = _direction;
        Vector3 newScale = this.gameObject.transform.localScale;
        if (SpFunctions.RealSign(newScale.x) != (int)_direction)
        {
            newScale.x *= -1;
            this.gameObject.transform.localScale = newScale;
        }
    }

    /// <summary>
    /// Начать передвижение
    /// </summary>
	public virtual void StartWalking(ActionClass a)
    {
        if (!moving)
        {
            moving = true;
        }
		movingDirection = a.dir;
    }

    /// Начать передвижение
    /// </summary>
    public virtual void StartWalking(orientationEnum _direction)
    {
        if (!moving)
        {
            moving = true;
        }
        movingDirection = _direction;
    }

    /// <summary>
    /// Прекратить передвижение
    /// </summary>
    public virtual void StopWalking()
    {
        moving = false;
    }

    /// <summary>
    /// Двигаться с заданной скоростью
    /// </summary>
    protected virtual void Move(orientationEnum _direction, float _speed)
    {
        Vector3 targetVelocity = new Vector3(0f, 0f, 0f);
        if (_direction == orientationEnum.left)
        {
            targetVelocity = new Vector3(-_speed, rigid.velocity.y, rigid.velocity.z);
        }
        else if (_direction == orientationEnum.right)
        {
            targetVelocity = new Vector3(_speed, rigid.velocity.y, rigid.velocity.z);
        }
        if (Vector3.Distance(rigid.velocity, targetVelocity) < velEps)
        {
            rigid.velocity = targetVelocity;
        }
        else
        {
            rigid.velocity = Vector3.Lerp(rigid.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Присесть (либо выйти из состояния приседа)
    /// </summary>
	public virtual void Crouch(bool yes)
    {
    }
    
    /// <summary>
    /// Совершить кувырок
    /// </summary>
	public virtual void Flip(ActionClass a)
    { }

    /// <summary>
    /// Совершить кувырок
    /// </summary>
    public virtual void Flip()
    { }

    /// <summary>
    /// Установить максимальную скорость персонажа
    /// </summary>
    public virtual void SetMaxSpeed(float _speed)
    {
        currentMaxSpeed = _speed;
    }

    /// <summary>
    /// Функция преследования цели
    /// </summary>
    public virtual void Pursue()
    {
    }

    /// <summary>
    /// Функция избегания цели
    /// </summary>
    public virtual void Escape()
    {
    }

    /// <summary>
    /// Совершить прыжок
    /// </summary>
	public virtual void Jump(ActionClass a)
    {
    }

    /// <summary>
    /// Совершить прыжок
    /// </summary>
    public virtual void Jump()
    {
    }

    /// <summary>
    /// Спрыгнуть с платформы
    /// </summary>
	public virtual void JumpDown(ActionClass a)
    {
        StartCoroutine(JumpDownRoutine());
    }

    /// <summary>
    /// Спрыгнуть с платформы
    /// </summary>
    public virtual void JumpDown()
    {
        StartCoroutine(JumpDownRoutine());
    }

    protected virtual IEnumerator JumpDownRoutine()
    {
        Vector3 pos = platformCheck.localPosition;
        float y = pos.y;
        platformCheck.localPosition = new Vector3(pos.x, pos.y - jumpDownOffset, pos.z);
        yield return new WaitForSeconds(jumpDownTime);
        platformCheck.localPosition = pos;
    }

    /// <summary>
    /// Функция прохода через двери
    /// </summary>
    public virtual void GoThroughTheDoor(DoorClass door)
    {
        transform.position = new Vector3 (door.pairDoor.transform.position.x+SpFunctions.RealSign(door.pairDoor.transform.position.x - door.transform.position.x) *DoorClass.changeRoomOffsetX,
                                          door.pairDoor.transform.position.y+DoorClass.changeRoomOffsetY,
                                          door.roomPath.id.coordZ);
        door.OnDoorOpen(new JournalEventArgs());
    }

    /// <summary>
    /// Установить в правой руке нужное оружие
    /// </summary>
    public virtual void SetWeapon(WeaponClass _weapon, string weaponType)
    {
   
    }

    /// <summary>
    /// Функция зацепления за высокое препятствие
    /// </summary>
    public virtual void HangHighObstacle()
    {
    }

    /// <summary>
    /// Функция обхода высокого препятствия
    /// </summary>
    public virtual void AvoidHighObstacle(float height)
    {
    }

    /// <summary>
    /// Функция обхода низкого препятствия
    /// </summary>
    public virtual void AvoidLowObstacle(float height)
    {
        envStats.obstacleness = obstaclenessEnum.noObstcl;
        envStats.interaction = interactionEnum.noInter;
    }

    /// <summary>
    /// Функция взаимодействия со стеной
    /// </summary>
    public virtual void WallInteraction()
    {

    }

    /// <summary>
    /// Зацепиться (за заросли, верёвку, уступ...) или наоборот отпустить
    /// </summary>
    public virtual void Hang(bool yes)
    {
    }

    /// <summary>
    /// Установить направление особого перемещения
    /// </summary>
    /// <param name="direction"></param>
    public virtual void StartClimbing(Vector2 _climbDirection)
    {
        moving = true;
    }

    /// <summary>
    /// Взобраться на платформу
    /// </summary>
    public virtual void ClimbOntoThePlatform()
    {
    }

    /// <summary>
    /// Учёт ситуации и произведение нужной в данный момент атаки
    /// </summary>
	public virtual void Attack(ActionClass a)
    {
        StartCoroutine(AttackProcess());
    }

    /// <summary>
    /// Учёт ситуации и произведение нужной в данный момент атаки
    /// </summary>
    public virtual void Attack()
    {
        StartCoroutine(AttackProcess());
    }

    /// <summary>
    /// //Процесс атаки
    /// </summary>
    protected virtual IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(0f);
    }

    /// <summary>
    /// Использовать предмет.
    /// </summary>
    /// <param name="Какие предметы используются"></param>
    /// <param name="Если true, то действие предмета произойдёт в начале использования. Иначе - в конце"></param>
    public virtual void UseItem(ItemBunch itemBunch)
    {
        UsableItemClass item = (UsableItemClass)itemBunch.item;
        if (item.grounded ? envStats.groundness == groundnessEnum.grounded : (envStats.groundness != groundnessEnum.crouch))
        {
            if (itemActionList.ContainsKey(item.itemAction.actionName))
            {
                StartCoroutine(ItemProcess(item.itemAction,0f));
            }
        }
    }

    /// <summary>
    /// Задержать действие, совершаемое при использовании предмета
    /// </summary>
    public virtual void ChargeItem(ItemBunch itemBunch)
    {
    }

    /// <summary>
    /// Совершить действие, происходящее при прекращении зарядки
    /// </summary>
    public virtual void ReleaseItem(ItemBunch itemBunch)
    {
    }

    protected virtual IEnumerator ItemProcess(ItemActionData itemData, float _chargeValue)
    {
        itemActionList[itemData.actionName](this, itemData, _chargeValue);
        yield return new WaitForSeconds(itemData.actionTime);
    }

    #region itemActions

    protected static void SetUpItem(PersonActions person, ActionData aData, float _chargeValue)
    {
        ItemActionData iData = (ItemActionData)aData;
        GameObject drop = Instantiate(iData.itemObject);
        drop.transform.position = new Vector3(person.transform.position.x, person.transform.position.y, person.transform.position.z + iData.argument);
    }

    protected static void ThrowItem(PersonActions person, ActionData aData, float _chargeValue)
    {
        ItemActionData iData = (ItemActionData)aData;
        GameObject item = Instantiate(iData.itemObject, person.Sight.position, person.Sight.rotation) as GameObject;
        Vector3 scale = item.transform.localScale;
        item.transform.localScale = new Vector3(scale.x * Mathf.Sign(person.transform.localScale.x), scale.y, scale.z);
        Rigidbody rigid = item.GetComponentInChildren<Rigidbody>();
        rigid.AddForce(new Vector3(Mathf.Sign(person.transform.localScale.x) * iData.argument * _chargeValue, iData.argument * _chargeValue/2, 0f));
    }

    #endregion //itemActions

    /// <summary>
    /// Произвести процесс смерти
    /// </summary>
    public override void Death()
    {
        base.Death();
        death= true;
        cAnim.Death();
    }

    #endregion //Interface

    /// <summary>
    /// Задать, какой удар должен наносить персонаж
    /// </summary>
    public virtual void SetHitData(string hitName)
    {       
    }

	public override void Hitted ()
	{
		base.Hitted ();
		//ДОБАВИЛ-------------------------------------------------------
		_hitTime = 0.1f;
		GAFMovieClip[] gafs = GetComponentsInChildren<GAFMovieClip>();
		foreach(GAFMovieClip gaf in gafs)
		{
			if(gaf.individualMaterials!=null)
				gaf.settings.animationColor = Color.red;
		}
		//---------------------------------------------------------------
	}

	//ДОБАВИЛ
	public virtual void FixedUpdate()
	{
		if(_hitTime>0)
		{
			_hitTime-=Time.deltaTime;
		}
		else
		{
			{
				GAFMovieClip[] gafs = GetComponentsInChildren<GAFMovieClip>();
				foreach(GAFMovieClip gaf in gafs)
				{
					if(gaf.individualMaterials!=null)
						gaf.settings.animationColor = Color.white;
				}
			}
		}
	}

	public virtual void Update()
	{
	}

    /// <summary>
    /// Задать поле статов
    /// </summary>
    public virtual void SetStats(EnvironmentStats _stats)
    {
    }

    /// <summary>
    /// Ф-ция, устанавливающая скорости передвижения персонажа
    /// </summary>
    public virtual void SetSpeeds(float speed, float defSpeed)
    {
        float koof = speed / defSpeed;
        runSpeed = defRunSpeed *koof;
        fastRunSpeed = defFastRunSpeed*koof;
        crouchSpeed = defCrouchSpeed* koof;
        ladderSpeed = defStairSpeed * koof;
        ropeSpeed = defRopeSpeed * koof;
        thicketSpeed = defThicketSpeed * koof;
        ledgeSpeed = defLedgeSpeed * koof;
        boxSpeed = defBoxSpeed * koof;
    }

}
