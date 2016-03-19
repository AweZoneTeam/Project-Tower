using UnityEngine;
using System.Collections;

/// <summary>
/// Набор действий сложных персонажей, которые сильно меняют игровой процесс
/// </summary>
public class PersonActions : DmgObjActions
{

    #region consts

    protected const int maxEmployment = 10;

    #endregion //consts

    #region fields

    protected Rigidbody rigid;

    protected PersonVisual cAnim;

    public HitClass hitData = null;//Какими параметрами атаки персонаж пользуется в данный момент
    protected HitController hitBox;//хитбокс оружия персонажа

    public Transform target;

    #endregion //fields

    #region parametres

    public orientationEnum movingDirection;
    protected bool moving;
    protected bool death=false;

    public float maxSpeed;
    public float currentMaxSpeed;
    public float fastRunSpeed=0f;

    public float acceleration;
    public float jumpForce;

    protected bool precipiceIsForward = false;
    public bool PrecipiceIsForward { set { precipiceIsForward = value; } }

    protected bool jumpIsPossible = true;
    public bool JumpIsPossible { set { jumpIsPossible = value; } }

    public int employment=maxEmployment;

    #endregion //parametres

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        cAnim = GetComponentInChildren<PersonVisual>();
    }

    #region Interface

    /// <summary>
    /// Повернуть персонажа 
    /// </summary>
    /// <param name="direction"></param>
    public virtual void Turn(orientationEnum direction)
    {
        Vector3 newScale = this.gameObject.transform.localScale;
        if (SpFunctions.RealSign(newScale.x) != (int)direction)
        {
            newScale.x *= -1;
            this.gameObject.transform.localScale = newScale;
        }
    }

    /// <summary>
    /// Начать передвижение
    /// </summary>
    public virtual void StartWalking(orientationEnum direction)
    {
        if (!moving)
        {
            moving = true;
        }
        movingDirection = direction;
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
    /// Совершить прыжок
    /// </summary>
    public virtual void Jump()
    {
    }

    /// <summary>
    /// Функция прохода через двери
    /// </summary>
    public virtual void GoThroughTheDoor(DoorClass door)
    {
        transform.position = door.nextPosition;
    }

    /// <summary>
    /// Установить в правой руке нужное оружие
    /// </summary>
    public virtual void SetWeapon(WeaponClass _weapon)
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

    /// <summary>
    /// Задать поле статов
    /// </summary>
    public virtual void SetStats(Stats _stats)
    {
    }

}
