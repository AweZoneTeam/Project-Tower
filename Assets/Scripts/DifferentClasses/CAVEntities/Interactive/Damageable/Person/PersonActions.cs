using UnityEngine;
using System.Collections;
//ДОБАВИЛ
using GAF.Core;

/// <summary>
/// Набор действий сложных персонажей, которые сильно меняют игровой процесс
/// </summary>
public class PersonActions : DmgObjActions
{

    #region consts

    protected const int maxEmployment = 10;

    protected float jumpDownTime=1f;
    protected float jumpDownOffset = 4f;

    #endregion //consts

    #region fields

    protected Rigidbody rigid;

    protected PersonVisual cAnim;

    public Transform platformCheck;//Индикатор, использующийся для взаимодействия с платформами

    public HitClass hitData = null;//Какими параметрами атаки персонаж пользуется в данный момент
    protected HitController hitBox;//хитбокс оружия персонажа

    public Transform target;

    protected EnvironmentStats envStats;

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

    protected float runSpeed=0f;
    protected float fastRunSpeed=0f;
    protected float crouchSpeed = 25f;
    protected float stairSpeed = 0f;
    protected float ropeSpeed = 0f;
    protected float thicketSpeed = 0f;
    protected float ledgeSpeed = 0f;
    
    public float CurrentSpeed { get { return currentMaxSpeed; } set { } }
    public float RunSpeed { get { return runSpeed;} set { } }
    public float FastRunSpeed { get { return fastRunSpeed; } set { } }

    #endregion //speeds

    public float acceleration;
    public float jumpForce;

    protected bool precipiceIsForward = false;
    public bool PrecipiceIsForward { set { precipiceIsForward = value; } }

    protected bool jumpIsPossible = true;
    public bool JumpIsPossible { set { jumpIsPossible = value; } }

    public int employment=maxEmployment;

    #endregion //parametres

    public override void Initialize()
    {
        cAnim = GetComponentInChildren<PersonVisual>();
        platformCheck = transform.FindChild("Indicators").FindChild("PlatformCheck");
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
        Vector3 newScale = this.gameObject.transform.localScale;
        if (SpFunctions.RealSign(newScale.x) != (int)a.dir)
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

    /// <summary>
    /// Прекратить передвижение
    /// </summary>
    public virtual void StopWalking()
    {
        moving = false;
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
    /// Совершить прыжок
    /// </summary>
	public virtual void Jump(ActionClass a)
    {
    }

    /// <summary>
    /// Спрыгнуть с платформы
    /// </summary>
	public virtual void JumpDown(ActionClass a)
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
        transform.position = door.nextPosition;
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
    /// //Процесс атаки
    /// </summary>
    protected virtual IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(0f);
    }

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
        stairSpeed = defStairSpeed * koof;
        ropeSpeed = defRopeSpeed * koof;
        thicketSpeed = defThicketSpeed * koof;
        ledgeSpeed = defLedgeSpeed * koof;
    }

}
