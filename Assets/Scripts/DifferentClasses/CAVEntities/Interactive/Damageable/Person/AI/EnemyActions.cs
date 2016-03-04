using UnityEngine;
using System.Collections;

public class EnemyActions : PersonActions
{
    #region epsilons
    const float velEps = 1f;
    #endregion //epsilons

    #region parametres
    public float maxSpeed=30f;
    public float acceleration = 1f;
    public float jumpForce = 4000f;
    #endregion //parametres

    #region fields
    private Rigidbody rigid;
    private EnemyVisual cAnim;
    private Stats stats;

    public WeaponClass weapon;//Это поле соддержит данные по атакам персонажа
    protected HitController hitBox;//хитбокс оружия персонажа
    protected HitClass hitData = null;//Какими параметрами атаки персонаж пользуется в данный момент

    public Transform target;
    #endregion //fields

    public override void Awake()
    {
        base.Awake();
    }

    public void Update()
    {
        if (cAnim != null)
        {
            if (moving)
            {
                cAnim.GroundMove();
            }
            else
            {
                cAnim.GroundStand();
            }
        }
    }

    public override void Initialize()
    {
        orientation = orientationEnum.right;
        rigid = GetComponent<Rigidbody>();
        hitBox = GetComponentInChildren<HitController>();
        cAnim = GetComponentInChildren<EnemyVisual>();
    }

    #region interface
    /// <summary>
    /// Функция преследования противника
    /// </summary>
    public virtual void Pursue()
    {
        if (target.position.x < transform.position.x)
        {
            StartWalking(orientationEnum.left);
        }
        else if (target.position.x > transform.position.x)
        {
            StartWalking(orientationEnum.right);
        }
    }

    /// <summary>
    /// Идти в указанном направлении
    /// </summary>
    public override void StartWalking(orientationEnum direction)
    {
        base.StartWalking(direction);
        Turn(direction);
        Vector3 targetVelocity=new Vector3(0f,0f,0f);
        if (direction == orientationEnum.left)
        {
            targetVelocity = new Vector3(-maxSpeed, rigid.velocity.y, rigid.velocity.z);
        }
        else if(direction == orientationEnum.right)
        {
            targetVelocity = new Vector3(maxSpeed, rigid.velocity.y, rigid.velocity.z);
        }
        if (Vector3.Distance(rigid.velocity, targetVelocity) < velEps)
        {
            rigid.velocity = targetVelocity;
        }
        else
        {
            rigid.velocity = Vector3.Lerp(rigid.velocity, targetVelocity, acceleration*Time.fixedDeltaTime);
        }
    }

    #endregion //interface

    /// <summary>
    /// Задать поле статов
    /// </summary>
    public override void SetStats(Stats _stats)
    {
        stats = _stats;
    }

}
