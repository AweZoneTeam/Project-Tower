using UnityEngine;
using System.Collections;

public class EnemyActions : PersonActions
{
    #region epsilons
    const float velEps = 1f;
    #endregion //epsilons

    #region parametres

    protected bool jumped;

    #endregion //parametres

    #region fields
    private Stats stats;

    public WeaponClass weapon;//Это поле соддержит данные по атакам персонажа

    #endregion //fields

    public override void Awake()
    {
        base.Awake();
    }

    public void Update()
    {
        if (!death)
        {
            if (cAnim != null)
            {
                if (stats.groundness == groundnessEnum.inAir)
                {
                    cAnim.AirMove();
                }
                else if (stats.groundness == groundnessEnum.grounded)
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
        }
    }

    public override void Initialize()
    {
        rigid = GetComponent<Rigidbody>();
        hitBox = GetComponentInChildren<HitController>();
        cAnim = GetComponentInChildren<EnemyVisual>();
    }

    #region interface

    /// <summary>
    /// Функция преследования цели
    /// </summary>
    public override void Pursue()
    {
        if (target.position.x < transform.position.x)
        {
            Turn(orientationEnum.left);
        }
        else if (target.position.x > transform.position.x)
        {
            Turn(orientationEnum.right);
        }
        if (!precipiceIsForward)
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
        if (precipiceIsForward)
        {
            if (jumpIsPossible)
            {
                if ((stats.groundness == groundnessEnum.grounded) && (!jumped))
                {
                    Jump();
                }
            }
            else
            {
                StopWalking();
            }
        }
    }

    /// <summary>
    /// Идти в указанном направлении
    /// </summary>
    public override void StartWalking(orientationEnum direction)
    {
        base.StartWalking(direction);
        stats.direction = direction;
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

    /// <summary>
    /// Остановить передвижение
    /// </summary>
    public override void StopWalking()
    {
        base.StopWalking();
        Vector3 targetVelocity = new Vector3(0f, rigid.velocity.y, rigid.velocity.z);
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
    /// Совершить прыжок
    /// </summary>
    public override void Jump()
    {
        rigid.velocity = new Vector3(rigid.velocity.x, Mathf.Clamp(rigid.velocity.y + jumpForce, Mathf.NegativeInfinity, jumpForce), rigid.velocity.z);
        StartCoroutine(JumpProcess());
    }

    protected IEnumerator JumpProcess()//Процесс прыжка
    {
        jumped = true;
        yield return new WaitForSeconds(0.1f);
        jumped = false;
    }
    #endregion //interface

    public override void Attack()
    {
        if ((hitData != null) && (weapon != null))
        {
            if (cAnim != null)
            {
                cAnim.Attack(hitData.hitName, hitData.hitTime);
            }
            StartCoroutine(AttackProcess());
        }
    }

    protected override IEnumerator AttackProcess()//Процесс атаки
    {
        if (hitBox != null)
        {
            GameObject hBox = hitBox.gameObject;
            hBox.transform.localPosition = hitData.hitPosition;
            hBox.GetComponent<BoxCollider>().size = hitData.hitSize;
            yield return new WaitForSeconds(hitData.hitTime - hitData.beginTime);
            this.hitBox.SetHitBox(hitData.beginTime - hitData.endTime, hitData);
            yield return new WaitForSeconds(hitData.beginTime);
            hitData = null;
        }
    }

    public override void SetHitData(string hitName)
    {
        for (int i = 0; i < weapon.hitData.Count; i++)
        {
            if (string.Equals(hitName, weapon.hitData[i].hitName))
            {
                hitData = weapon.hitData[i];
            }
        }
    }

    /// <summary>
    /// Задать поле статов
    /// </summary>
    public override void SetStats(Stats _stats)
    {
        stats = _stats;
    }

}
