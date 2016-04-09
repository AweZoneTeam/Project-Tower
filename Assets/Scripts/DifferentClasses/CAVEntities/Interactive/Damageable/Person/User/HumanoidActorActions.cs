using UnityEngine;
using System.Collections;

public class HumanoidActorActions : PersonActions
{

    #region consts

    const float flipTime = 0.8f;

    const float lowObstacleTime = 0.5f;
    const float maxLowObstacleHeight = 8f;
    const float lowObstacleSpeed = 30f;

    const float highObstacleTime = 0.8f;
    const float highObstacleSpeed = 30f;
    const float maxHighObstacleHeight = 17f;

    const float highWallPosition1 = 12f;
    const float highWallPosition2 = -2.5f;
    const float frontWallPosition1 =2.5f;
    const float frontWallPosition2 = -2.5f;

    const float platformClimbTime = 0.85f;
    const float platformClimbSpeed = 18f;

    protected const float camMaxDistance = 20f;

    #endregion //consts

    #region parametres

    public float flipForce = 2000f;

	public bool touchingGround;
	public BoxCollider2D groundCol;

    #endregion //parametres

    #region fields

    public BoxCollider upperBody, lowerBody;//2 коллайдера, отвечающих за 2 половины тела персонажа

    protected Transform aboveWallCheck, lowWallCheck, frontWallCheck, highWallCheck;

    private WeaponClass mainWeapon, secondaryWeapon;//Какое оружие персонаж носит в правой руке

    protected CameraController cam = null;

    #endregion //fields

    public void OnCollisionEnter2D(Collision2D col) {
		touchingGround = true;
	}

	public void OnCollisionExit2D(Collision2D col) {
		touchingGround = false;
	}

    public void Update() {

        if (!death)
        {
            #region UsualActions

            if (envStats.interaction == interactionEnum.noInter)
            {
                if (envStats.groundness == groundnessEnum.crouch)
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
                        direction.dir = orientationEnum.left;
                    }
                    else
                    {
                        rigid.velocity = new Vector3(currentMaxSpeed, rigid.velocity.y, rigid.velocity.z);
                        direction.dir = orientationEnum.right;
                    }
                    if ((envStats.groundness == groundnessEnum.grounded) && (cAnim != null))
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
                    if ((envStats.groundness == groundnessEnum.grounded) && (rigid.velocity.x != 0))
                    {
                        rigid.drag = 0.1f;
                    }
                    if ((envStats.groundness == groundnessEnum.grounded) && (cAnim != null))
                    {
                        if (precipiceIsForward)
                        {
                            cAnim.Look(new Vector2(1f, 0f));
                        }
                        else
                        {
                            cAnim.GroundStand();
                        }
                    }
                }
                if (cAnim != null)
                {
                    if (envStats.groundness == groundnessEnum.inAir)
                    {
                        cAnim.AirMove();
                    }
                    if (envStats.groundness == groundnessEnum.crouch)
                    {
                        cAnim.CrouchMove();
                    }
                }
            }
            #endregion //UsualActions

            #region EdgeActions

            else if (envStats.interaction == interactionEnum.edge)
            {
                cAnim.Hanging(0f);
            }

            #endregion //EdgeActions

