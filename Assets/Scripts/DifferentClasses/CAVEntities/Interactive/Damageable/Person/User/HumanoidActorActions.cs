using UnityEngine;
using System.Collections;

public class HumanoidActorActions : PersonActions
{

    #region consts

    const float flipTime = 0.8f;
    const float lowObstacleTime = 0.5f;
    const float highObstacleTime = 0.8f;

    const float lowObstacleSpeed = 30f;
    const float highObstacleSpeed = 30f;

    const float maxLowObstacleHeight = 8f;
    const float maxHighObstacleHeight = 17f;

    const float highWallPosition1 = 12f;
    const float highWallPosition2 = -2.5f;
    const float frontWallPosition1 =2.5f;
    const float frontWallPosition2 = -2.5f;

    #endregion //consts

    #region parametres

    public float crouchingSpeed = 25f;

    public float flipForce = 2000f;

	public bool touchingGround;
	public BoxCollider2D groundCol;

    #endregion //parametres

    #region fields
    private Stats stats;//Параметры персонажа

    public BoxCollider upperBody, lowerBody;//2 коллайдера, отвечающих за 2 половины тела персонажа

    protected Transform aboveWallCheck, lowWallCheck, frontWallCheck, highWallCheck;

    private WeaponClass mainWeapon;//Какое оружие персонаж носит в правой руке

    public int k1=0;

    #endregion //fields

    public void OnCollisionEnter2D(Collision2D col) {
		touchingGround = true;
	}

	public void OnCollisionExit2D(Collision2D col) {
		touchingGround = false;
	}

    /// <summary>
    /// Инициализация полей и переменных
    /// </summary>
    public override void Awake ()
    {
        base.Awake();
	}

    public void Update() {

        if (!death)
        {
            #region UsualActions

            if (stats.interaction == interactionEnum.noInter)
            {
                if (stats.groundness == groundnessEnum.crouch)
                {
                    upperBody.isTrigger = true;
                }
                else
                {
                    upperBody.isTrigger = false;
                }
                if (moving)
                {
                    if (movingDirection == orientationEnum.left)
                    {
                        rigid.velocity = new Vector3(-currentMaxSpeed, rigid.velocity.y, rigid.velocity.z);
                        stats.direction = orientationEnum.left;
                    }
                    else
                    {
                        rigid.velocity = new Vector3(currentMaxSpeed, rigid.velocity.y, rigid.velocity.z);
                        stats.direction = orientationEnum.right;
                    }
                    if ((stats.groundness == groundnessEnum.grounded) && (cAnim != null))
                    {
                        if (currentMaxSpeed == fastRunSpeed)
                        {
                            cAnim.FastGroundMove();
                        }
                        else
                        {
                            cAnim.GroundMove();
                        }
                    }
                }
                else
                {
                    if ((stats.groundness == groundnessEnum.grounded) && (rigid.velocity.x != 0))
                    {
                        rigid.drag = 0.1f;
                    }
                    if ((stats.groundness == groundnessEnum.grounded) && (cAnim != null))
                    {
                        cAnim.GroundStand();
                    }
                }
                if (cAnim != null)
                {
                    if (stats.groundness == groundnessEnum.inAir)
                    {
                        cAnim.AirMove();
                    }
                    if (stats.groundness == groundnessEnum.crouch)
                    {
                        cAnim.CrouchMove();
                    }
                }
            }
            #endregion //UsualActions

            #region EdgeActions

            if (stats.interaction ==interactionEnum.edge)
            {
                cAnim.Hanging(0f);
            }

            #endregion //EdgeActions

        }
    }

    public override void Initialize()
    {
        cAnim = transform.FindChild("Body").gameObject.GetComponent<CharacterVisual>();
        rigid = GetComponent<Rigidbody>();
        hitBox = GetComponentInChildren<HitController>();
        hitData = null;
        currentMaxSpeed = maxSpeed;
        BoxCollider[] cols = gameObject.GetComponents<BoxCollider>();
        lowerBody = cols[0];
        upperBody = cols[1];
        frontWallCheck = transform.FindChild("Indicators").FindChild("FrontWallCheck");
        highWallCheck = transform.FindChild("Indicators").FindChild("HighWallCheck");
        employment = maxEmployment;
    }

	public override void Turn(orientationEnum Direction) {
        if (employment <= 4)
        {
            return;
        }
        base.Turn(Direction);
	}

	public override void StartWalking(orientationEnum Direction)
    {
        if (employment <= 3)
        {
            return;
        }
        base.StartWalking(Direction);
	}

	public override void Jump()
    {
        if (employment <= 5)
        {
            return;
        }
		if (stats.groundness==groundnessEnum.grounded)
        {
                rigid.velocity =new Vector3(rigid.velocity.x, Mathf.Clamp(rigid.velocity.y+jumpForce,Mathf.NegativeInfinity,jumpForce), rigid.velocity.z);
		}
	}

    /// <summary>
    /// Присесть (или выйти из состояния приседа)
    /// </summary>
    public override void Crouch(bool yes)
    {
        if (employment <= 4)
        {
            return;
        }
        if (yes)
        {
            stats.groundness = groundnessEnum.crouch;
            SetMaxSpeed(crouchingSpeed);
            highWallCheck.localPosition = new Vector3(highWallCheck.localPosition.x, highWallPosition2, highWallCheck.localPosition.z);
            frontWallCheck.localPosition = new Vector3(frontWallCheck.localPosition.x, frontWallPosition2, frontWallCheck.localPosition.z);
        }
        else
        {
            if (stats.groundness == groundnessEnum.crouch)
            {
                highWallCheck.localPosition = new Vector3(highWallCheck.localPosition.x, highWallPosition1, highWallCheck.localPosition.z);
                frontWallCheck.localPosition = new Vector3(frontWallCheck.localPosition.x, frontWallPosition1, frontWallCheck.localPosition.z);
                stats.groundness = groundnessEnum.grounded;
            }
        }
    }

