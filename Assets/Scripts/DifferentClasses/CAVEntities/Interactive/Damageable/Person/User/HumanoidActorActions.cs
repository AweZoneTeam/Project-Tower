using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HumanoidActorActions : PersonActions
{

    #region consts

    const float flipTime = 0.8f;

    const float lowObstacleTime = 0.5f;
    const float maxLowObstacleHeight = 8f;
    const float lowObstacleSpeed = 30f;

    const float highObstacleTime = .7f;
    const float crouchTime = 2f;
    const float highObstacleSpeed = 30f;
    const float maxHighObstacleHeight = 17f;

    const float highWallPosition1 = 12f;
    const float highWallPosition2 = -2.5f;
    const float frontWallPosition1 =2.5f;
    const float frontWallPosition2 = -2.5f;

    const float platformClimbTime = 0.85f;
    const float platformClimbSpeed = 18f;

    protected const float camMaxDistance = 40f;

    #endregion //consts

    #region parametres

    public float flipForce = 2000f;

	public bool touchingGround;
	public BoxCollider2D groundCol;

    public bool canCharge;
    public float chargeValue;

    #endregion //parametres

    #region fields

    public Collider upperBody, lowerBody;//2 коллайдера, отвечающих за 2 половины тела персонажа

    protected Transform aboveWallCheck, lowWallCheck, frontWallCheck, highWallCheck;

    public WeaponClass mainWeapon, secondaryWeapon;//Какое оружие персонаж носит в правой руке

    protected CameraController cam = null;

    #endregion //fields

    #region queue//очередь инпутов

    public ActionQueue actionQueue = new ActionQueue();

    void NextAction()
    {
        if (!canCharge && chargeValue == 0f && !Input.GetButton("Horizontal"))
        {
            if (actionQueue.turn != null)
            {
                Turn(actionQueue.turn);
                actionQueue.turn = null;
            }
            if (actionQueue.startWalk != null)
            {
                StartWalking(actionQueue.startWalk);
                actionQueue.startWalk = null;
            }
            if (actionQueue.next != null)
            {
                actionBase[actionQueue.next.func](actionQueue.next);
                actionQueue.next = null;
            }
        }
        actionQueue.Clear();
    }

    #endregion//queue

    #region FuncDictionary

    protected delegate void PAction(ActionClass a);
    protected delegate bool PCondition(ActionClass a);
    protected Dictionary<string, PCondition> conditionBase;
    protected Dictionary<string, PAction> actionBase;
    public virtual void FuncDictionry()
    {
        conditionBase = new Dictionary<string, PCondition>();

        actionBase = new Dictionary<string, PAction>();
        actionBase.Add("Turn", Turn);
        actionBase.Add("Attack", Attack);
        actionBase.Add("StartCharge", StartCharge);
        actionBase.Add("Flip", Flip);
        actionBase.Add("Jump", Jump);

    }

    #endregion//FuncDictionary

    #region fightingMode

    string currentCombo = "";
    int nCombo = 0;
    float comboTime;

    bool _isFightMode;
    public bool isFightMode
    {
        get { return _isFightMode; }
        set
        {
            if (value)
            {
                _fightingModeTime = fightingModeTime;
                if (!_isFightMode)
                {
                    //достать оружие
                }
            }
            if (!value && _isFightMode)
            {
                _fightingModeTime = 0;
                //убрать оружие
            }
            _isFightMode = value;
        }
    }
    public float fightingModeTime;
    float _fightingModeTime;

    public override void Hitted()
    {
        base.Hitted();
        isFightMode = true;
    }

    #endregion//fightingMode

    public void OnCollisionEnter2D(Collision2D col)
    {
		touchingGround = true;
	}

	public void OnCollisionExit2D(Collision2D col) {
		touchingGround = false;
	}

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_fightingModeTime > 0)
            _fightingModeTime -= Time.deltaTime;
        else
            if (isFightMode) isFightMode = false;
        if (comboTime > 0)
            comboTime -= Time.deltaTime;
        else
        {
            nCombo = 0;
            currentCombo = "";
        }
    }

    public void Update()
    {

        if (!death)
        {

            #region UsualActions

            if (envStats.interaction == interactionEnum.noInter)
            {
                if ((envStats.groundness == groundnessEnum.crouch)||(!aboveWallCheck.gameObject.activeSelf)||(wallIsAbove))
                {
                    upperBody.isTrigger = true;
                }
                else
                {
                    upperBody.isTrigger = false;
                }
                if (moving)
                {
                    Move(movingDirection, currentMaxSpeed);
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

                if (employment < 4)
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
                else if (envStats.interaction == interactionEnum.ladder)
                {
                    SetMaxSpeed(ladderSpeed);
                    if (cAnim != null)
                    {
                        cAnim.LadderMove();
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

            #region actionWithInteractiveObject

            else if (envStats.interaction==interactionEnum.interactive)
            {
                if (interactionObject is MoveableBoxActions)
                {
                    SetMaxSpeed(boxSpeed);
                    if (moving)
                    {
                        Move(movingDirection, boxSpeed);
                        if (cAnim != null)
                        {
                            cAnim.BoxMove();
                        }
                    }
                }
            }

            #endregion //actionWithInteractiveObject

        }
    }

    public override void Initialize()
    {
        base.Initialize();
        FuncDictionry();
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
        aboveWallCheck = transform.FindChild("Indicators").FindChild("AboveWallCheck");
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
            cam.SetOffsetPosition(camMaxDistance / 2 * sightDirection);
        }
        else if (sightDirection.y < 0f)
        {
            cAnim.Look(new Vector2(0f, -1f));
            cam.SetOffsetPosition(camMaxDistance * sightDirection);
        }
        else
        {
            cam.SetOffsetPosition(camMaxDistance * sightDirection);
        }
    }

    /// <summary>
    /// Совершить поворот
    /// </summary>
	public override void Turn(ActionClass a)
    {
        if (employment <= 4)
        {
            if (!canCharge)
            {
                a.func = "Turn";
                actionQueue.AddToQueue(a);
            }
            return;
        }
        base.Turn(a);
    }

    /// <summary>
    /// Начать обыкновенное перемещение
    /// </summary>
	public override void StartWalking(ActionClass a)
    {
        if (employment <= 4)
        {
            if (!canCharge)
            {
                a.func = "StartWalking";
                actionQueue.AddToQueue(a);
            }
            return;
        }
        base.StartWalking(a);
    }

    /// <summary>
    /// Совершить прыжок
    /// </summary>
	public override void Jump(ActionClass a)
    {
        if (employment <= 5)
        {
            if (!canCharge)
            {
                a = new ActionClass();
                a.func = "Jump";
                actionQueue.AddToQueue(a);
            }
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
    public override void JumpDown(ActionClass a)
    {
        base.JumpDown();
    }

    /// <summary>
    /// Присесть (или выйти из состояния приседа)
    /// </summary>
    public override void Crouch(bool yes)
    {
        if (yes)
        {
            if (employment <= 4)
            {
                return;
            }
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
    public override void Flip(ActionClass a)
    {
        if (employment <= 4)
        {
            if (!canCharge)
            {
                if (a == null)
                    a = new ActionClass();
                a.func = "Flip";
                actionQueue.AddToQueue(a);
            }
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
        SwitchCollider(false);
        rigid.velocity = new Vector3(0f,0f,rigid.velocity.z);
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
            envStats.interaction = interactionEnum.edge;
            rigid.velocity = new Vector3(0f, highObstacleSpeed, rigid.velocity.z);
            SwitchCollider(false);
            if (cAnim != null)
            {
                cAnim.Hanging(1f);
            }
            rigid.useGravity = false;
            StartCoroutine(AfterClimbProcess(highObstacleTime*height/20f));
        }
    }

    protected virtual IEnumerator AfterClimbProcess(float _climbTime)
    {
        yield return new WaitForSeconds(_climbTime);
        rigid.velocity = new Vector3((int)direction.dir * highObstacleSpeed, 5f, rigid.velocity.z);
        rigid.useGravity = true;
        envStats.interaction = interactionEnum.noInter;
        SwitchCollider(true);
        upperBody.isTrigger = true;
        aboveWallCheck.gameObject.SetActive(false);
        yield return new WaitForSeconds(crouchTime);
        aboveWallCheck.gameObject.SetActive(true);
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
            SwitchCollider(true);
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
	/// Оповещает о том, что можно начинать накапливать чардж атаку
	/// </summary>
	public void StartCharge(ActionClass action)
    {
        isFightMode = true;
        if (action is AttackClass)
        {
            if (employment < 10)
            {
                return;
            }
            employment = 0;
        }
        else if (action is ShieldClass)
        {
            if (employment < 10)
            {
                return;
            }
            employment = 0;
            orgStats.addDefence = ((ShieldClass)action).addDefence;
        }
        else if (action is BowClass)
        {
            if (employment < 10)
            {
                return;
            }
            employment = 0;
            chargeValue = -100f;
            GameObject.Find("Aim").GetComponent<MeshRenderer>().enabled = true;
        }
        canCharge = true;
    }

    /// <summary>
    /// Оповещает о том, что можно высвободить силу богов
    /// </summary>
    public void EndCharge(ActionClass action)
    {
        isFightMode = true;
        if (action is AttackClass)
        {
            AttackClass a = (AttackClass)action;
            if (canCharge)
            {
                canCharge = false;
                employment = 10;
                if (chargeValue >= a.chargeAttack.finalTime)
                {
                    hitData = a.chargeAttack.hitData;
                    StartCoroutine(AttackProcess(6));
                }
                chargeValue = 0f;
            }
            else
            {
                chargeValue = 0f;
                actionQueue.next = null;
                Attack(a);
            }
        }
        else if (action is ShieldClass)
        {
            if (canCharge)
            {
                canCharge = false;
                employment = 10;
                orgStats.addDefence = null;
            }
        }
        else if (action is BowClass)
        {
            if (canCharge)
            {
                GameObject.Find("Aim").GetComponent<MeshRenderer>().enabled = false;
                GameObject arrow = Instantiate(((BowClass)action).arrow);
                Debug.Log(transform.position + Vector3.up * 2.5f);
                arrow.transform.position = transform.position + Vector3.up * 2.5f;
                arrow.GetComponent<HitController>().hitData = ((BowClass)action).hitData;
                Vector3 force = new Vector3();
                force.x = ((BowClass)action).shotForce * Mathf.Cos((chargeValue + 100) * Mathf.PI / 180f);
                force.y = ((BowClass)action).shotForce * Mathf.Sin((chargeValue + 100) * Mathf.PI / 180f);
                if (movingDirection == orientationEnum.left) force.x = force.x * -1f;
                arrow.GetComponent<Rigidbody>().AddForce(force);
                chargeValue = 0f;
                canCharge = false;
                employment = 10;
            }
        }
    }

    /// <summary>
    /// Учёт ситуации и произведение нужной в данный момент атаки
    /// </summary>
    public override void Attack(ActionClass action)
    {
        if (employment <= 4)
        {

            if (!canCharge)
            {
                action.func = "Attack";
                actionQueue.AddToQueue(action);
            }
            return;
        }
        if (action is AttackClass)
        {
            if (envStats.groundness != ((AttackClass)action).groundnessState)
            {
                return;
            }
            ComboClass combo = ((AttackClass)action).combo;
            hitData = null;
            if (mainWeapon != null)
            {
                if ((cAnim != null) && (hitData == null))
                {
                    if (combo.comboName != currentCombo)
                    {
                        currentCombo = combo.comboName;
                        if (!combo.preCombo.Contains(currentCombo))
                            nCombo = 0;
                    }
                    isFightMode = true;
                    hitData = combo.hitData[nCombo];
                    nCombo++;
                    if (nCombo >= combo.hitData.Count)
                    {
                        nCombo = 0;
                        currentCombo = "";
                    }
                    else
                    {
                        comboTime = hitData.hitTime;
                    }
                }
                if (hitData != null)
                {
                    StartCoroutine(AttackProcess(6));
                }
            }
        }
        if (action is BowClass)
        {
            StartCoroutine(ShotProcess(6, action));
        }
    }

    protected virtual IEnumerator ShotProcess(int _employment, ActionClass act)
    {
        employment -= _employment;
        yield return new WaitForSeconds(0.3f);
        GameObject.Find("Aim").GetComponent<MeshRenderer>().enabled = false;
        GameObject arrow = Instantiate(((BowClass)act).arrow);
        Debug.Log(transform.position + Vector3.up * 2.5f);
        arrow.transform.position = transform.position + Vector3.up * 2.5f;
        arrow.GetComponent<HitController>().hitData = ((BowClass)act).hitData;
        Vector3 force = new Vector3();
        force.x = ((BowClass)act).shotForce;
        if (movingDirection == orientationEnum.left) force.x = force.x * -1f;
        arrow.GetComponent<Rigidbody>().AddForce(force);
        chargeValue = 0f;
        canCharge = false;
        employment += _employment;
        NextAction();
    }

    protected virtual IEnumerator AttackProcess(int _employment)//Процесс атаки
    {
        if (hitBox != null)
        {
            GameObject hBox = hitBox.gameObject;
            hBox.transform.localPosition = hitData.hitPosition;
            hBox.GetComponent<BoxCollider>().size = hitData.hitSize;
            employment -= _employment;
            StopWalking();
            if (hitData.hitForce.magnitude > 0)
            {
                rigid.velocity = Vector3.zero;
            }
            yield return new WaitForSeconds(hitData.hitTime - hitData.beginTime);
            if (movingDirection == orientationEnum.right)
            {
                rigid.AddForce(hitData.hitForce);
            }
            else if (movingDirection == orientationEnum.left)
            {
                rigid.AddForce(new Vector3(-hitData.hitForce.x, hitData.hitForce.y, hitData.hitForce.z));
            }
            this.hitBox.SetHitBox(hitData.beginTime - hitData.endTime, hitData);
            yield return new WaitForSeconds(hitData.beginTime - hitData.comboTime);
            employment += _employment;
            hitData = null;
            NextAction();
        }
    }

    public override void UseItem(ItemBunch itemBunch)
    {
        UsableItemClass item = (UsableItemClass)itemBunch.item;
        if ((employment <= 4))
        {
            return;
        }
        foreach (ItemActionData _itemAction in item.itemActions)
        {
            if (item != null ? (_itemAction.grounded ? envStats.groundness == groundnessEnum.grounded : (_itemAction.crouching? envStats.groundness == groundnessEnum.crouch : envStats.groundness != groundnessEnum.crouch)) : false)
            {
                if (itemActionList.ContainsKey(_itemAction.actionName))
                {
                    StartCoroutine(ItemProcess(_itemAction, 0f));
                }
                if (_itemAction.consumed)
                {
                    itemBunch.quantity--;
                }
                break;
            }
        }
    }

    /// <summary>
    /// Задержать действие, совершаемое при использовании предмета
    /// </summary>
    public override void ChargeItem(ItemBunch itemBunch)
    {
        UsableItemClass item = (UsableItemClass)itemBunch.item;
        if (item.charge ? ((employment < 5) && (chargeValue == 0)) : true)
        {
            return;
        }
        if (item.grounded ? envStats.groundness == groundnessEnum.grounded : (envStats.groundness != groundnessEnum.crouch))
        {
            ChargeData chargeData = item.chargeData;
            if (chargeValue == 0)
            {
                employment -= chargeData.employment;
            }
            if (chargeValue < chargeData.maxValue)
            {
                chargeValue += Time.deltaTime;
            }
            else if (chargeValue > chargeData.maxValue)
            {
                chargeValue = chargeData.maxValue;
            }
        }
    }

    /// <summary>
    /// Совершить действие, происходящее при прекращении зарядки
    /// </summary>
    public override void ReleaseItem(ItemBunch itemBunch)
    {
        UsableItemClass item = (UsableItemClass)itemBunch.item;
        if (!item.charge)
        {
            return;
        }
        ItemChargeData chargeAction = item.chargeData;
        employment += chargeAction.employment;
        if (chargeValue < chargeAction.deadZone)
        {
            if (item.itemActions.Count > 0 ? itemActionList.ContainsKey(item.itemActions[0].actionName) : false)
            {
                StartCoroutine(ItemProcess(item.itemActions[0], 0f));
            }
            if (item.itemActions.Count>0? item.itemActions[0].consumed:false)
            {
                itemBunch.quantity--;
            }
        }
        else if (chargeValue < chargeAction.maxValue)
        {
            if (itemActionList.ContainsKey(chargeAction.unchargedAction.actionName))
            {
                StartCoroutine(ItemProcess((ItemActionData)chargeAction.unchargedAction, chargeValue));
            }
            ItemActionData itemData = (ItemActionData)chargeAction.unchargedAction;
            if (itemData.consumed)
            {
                itemBunch.quantity--;
            }
        }
        else
        {
            if (itemActionList.ContainsKey(chargeAction.chargedAction.actionName))
            {
                StartCoroutine(ItemProcess((ItemActionData)chargeAction.chargedAction, chargeValue));
            }
            ItemActionData itemData = (ItemActionData)chargeAction.chargedAction;
            if (itemData.consumed)
            {
                itemBunch.quantity--;
            }
        }
        chargeValue = -1f;
    }

    protected override IEnumerator ItemProcess(ItemActionData itemData, float _chargeValue)
    {
        employment -= itemData.employment;
        itemActionList[itemData.actionName](this, itemData, chargeValue);
        yield return new WaitForSeconds(itemData.actionTime);
        employment += itemData.employment;
    }

    public override void GoThroughTheDoor(DoorClass door)
    {
        GameObject.FindGameObjectWithTag(Tags.cam).GetComponent<CameraController>().ChangeRoom(GameStatistics.currentArea, door.roomPath);
        SpFunctions.ChangeRoomData(door.roomPath);
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