            #region SpecialMovement
            else if (envStats.interaction != interactionEnum.interactive)
            {

                #region DefineMovement

                if (employment<4)
                {
                    return;
                }
                if (envStats.interaction == interactionEnum.thicket)
                {
                    SetMaxSpeed(thicketSpeed);
                    if (cAnim != null)
                    {
                        cAnim.ThicketMove();
                    }
                }
                else if (envStats.interaction == interactionEnum.stair)
                {
                    SetMaxSpeed(stairSpeed);
                    if (cAnim != null)
                    {
                        cAnim.StairMove();
                    }
                }
                else if (envStats.interaction == interactionEnum.rope)
                {
                    SetMaxSpeed(ropeSpeed);
                    if (cAnim != null)
                    {
                        cAnim.RopeMove();
                    }
                }
                else
                {
                    SetMaxSpeed(ledgeSpeed);
                    if (cAnim != null)
                    {
                        cAnim.LedgeMove(-1f);
                    }
                }

                #endregion //DefineMovement

                #region Move

                if (moving)
                {
                    rigid.velocity = new Vector3(currentMaxSpeed * climbingDirection.x, currentMaxSpeed * climbingDirection.y, 0f);
                }
                else
                {
                    rigid.velocity = new Vector3(0f, 0f, 0f);
                }

                #endregion //Move

            }
            #endregion //SpecialMovement

        }
    }

    public override void Initialize()
    {
        base.Initialize();
        cam = GameObject.FindGameObjectWithTag(Tags.cam).GetComponent<CameraController>();
        rigid = GetComponent<Rigidbody>();
        hitBox = GetComponentInChildren<HitController>();
        hitData = null;
        currentMaxSpeed = runSpeed;
        BoxCollider[] cols = gameObject.GetComponents<BoxCollider>();
        lowerBody = cols[0];
        upperBody = cols[1];
        frontWallCheck = transform.FindChild("Indicators").FindChild("FrontWallCheck");
        highWallCheck = transform.FindChild("Indicators").FindChild("HighWallCheck");
        employment = maxEmployment;
    }

    /// <summary>
    /// Разглядеть
    /// </summary>
    public override void Observe(Vector2 sightDirection)
    {
        if (sightDirection.y > 0f)
        {
            cAnim.Look(new Vector2(0f, 1f));
        }
        else if (sightDirection.y < 0f)
        {
            cAnim.Look(new Vector2(0f, -1f));
        }
        cam.SetOffsetPosition(camMaxDistance * sightDirection);
    }

    /// <summary>
    /// Совершить поворот
    /// </summary>
	public override void Turn(orientationEnum Direction) {
        if (employment <= 4)
        {
            return;
        }
        base.Turn(Direction);
	}

    /// <summary>
    /// Начать обыкновенное перемещение
    /// </summary>
	public override void StartWalking(orientationEnum Direction)
    {
        if (employment <= 3)
        {
            return;
        }
        base.StartWalking(Direction);
	}

    /// <summary>
    /// Совершить прыжок
    /// </summary>
	public override void Jump()
    {
        if (employment <= 5)
        {
            return;
        }
        if (envStats.groundness == groundnessEnum.grounded)
        {
            rigid.velocity = new Vector3(rigid.velocity.x, Mathf.Clamp(rigid.velocity.y + jumpForce, Mathf.NegativeInfinity, jumpForce), rigid.velocity.z);
        }
        else if (envStats.groundness == groundnessEnum.crouch)
        {
            JumpDown();
        }
	}

    /// <summary>
    /// Спрыгнуть с платформы
    /// </summary>
    public override void JumpDown()
    {
        base.JumpDown();
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
            envStats.groundness = groundnessEnum.crouch;
            SetMaxSpeed(crouchSpeed);
            highWallCheck.localPosition = new Vector3(highWallCheck.localPosition.x, highWallPosition2, highWallCheck.localPosition.z);
            frontWallCheck.localPosition = new Vector3(frontWallCheck.localPosition.x, frontWallPosition2, frontWallCheck.localPosition.z);
        }
        else
        {
            if (envStats.groundness == groundnessEnum.crouch)
            {
                highWallCheck.localPosition = new Vector3(highWallCheck.localPosition.x, highWallPosition1, highWallCheck.localPosition.z);
                frontWallCheck.localPosition = new Vector3(frontWallCheck.localPosition.x, frontWallPosition1, frontWallCheck.localPosition.z);
                envStats.groundness = groundnessEnum.grounded;
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
        envStats.groundness = groundnessEnum.crouch;
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
            rigid.velocity = new Vector3((int)direction.dir * lowObstacleSpeed / 2, 5f, rigid.velocity.z);
        }
    }

    /// <summary>
    /// Обойти высокое препятствие
    /// </summary>
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
            rigid.velocity= new Vector3((int)direction.dir * highObstacleSpeed/2, 5f, rigid.velocity.z);
            rigid.useGravity = true;
        }
    }

    /// <summary>
    /// Зацепиться за (заросли, верёвку, уступ...)
    /// </summary>
    /// <param name="yes"></param>
    public override void Hang(bool yes)
    {
        if (yes)
        {
            rigid.useGravity = false;
        }
        else
        {
            rigid.useGravity = true;
            envStats.interaction = interactionEnum.noInter;
            SetMaxSpeed(runSpeed);
            moving = false;
        }
    }

    /// <summary>
    /// Начать особое перемещение
    /// </summary>
    /// <param name="_climbDirection"></param>
    public override void StartClimbing(Vector2 _climbDirection)
    {
        if (employment <= 5)
        {
            return;
        }
        climbingDirection = _climbDirection;
        if (climbingDirection != Vector2.zero)
        {
            base.StartClimbing(_climbDirection);
        }
    }

    /// <summary>
    /// Взобраться на платформу
    /// </summary>
    public override void ClimbOntoThePlatform()
    {
        if (employment == maxEmployment)
        {
            StartCoroutine(ClimbingOnPlatform(platformClimbTime, 10));
        }
        if (cAnim != null)
        {
            cAnim.LedgeMove(1f);
        }
    }

    IEnumerator ClimbingOnPlatform(float _time, int _employment)
    {
        moving = false;
        rigid.velocity = new Vector3(0f, platformClimbSpeed, rigid.velocity.z);
        employment -= _employment;
        yield return new WaitForSeconds(_time);
        employment += _employment;
        Hang(false);
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
            if (envStats.groundness == groundnessEnum.grounded)
            {
                hitData = mainWeapon.GetHit("groundHit");
                if ((cAnim != null)&&(hitData!=null))
                {
                    cAnim.Attack("Hit", hitData.hitTime);
                }
                StartCoroutine(AttackProcess(6)); 
            }
            else if (envStats.groundness == groundnessEnum.inAir)
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
        SpFunctions.ChangeRoomData(door.roomPath);
        GameObject.FindGameObjectWithTag(Tags.cam).GetComponent<CameraController>().ChangeRoom();
        base.GoThroughTheDoor(door);
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
    public override void SetStats(EnvironmentStats _stats)
    {
        envStats = _stats;
    }

    /// <summary>
    /// Установить нужное оружие
    /// </summary>
    public override void SetWeapon(WeaponClass _weapon, string weaponType)
    {
        if (string.Equals("Main", weaponType))
        {
            mainWeapon = _weapon;
        }
        else if (string.Equals("Secondary", weaponType))
        {
            secondaryWeapon = _weapon;
        }
        else if (string.Equals("TwoHanded", weaponType))
        {
            mainWeapon = _weapon;
            secondaryWeapon = _weapon;
        }
    }
}