    /// <summary>
    /// Совершить кувырок
    /// </summary>
    public override void Flip()
    {
        if (employment <= 4)
        {
            return;
        }
        moving = false;
        stats.groundness = groundnessEnum.crouch;
        rigid.velocity = new Vector3(flipForce * (int)movingDirection, rigid.velocity.y, rigid.velocity.z);
        if (cAnim != null)
        {
            cAnim.Flip();
        }
        Crouch(true);
        StartCoroutine(ActionRoutine(flipTime, 10));
    }

    /// <summary>
    /// Произвести взаимодействие со стеной
    /// </summary>
    public override void WallInteraction()
    {
        if (employment <= 4)
        {
            return;
        }
        if (cAnim != null)
        {
            cAnim.WallInteraction(0f);
        }
    }

    /// <summary>
    /// Зацепиться за высокое препятствие
    /// </summary>
    public override void HangHighObstacle()
    {
        rigid.useGravity = false;
        rigid.velocity = new Vector3(rigid.velocity.x,0f,rigid.velocity.z);
        if (cAnim != null)
        {
            cAnim.Hanging(0f);
        }
    }

    /// <summary>
    /// Обойти низкое препятствие
    /// </summary>
    public override void AvoidLowObstacle(float height)
    {
        if (employment == maxEmployment)
        {
            StartCoroutine(ActionRoutine(lowObstacleTime*height/maxLowObstacleHeight, 10));
        }
        if (height > 0f)
        {
            rigid.useGravity = false;
            rigid.velocity = new Vector3(0f, lowObstacleSpeed, rigid.velocity.z);
            if (cAnim != null)
            {
                cAnim.WallInteraction(0.3f);
            }
        }
        if (height == 0f)
        {
            rigid.useGravity = true;
            rigid.velocity = new Vector3((int)stats.direction * lowObstacleSpeed / 2, 5f, rigid.velocity.z);
        }
    }

    public override void AvoidHighObstacle(float height)
    {
        if (employment == maxEmployment)
        {
            StartCoroutine(ActionRoutine(highObstacleTime*height/maxHighObstacleHeight, 10));
        }
        if (height > 0f)
        {
            rigid.velocity = new Vector3(0f, highObstacleSpeed, rigid.velocity.z);
            if (cAnim != null)
            {
                cAnim.Hanging(1f);
            }
            rigid.useGravity = false;
        }
        if (height==0f)
        {
            rigid.velocity= new Vector3((int)stats.direction*highObstacleSpeed/2, 5f, rigid.velocity.z);
            rigid.useGravity = true;
        }
    }

    IEnumerator ActionRoutine(float _time, int _employment)
    {
        employment -= _employment;
        yield return new WaitForSeconds(_time);
        employment += _employment;
    }

    /// <summary>
    /// Учёт ситуации и произведение нужной в данный момент атаки
    /// </summary>
    public override void Attack()
    {
        if (employment <= 4)
        {
            return;
        } 
        if ((hitData == null)&&(mainWeapon!=null))
        {
            if (stats.groundness == groundnessEnum.grounded)
            {
                hitData = mainWeapon.GetHit("groundHit");
                if ((cAnim != null)&&(hitData!=null))
                {
                    cAnim.Attack("Hit", hitData.hitTime);
                }
                StartCoroutine(AttackProcess(6)); 
            }
            else if (stats.groundness == groundnessEnum.inAir)
            {
                hitData = mainWeapon.GetHit("airHit");
                if ((cAnim != null)&&(hitData!=null))
                {
                    cAnim.Attack("Hit", hitData.hitTime);
                }
                StartCoroutine(AttackProcess(6));
            }
        }
    }

    protected virtual IEnumerator AttackProcess(int _employment)//Процесс атаки
    {
        if (hitBox != null)
        {
            GameObject hBox = hitBox.gameObject;
            hBox.transform.localPosition = hitData.hitPosition;
            hBox.GetComponent<BoxCollider>().size = hitData.hitSize;
            employment -= _employment;
            yield return new WaitForSeconds(hitData.hitTime-hitData.beginTime);
            this.hitBox.SetHitBox(hitData.beginTime - hitData.endTime, hitData);
            yield return new WaitForSeconds(hitData.beginTime);
            employment += _employment;
            hitData = null;
        }
    }

    public override void GoThroughTheDoor(DoorClass door)
    {
        base.GoThroughTheDoor(door);
        SpFunctions.ChangeRoomData(door.roomPath);
        GameObject.FindGameObjectWithTag(Tags.cam).GetComponent<CameraController>().ChangeRoom();
    }

    public override void Death()
    {
        Drop();
        death = true;
        if (cAnim != null)
        {
            cAnim.Death();
        }
    }

    /// <summary>
    /// Задать поле статов
    /// </summary>
    public override void SetStats(Stats _stats)
    {
        stats = _stats;
    }

    /// <summary>
    /// Установить в правой руке нужное оружие
    /// </summary>
    public override void SetWeapon(WeaponClass _weapon)
    {
        mainWeapon = _weapon;
    }
}

