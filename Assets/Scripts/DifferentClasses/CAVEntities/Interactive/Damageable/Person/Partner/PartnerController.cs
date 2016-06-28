using UnityEngine;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class PartnerController : PersonController
{

    #region consts

    private const float doorDistance = 4.5f;

    private const float obstacleRadius = 2f;

    protected const int fastRunMaxCount = 2;
    protected const float fastRunInputTime = 0.3f;

    protected float observeDistance = 1.5f;

    protected const float minLedgeSpeed = -90f;

    #endregion //consts

    #region dictionaries

    /// <summary>
    /// Типовые сообщения о возможности прекратить взаимодействие с интерактивным объектом
    /// </summary>
    protected static Dictionary<interactionEnum, string> interuptionMessages = new Dictionary<interactionEnum, string> { { interactionEnum.edge, "Отпустить обрыв" },
                                                                                                                  { interactionEnum.ladder, "Отпустить лестницу"},
                                                                                                                  { interactionEnum.ledge, "Отпустить выступ"},
                                                                                                                  { interactionEnum.platform, "Отпустить платформу"},
                                                                                                                  { interactionEnum.rope, "Отпустить верёвку"},
                                                                                                                  { interactionEnum.thicket, "Отпустить заросли" },
                                                                                                                  { interactionEnum.mount, "Спешиться"} };
    /// <summary>
    /// Типовые сообщения о возможности открыть дверь
    /// </summary>
    protected static Dictionary<doorEnum, string> doorOpenMessages = new Dictionary<doorEnum, string> { { doorEnum.back, "Открыть дверь на заднем плане" },
                                                                                                               { doorEnum.forward, "Открыть дверь на переднем плане" },
                                                                                                               { doorEnum.left, "Открыть дверь слева" },
                                                                                                               { doorEnum.right, "Открыть дверь справа"} };
    /// <summary>
    /// Типовые сообщения о возможности пройти через дверь
    /// </summary>
    protected static Dictionary<doorEnum, string> doorGoThroughMessages = new Dictionary<doorEnum, string> { { doorEnum.back, "Открыть дверь на заднем плане" },
                                                                                                               { doorEnum.forward, "Открыть дверь на переднем плане" },
                                                                                                               { doorEnum.left, "Открыть дверь слева" },
                                                                                                               { doorEnum.right, "Открыть дверь справа"} };


    #endregion //dictionaries

    #region enums

    protected enum directionClaveEnum { leftHold = -2, leftDown = -1, notPressed = 0, rightDown = 1, rightHold = 2 };

    #endregion //enums

    #region parametres

    protected directionClaveEnum dirClave = directionClaveEnum.notPressed;

    //текущая выполняемая атака
    AttackClass attack = new AttackClass();

    //использует ли персонаж маунта
    bool mounted;

    //окно инвентаря
    public EquipmentWindow equipWindow;

    public int fastRunInputCount = 0;
    protected float fastRunInputTimer = 0f;

    HumanoidActorActions HAA;
    public WeaponClass defaultWeapon1, defaultWeapon2;

    public string partnerName;

    protected List<string> dependedPartTypes = new List<string> { "Sword", "Shield" };
    protected Dictionary<string, string> partDependencies = new Dictionary<string, string> { { "Sword", "RightHand" }, { "Shield", "LeftHand" } };

    #endregion //parametres

    #region fields

    [SerializeField]
    protected EquipmentClass equip;//Экипировка персонажа

    protected InteractionInfo currentInteraction = null;
    public InteractionInfo CurrentInteraction { set { currentInteraction = value; SpFunctions.SendMessage(new MessageSentEventArgs(value == null ? "" : value.interactionMessage, value == null ? 4 : 3, 0f)); } }
    [SerializeField]
    protected List<GameObject> doors = new List<GameObject>();//Двери, с которыми персонаж может провзаимодействовать

    protected Transform aboveWallCheck;
    protected GroundChecker lowWallCheck, highWallCheck, edgeCheck;
    protected AIController AI;

    #endregion //fields

    //Инициализация полей и переменных
    public override void Awake()
    {
        base.Awake();
        HAA = GetComponent<HumanoidActorActions>();
        AI = GetComponent<AIController>();
        orgStats.maxHealth = 100f;
        orgStats.health = 100f;

    }

    public virtual void Start()
    {
        InitializeEquipment();
    }

    public override void Update()
    {
        Type type = gameObject.GetComponent<InterObjController>().GetType();
        if (!GameStatistics.paused)
        {

            if ((orgStats.hitted > 0f) && (orgStats.health > 0f))
            {
                Hitted();
            }

            if (orgStats.health <= 0f)
            {
                Death();
            }
            if (pActions != null)
            {
                if (!death)
                {

                    #region UsualState

                    if ((envStats.interaction == interactionEnum.noInter) || (envStats.interaction == interactionEnum.mount))
                    {

                        #region mainWeapon
                        //это условие обрабатывае ситуацию когда быстро(за один кадр) был отжита и нажата кнопка атаки(
                        if (Input.GetButtonUp("PAttack") && Input.GetButtonDown("PAttack"))
                        {
                            if (HAA.mainWeapon is SimpleWeapon && attack != null)
                            {
                                if (HAA.chargeValue > attack.chargeAttack.deadTime && attack.chargeAttack.finalTime > 0f)//чардж копился
                                {
                                    HAA.EndCharge(attack);
                                }
                                else//чардж не копился
                                {
                                    HAA.Attack(attack);
                                }
                                attack = null;
                            }
                            if (HAA.secondaryWeapon is Bow)
                            {
                                if (HAA.canCharge && HAA.chargeValue < 0)//персонаж прицеливался
                                {
                                    HAA.EndCharge(((Bow)HAA.secondaryWeapon).arrowStats);
                                }
                                else//персонаж не прицеливался
                                {
                                    HAA.Attack(((Bow)HAA.secondaryWeapon).arrowStats);
                                }
                            }
                        }

                        if (Input.GetButtonDown("PAttack"))
                        {
                            if (HAA.mainWeapon is SimpleWeapon)
                            {
                                attackState state = GetAttackState();
                                attack = ((SimpleWeapon)HAA.mainWeapon).GetAttack(state.ToString());
                                if (attack != null)
                                {
                                    if (attack.chargeAttack.finalTime <= 0)//считать что если время накопление анаки не больше нуля то чардж атаки нет
                                    {
                                        HAA.Attack(attack);
                                    }
                                    else//чардж есть
                                    {
                                        if (HAA.chargeValue >= 0)//теоретически это условие не выполнится никогда так как 
                                        {                        //после всякого чарджа он и так всё обнуляет
                                            HAA.chargeValue = 0f;//но осторожность не повредит
                                            HAA.canCharge = false;
                                        }
                                    }
                                }
                            }
                        }

                        if (Input.GetButton("PAttack"))
                        {
                            if (HAA.mainWeapon is SimpleWeapon && attack != null)
                            {
                                if (HAA.chargeValue >= 0)//если персонаж не прицеливаемся
                                {
                                    HAA.isFightMode = true;
                                    HAA.chargeValue += Time.deltaTime;
                                    if (HAA.chargeValue > attack.chargeAttack.deadTime)// && attack.chargeAttack.deadTime > 0 убрал пока
                                    {
                                        HAA.StartCharge(attack);
                                    }
                                    if (HAA.canCharge)
                                    {
                                        if (HAA.chargeValue >= attack.chargeAttack.finalTime && attack.chargeAttack.autoFinal)
                                        {
                                            HAA.EndCharge(attack);
                                        }
                                    }
                                    else
                                    {
                                        if (HAA.chargeValue > attack.chargeAttack.deadTime)// && attack.chargeAttack.deadTime > 0 убрал пока
                                        {
                                            HAA.chargeValue = attack.chargeAttack.deadTime;
                                        }
                                    }
                                }
                            }
                            if (HAA.secondaryWeapon is Bow && envStats.groundness != groundnessEnum.inAir)
                            {
                                if (HAA.chargeValue < 0.2f)
                                {
                                    if (HAA.chargeValue >= 0)
                                    {
                                        HAA.isFightMode = true;
                                        HAA.chargeValue += Time.deltaTime;
                                    }
                                    else
                                    {
                                        if (HAA.movingDirection == orientationEnum.left)
                                        {
                                            HAA.chargeValue += Input.GetAxis("PHorizontal");
                                        }
                                        if (HAA.movingDirection == orientationEnum.right)
                                        {
                                            HAA.chargeValue -= Input.GetAxis("PHorizontal");
                                        }
                                        HAA.isFightMode = true;
                                        if (HAA.chargeValue > -10f)
                                            HAA.chargeValue = -10f;
                                        if (HAA.chargeValue < -190f)
                                            HAA.chargeValue = -190f;
                                        GameObject.Find("Aim").transform.localPosition = new Vector3
                                            (
                                                7 * Mathf.Cos((HAA.chargeValue + 100) * Mathf.PI / 180f),
                                                 2.5f + 7 * Mathf.Sin((HAA.chargeValue + 100) * Mathf.PI / 180f),
                                                 0f
                                            );
                                    }
                                }
                                else
                                {
                                    HAA.StartCharge(((Bow)HAA.secondaryWeapon).arrowStats);
                                }
                            }
                        }

                        if (Input.GetButtonUp("PAttack"))
                        {
                            if (HAA.mainWeapon is SimpleWeapon && attack != null)
                            {
                                if (HAA.chargeValue > attack.chargeAttack.deadTime && attack.chargeAttack.finalTime > 0f)
                                {
                                    HAA.EndCharge(attack);
                                }
                                else
                                {
                                    if (attack.chargeAttack.finalTime > 0)
                                    {
                                        HAA.Attack(attack);
                                    }
                                }
                                if (HAA.chargeValue > 0f)
                                {
                                    HAA.chargeValue = 0f;
                                }
                            }
                            if (HAA.secondaryWeapon is Bow)
                            {
                                if (HAA.canCharge && HAA.chargeValue < 0)
                                {
                                    HAA.EndCharge(((Bow)HAA.secondaryWeapon).arrowStats);
                                }
                                else
                                {
                                    HAA.Attack(((Bow)HAA.secondaryWeapon).arrowStats);
                                }
                            }
                        }

                        #endregion//mainWeapon

                        #region secondaryWeapon

                        if (Input.GetButtonUp("PSecondAttack") && Input.GetButtonDown("PSecondAttack") && attack != null)
                        {
                            if (HAA.secondaryWeapon is Shield)
                            {
                                HAA.EndCharge(((Shield)HAA.secondaryWeapon).defStats);
                            }
                            else if (HAA.secondaryWeapon is SimpleWeapon && attack != null)
                            {
                                if (HAA.chargeValue > attack.chargeAttack.deadTime && attack.chargeAttack.finalTime > 0f)
                                {
                                    HAA.EndCharge(attack);
                                }
                                else
                                {
                                    if (attack.chargeAttack.finalTime > 0)
                                    {
                                        HAA.Attack(attack);
                                    }
                                }
                            }
                        }

                        if (Input.GetButtonDown("PSecondAttack"))
                        {
                            if (HAA.secondaryWeapon is SimpleWeapon)
                            {
                                attackState state = GetAttackState();
                                attack = ((SimpleWeapon)HAA.secondaryWeapon).GetAttack(state.ToString());
                                if (attack != null)
                                {
                                    if (attack.chargeAttack.finalTime <= 0)//считать что если время накопление анаки не больше нуля то чардж атаки нет
                                    {
                                        HAA.Attack(attack);
                                    }
                                    else
                                    {
                                        if (HAA.chargeValue >= 0)
                                        {
                                            HAA.chargeValue = 0f;
                                            HAA.canCharge = false;
                                        }
                                    }
                                }
                            }
                        }

                        if (Input.GetButton("PSecondAttack") && attack != null)
                        {
                            if (HAA.secondaryWeapon is Shield && envStats.groundness != groundnessEnum.inAir)
                            {
                                HAA.StartCharge(((Shield)HAA.secondaryWeapon).defStats);
                            }
                            else if (HAA.secondaryWeapon is SimpleWeapon && attack != null)
                            {
                                if (HAA.chargeValue >= 0)
                                {
                                    HAA.isFightMode = true;
                                    HAA.chargeValue += Time.deltaTime;
                                    if (HAA.chargeValue > attack.chargeAttack.deadTime && attack.chargeAttack.deadTime > 0)
                                    {
                                        HAA.StartCharge(attack);
                                    }
                                    if (HAA.canCharge)
                                    {
                                        if (HAA.chargeValue >= attack.chargeAttack.finalTime && attack.chargeAttack.autoFinal)
                                        {
                                            HAA.EndCharge(attack);
                                        }
                                    }
                                    else
                                    {
                                        if (HAA.chargeValue > attack.chargeAttack.deadTime && attack.chargeAttack.deadTime > 0)
                                        {
                                            HAA.chargeValue = attack.chargeAttack.deadTime;
                                        }
                                    }
                                }
                            }
                        }

                        if (Input.GetButtonUp("PSecondAttack") && attack != null)
                        {
                            if (HAA.secondaryWeapon is Shield)
                            {
                                HAA.EndCharge(((Shield)HAA.secondaryWeapon).defStats);
                            }
                            else if (HAA.secondaryWeapon is SimpleWeapon && attack != null)
                            {
                                if (HAA.chargeValue > attack.chargeAttack.deadTime && attack.chargeAttack.finalTime > 0f)
                                {
                                    HAA.EndCharge(attack);
                                }
                                else
                                {
                                    if (attack.chargeAttack.finalTime > 0)
                                    {
                                        HAA.Attack(attack);
                                    }
                                }
                                if (HAA.chargeValue > 0f)
                                {
                                    HAA.chargeValue = 0f;
                                }
                            }
                        }

                        #endregion//secondaryWeapon

                        #region FastRunInput

                        if (Input.GetButtonDown("PHorizontal"))
                        {
                            if (Mathf.Abs(fastRunInputCount) < fastRunMaxCount)
                            {
                                float value = Input.GetAxis("PHorizontal");
                                if (fastRunInputCount * value < 0)
                                {
                                    fastRunInputCount = 0;
                                }
                                fastRunInputCount += SpFunctions.RealSign(value);
                                fastRunInputTimer = fastRunInputTime;
                            }
                        }

                        if (fastRunInputTimer > 0)
                        {
                            fastRunInputTimer -= Time.deltaTime;
                        }

                        #endregion //FastRunInput

                        if (Input.GetButton("PHorizontal"))
                        {
                            if (Mathf.Abs(fastRunInputCount) == fastRunMaxCount)
                            {
                                pActions.SetMaxSpeed(pActions.FastRunSpeed);
                            }
                            else
                            {
                                if ((pActions.CurrentSpeed != pActions.FastRunSpeed) && (envStats.groundness == groundnessEnum.grounded))
                                {
                                    pActions.SetMaxSpeed(pActions.RunSpeed);
                                }
                                if (fastRunInputTimer <= 0f)
                                {
                                    fastRunInputCount = 0;
                                }
                            }
                            orientationEnum orientation = (orientationEnum)SpFunctions.RealSign(Input.GetAxis("PHorizontal"));
                            Debug.Log((orientationEnum)SpFunctions.RealSign(Input.GetAxis("PHorizontal")));
                            pActions.Turn(new ActionClass(orientation));
                            pActions.StartWalking(new ActionClass(orientation));

                            #region ObstacleInteraction

                            if (envStats.obstacleness != obstaclenessEnum.noObstcl)
                            {
                                pActions.SetMaxSpeed(pActions.RunSpeed);
                                fastRunInputCount = 0;
                                pActions.StopWalking();
                                pActions.WallInteraction();
                                if (envStats.obstacleness == obstaclenessEnum.lowObstcl)
                                {
                                    if (lowWallCheck.GetCount() > 0)
                                    {
                                        pActions.AvoidLowObstacle(CheckHeight());
                                        envStats.interaction = interactionEnum.lowEdge;
                                    }
                                }
                                else if (envStats.obstacleness == obstaclenessEnum.medObstcl)
                                {
                                    if (Input.GetButtonDown("PJump"))
                                    {
                                        pActions.AvoidHighObstacle(CheckHeight());
                                        envStats.interaction = interactionEnum.edge;
                                    }
                                }
                                else if (envStats.obstacleness == obstaclenessEnum.highObstcl)
                                {
                                    pActions.HangHighObstacle();
                                    hanging = true;
                                    envStats.interaction = interactionEnum.edge;
                                }
                            }

                            #endregion //ObstacleInteraction

                        }
                        else
                        {
                            if (fastRunInputTimer <= 0)
                            {
                                fastRunInputCount = 0;
                                pActions.SetMaxSpeed(pActions.RunSpeed);
                            }
                            pActions.StopWalking();
                        }

                        if (Input.GetButtonDown("PCrouch") && Input.GetButtonDown("PJump"))
                        {
                            if (envStats.groundness == groundnessEnum.grounded)
                            {
                                pActions.Flip(null);
                            }
                        }
                        else if (Input.GetButtonDown("PJump"))
                        {
                            pActions.Jump(null);
                        }

                        pActions.Crouch((Input.GetButton("PCrouch")) || (WallIsAbove()));

                        if ((envStats.groundness == groundnessEnum.inAir) && (rigid.velocity.y < -30f) && (rigid.velocity.y > minLedgeSpeed))
                        {
                            if (interactions.GetInteractionList() != null ? interactions.GetInteractionList().Count > 0 : false)
                            {
                                if (interactions.GetInteractionList()[0].gameObject.layer == LayerMask.NameToLayer("ledge"))
                                {
                                    Interact(this);
                                }
                            }
                        }

                        UseItem();

                    }

                    #endregion //UsualState

                    #region EdgeState

                    else if (envStats.interaction == interactionEnum.edge)
                    {
                        if (hanging && (Input.GetButtonDown("PJump")))
                        {
                            pActions.AvoidHighObstacle(CheckHeight());
                            hanging = false;
                        }
                    }

                    #endregion //EdgeState

                    #region LowEdgeState

                    else if (envStats.interaction == interactionEnum.lowEdge)
                    {
                        if (rigid.velocity.y > 5f)
                        {
                            pActions.AvoidLowObstacle(CheckHeight());
                        }
                        if (envStats.obstacleness == obstaclenessEnum.noObstcl)
                        {
                            pActions.AvoidLowObstacle(0f);
                            envStats.interaction = interactionEnum.noInter;
                        }
                    }

                    #endregion //LowEdgeState

                    #region SpecialMovementState

                    else if (envStats.interaction != interactionEnum.interactive)
                    {
                        if (rigid.useGravity == true)
                        {
                            pActions.Hang(true);
                        }
                        if (Input.GetButton("PHorizontal") || Input.GetButton("PVertical"))
                        {
                            float valueX = Input.GetAxis("PHorizontal");
                            float valueY = Input.GetAxis("PVertical");
                            Vector2 climbingDirection = new Vector2(valueX, valueY).normalized;
                            if (Physics.OverlapSphere(interCheck.position + new Vector3(climbingDirection.x * observeDistance, climbingDirection.y * observeDistance, 0f), interRadius, whatIsInteractable).Length > 0)
                            {
                                pActions.StartClimbing(climbingDirection);
                            }
                            else
                            {
                                pActions.StopWalking();
                            }
                        }
                        else
                        {
                            pActions.StopWalking();
                        }

                        if (envStats.interaction == interactionEnum.platform)
                        {
                            if ((Input.GetButton("PJump")) || (rigid.velocity.y > 5f))
                            {
                                pActions.ClimbOntoThePlatform();
                            }
                        }
                    }

                    #endregion //SpecialMovementState

                    #region interactiveObjects

                    else
                    {
                        if (interactionObject is MoveableBoxActions)
                        {
                            orientationEnum orientation = (orientationEnum)SpFunctions.RealSign(Input.GetAxis("PHorizontal"));
                            if ((Input.GetButton("PHorizontal")) && (!((MoveableBoxActions)interactionObject).empty))
                            {
                                pActions.StartWalking(orientation);
                            }
                            else
                            {
                                pActions.StopWalking();
                            }
                        }
                    }

                    #endregion //interactiveObjects

                    if (Input.GetButtonDown("PInteract"))
                    {
                        Interact(this);
                    }

                    if (Input.GetButtonDown("PChangeInteraction"))
                    {
                        ChangeInteraction();
                    }
                    /* if (Input.GetButtonDown("Attack"))//Стоит ли вводить атаки во всех возможных положениях персонажа?
                     {
                         actions.Attack();
                     }*/

                }

                #region ChangeItems

                if (Input.GetButtonDown("PChangeRWeapon"))
                {
                    List<PartController> children = new List<PartController>();
                    CharacterAnimator anim = GetComponentInChildren<CharacterAnimator>();
                    DeletePart(equip.rightWeapon, children, anim);
                    if ((equip.altRightWeapon != null) && (equip.leftWeapon == equip.rightWeapon))
                    {
                        DeletePart(equip.leftWeapon, children, anim);
                    }
                    equip.ChangeRightWeapon();
                    AddPart(equip.rightWeapon, children, anim);
                    SetChildren(anim.parts, children);
                    pActions.SetWeapon(equip.rightWeapon, "Main");
                    pActions.SetWeapon(equip.leftWeapon, "Secondary");
                }

                if (Input.GetButtonDown("PChangeLWeapon"))
                {
                    List<PartController> children = new List<PartController>();
                    CharacterAnimator anim = GetComponentInChildren<CharacterAnimator>();
                    DeletePart(equip.leftWeapon, children, anim);
                    if ((equip.altLeftWeapon != null) && (equip.leftWeapon == equip.rightWeapon))
                    {
                        DeletePart(equip.rightWeapon, children, anim);
                    }
                    equip.ChangeLeftWeapon();
                    AddPart(equip.leftWeapon, children, anim);
                    SetChildren(anim.parts, children);
                    pActions.SetWeapon(equip.rightWeapon, "Main");
                    pActions.SetWeapon(equip.leftWeapon, "Secondary");
                }

                if (Input.GetButtonDown("PChangeItem"))
                {
                    equip.ChangeItem();
                }

                #endregion //ChangeItems

                AnalyzeSituation();
            }
        }
    }

    public override void Initialize()
    {
        AI = GetComponent<AIController>();
        if (AI != null)
        {
            orgStats = AI.GetOrgStats();
            dropList = AI.dropList;
            HitParticle = AI.HitParticle;
            DefParticles = AI.DefParticles;
            envStats = AI.GetEnvStats();
            enemies = AI.enemies;
            currentRoom = AI.currentRoom;
            whatIsGround = AI.whatIsGround;
            whatIsInteractable = AI.whatIsInteractable;

        }
        base.Initialize();
        if (pActions != null)
        {
            pActions.SetWeapon(equip.rightWeapon, "Main");
            pActions.SetWeapon(equip.leftWeapon, "Secondary");
        }
        transform.GetComponentInChildren<CharacterAnimator>().SetStats(envStats);
        aboveWallCheck = transform.FindChild("Indicators").FindChild("AboveWallCheck");
        lowWallCheck = transform.FindChild("Indicators").FindChild("LowWallCheck").gameObject.GetComponent<GroundChecker>();
        highWallCheck = transform.FindChild("Indicators").FindChild("HighWallCheck").gameObject.GetComponent<GroundChecker>();
        edgeCheck = transform.FindChild("Indicators").FindChild("EdgeCheck").gameObject.GetComponent<GroundChecker>();
    }

    /// <summary>
    /// Возвращает инвентарь персонажа
    /// </summary>
    public override BagClass GetEquipment()
    {
        return equip;
    }

    /// <summary>
    /// Испольщрваит активный предмет инвентаря
    /// </summary>
    protected override void UseItem()
    {
        ItemBunch useItem = equip.useItem;
        UsableItemClass item = null;
        if (useItem != null ? ((item = (UsableItemClass)useItem.item) != null) : false)
        {
            if (pActions != null)
            {
                if ((item.charge && (pActions.ChargeValue >= 0f)))
                {
                    if (Input.GetButton("PUseItem"))
                    {
                        ItemChargeData chargeData = item.chargeData;
                        if (chargeData.autoInteruption ? (pActions.ChargeValue >= chargeData.maxValue) : false)
                        {
                            pActions.ReleaseItem(useItem);
                        }
                        else
                        {
                            pActions.ChargeItem(useItem);
                        }
                    }
                    else if (Input.GetButtonUp("PUseItem"))
                    {
                        pActions.ReleaseItem(useItem);
                    }
                }
                else if (Input.GetButtonDown("PUseItem"))
                {
                    if (!item.charge)
                    {
                        pActions.UseItem(useItem);
                    }
                    else
                    {
                        pActions.ChargeValue = 0f;
                    }
                }
                if (useItem.quantity == 0)
                {
                    equip.useItem = null;
                    equip.useItems[equip.useItems.IndexOf(useItem)] = null;
                    equip.ChangeItem();
                }
            }
        }
    }

    #region Interact

    /// <summary>
    /// Здесь описаны все взаимодействия, что могут пройзойти между персонажем и окружающим миром
    /// </summary>
    public override void Interact(InterObjController interactor)
    {
        if ((pActions != null) && (currentInteraction != null))
        {
            pActions.StopWalking();
            if (currentInteraction != null)
            {
                switch (currentInteraction.interactionType)
                {
                    case interactionInfoEnum.interupt:
                        {
                            if (envStats.interaction == interactionEnum.mount)
                            {
                                RemoveMount();
                            }
                            else
                            {
                                pActions.Hang(false);
                                currentInteraction = null;
                            }
                            break;
                        }
                    case interactionInfoEnum.intObj:
                        {
                            InterObjController interObj = currentInteraction.interObj.GetComponent<InterObjController>();
                            if (interObj != null)
                            {
                                interObj.Interact(interactor);
                            }
                            currentInteraction = null;
                            break;
                        }
                }
            }
        }
    }

    /// <summary>
    /// Эта функция вызывается при нанесении урона
    /// </summary>
    public override void Hitted()
    {
        pActions.Hitted();
    }

    /// <summary>
    /// Эта функция вызывается при смерти персонажа
    /// </summary>
    public override void Death()
    {
        if (!death)
        {
            death = true;
            pActions.Death();
        }
    }

    /// <summary>
    /// спешиться
    /// </summary>
    public override void RemoveMount()
    {
        base.RemoveMount();
        mounted = false;
        pActions.RemoveMount();
        envStats.interaction = interactionEnum.interactive;
    }

    /// <summary>
    /// Сесть на маунта
    /// </summary>
    public override void UseMount(MountActions mount)
    {
        base.UseMount(mount);
        OnRemoveMount += currentMount.Appear;
        if (equip.leftWeapon != null)
        {
            if (equipWindow.HaveEmptySlots(1))
            {
                mounted = true;
                equipWindow.AddItemInBag(equipWindow.leftWeaponSlot1.itemBunch);
                equipWindow.leftWeaponSlot1.DeleteItem();
                if (equip.leftWeapon.weaponType == "bow" || equip.leftWeapon.weaponType == "twoHandedSword")
                {
                    equipWindow.rightWeaponSlot1.DeleteItem();
                }
            }
            else
            {
                return;
            }
        }
        pActions.UseMount(mount);
    }
    #endregion //Interact

    #region Analyze

    /// <summary>
    /// Оценивает окружающую персонажа обстановку
    /// </summary>
    protected override void AnalyzeSituation()
    {
        base.AnalyzeSituation();
        DefineInteractable();
    }

    /// <summary>
    /// Определение типа препятствия, находящегося перед персонажем
    /// </summary>
    protected override void DefinePrecipice()
    {
        pActions.PrecipiceIsForward = (!(Physics.OverlapSphere(precipiceCheck.position, precipiceRadius, whatIsGround).Length > 0) && (envStats.groundness == groundnessEnum.grounded) && (!Input.GetButton("CamMove")));
    }

    /// <summary>
    /// Функция, учитывающая, находятся ли перед персонажем какие-нибудь препятствия
    /// </summary>
    protected override void CheckObstacles()
    {
        if ((lowWallCheck.GetCount() > 0) &&
            (frontWallCheck.GetCount() == 0))
        {
            envStats.obstacleness = obstaclenessEnum.lowObstcl;
        }
        else if ((highWallCheck.GetCount() > 0))
        {
            if (CheckEdge())
            {
                envStats.obstacleness = obstaclenessEnum.highObstcl;
            }
            else if (frontWallCheck.GetCount() > 0)
            {
                envStats.obstacleness = obstaclenessEnum.wall;
            }
        }
        else if ((highWallCheck.GetCount() == 0) &&
         (frontWallCheck.GetCount() > 0))
        {
            envStats.obstacleness = obstaclenessEnum.medObstcl;
        }
        else
        {
            envStats.obstacleness = obstaclenessEnum.noObstcl;
        }
    }

    /// <summary>
    /// Определение типа взаимодействия
    /// </summary>
    protected override void DefineInteractable()
    {

        #region specialMovement

        if ((envStats.interaction != interactionEnum.noInter) &&
            (envStats.interaction != interactionEnum.interactive) &&
            (envStats.interaction != interactionEnum.edge))
        {
            if (envStats.interaction != interactionEnum.mount)
            {
                if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("thicket")).Length > 0)
                {
                    envStats.interaction = interactionEnum.thicket;
                }
                else if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("rope")).Length > 0)
                {
                    envStats.interaction = interactionEnum.rope;
                }
                else if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("ladder")).Length > 0)
                {
                    envStats.interaction = interactionEnum.ladder;
                }
                else if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("ledge")).Length > 0)
                {
                    envStats.interaction = interactionEnum.ledge;
                }
                else if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("platform")).Length > 0)
                {
                    envStats.interaction = interactionEnum.platform;
                }
            }
            if (currentInteraction == null)
            {
                CurrentInteraction = new InteractionInfo(interactionInfoEnum.interupt, interuptionMessages[envStats.interaction], null);
            }
        }
        else if (currentInteraction != null ? currentInteraction.interactionType == interactionInfoEnum.interupt : false)
        {
            CurrentInteraction = null;
        }

        #endregion //specialMovement


        #region interactiveObjects

        if (currentInteraction == null ? true : currentInteraction.interactionType == interactionInfoEnum.intObj)
        {
            List<InterObjController> _interactions = interactions.GetInteractionList();
            if (currentInteraction != null)
            {
                if (!_interactions.Contains(currentInteraction.interObj.GetComponent<InterObjController>()))
                {
                    CurrentInteraction = null;
                }
            }
            if (currentInteraction == null)
            {
                foreach (InterObjController interaction in _interactions)
                {
                    if ((envStats.interaction != interactionEnum.noInter) && (envStats.interaction != interactionEnum.interactive))
                    {
                        if ((SpMoveSourceController)interaction == null)
                        {
                            CurrentInteraction = new InteractionInfo(interactionInfoEnum.intObj, interaction.Info, interaction.gameObject);
                            break;
                        }
                    }
                    else
                    {
                        CurrentInteraction = new InteractionInfo(interactionInfoEnum.intObj, interaction.Info, interaction.gameObject);
                        break;
                    }
                }
            }
        }

        #endregion //interactiveObjects



    }

    /// <summary>
    /// Функция, вызываемая, когда игрок хочет поменять объект взаимодействия
    /// </summary>
    protected virtual void ChangeInteraction()
    {
        int regNumb = 0;
        if (currentInteraction.interactionType == interactionInfoEnum.intObj)
        {
            goto regInteractiveObjects;
        }

    regSpMovement:
        #region specialMovement

        {
            if ((envStats.interaction != interactionEnum.noInter) &&
                (envStats.interaction != interactionEnum.interactive) &&
                (envStats.interaction != interactionEnum.edge))
            {
                CurrentInteraction = new InteractionInfo(interactionInfoEnum.interupt, interuptionMessages[envStats.interaction], null);
                return;
            }
            regNumb++;
            if (regNumb > 4)
            {
                return;
            }
            CurrentInteraction = null;
        }
    #endregion //specialMovement

    regInteractiveObjects:
        #region interactiveObjects
        {
            List<InterObjController> _interactions = interactions.GetInteractionList();
            int interactionIndex = currentInteraction != null ? (currentInteraction.interactionType == interactionInfoEnum.intObj ? _interactions.IndexOf(currentInteraction.interObj.GetComponent<InterObjController>()) : -1) : -1;
            if (_interactions.Count > interactionIndex + 1)
            {
                InterObjController intObj1 = _interactions[interactionIndex + 1];
                CurrentInteraction = new InteractionInfo(interactionInfoEnum.intObj, intObj1.Info, intObj1.gameObject);
                return;
            }
            regNumb++;
            if (regNumb > 4)
            {
                return;
            }
            CurrentInteraction = null;
        }
        #endregion //interactiveObjects
    }

    protected virtual bool WallIsAbove()
    {
        pActions.wallIsAbove = Physics.OverlapSphere(aboveWallCheck.position, obstacleRadius, whatIsGround).Length > 0;
        return pActions.wallIsAbove;
    }

    protected virtual float CheckHeight()
    {
        float delY = 0.3f;
        Vector3 pos;
        if (envStats.obstacleness == obstaclenessEnum.highObstcl)
        {
            pos = highWallCheck.transform.position;
        }
        else
        {
            pos = lowWallCheck.transform.position;
        }
        while (Physics.OverlapSphere(pos, obstacleRadius, whatIsGround).Length > 0)
        {
            pos = new Vector3(pos.x, pos.y + delY, pos.z);
        }
        return (pos.y - lowWallCheck.transform.position.y);
    }

    /// <summary>
    /// Проверить, можно ли зацепиться за верхнее препятствие
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckEdge()
    {
        return edgeCheck.GetCount() == 0;
    }

    /// <summary>
    /// Определить, какую атаку можно использовать
    /// </summary>
    /// <returns></returns>
    public attackState GetAttackState()
    {
        if (Input.GetButton("Crouch"))
        {
            return attackState.crouch;
        }
        if (envStats.groundness == groundnessEnum.grounded)
        {
            if (Input.GetButton("Horizontal"))
            {
                if (Mathf.Abs(fastRunInputCount) > 1)
                {
                    return attackState.run;
                }
                else
                {
                    return attackState.walk;
                }
            }
            else if (Input.GetKey(KeyCode.W))
            {
                return attackState.up;
            }
            else
            {
                return attackState.stay;
            }
        }
        else if (envStats.groundness == groundnessEnum.inAir)
        {
            if (Input.GetKey(KeyCode.S))
            {
                return attackState.jumpDawn;
            }
            else
            {
                return attackState.jump;
            }
        }
        return attackState.NO;
    }

    #endregion //Analyze

    #region equipment

    /// <summary>
    /// Инициализировать инвентарь 
    /// </summary>
    protected void InitializeEquipment()
    {
        List<PartController> parts = new List<PartController>();
        CharacterAnimator cAnim = GetComponentInChildren<CharacterAnimator>();
        ArmorSet armorSet = equip.armor;
        ConsiderArmor(armorSet.helmet, true);
        ConsiderArmor(armorSet.cuirass, true);
        ConsiderArmor(armorSet.pants, true);
        ConsiderArmor(armorSet.gloves, true);
        ConsiderArmor(armorSet.boots, true);
        if (equip.rightWeapon != null)
        {
            AddPart(equip.rightWeapon, parts, cAnim);
        }
        if (equip.leftWeapon != null)
        {
            AddPart(equip.leftWeapon, parts, cAnim);
        }
        SetChildren(cAnim.parts, parts);
        if (pActions != null)
        {
            pActions.SetSpeeds(orgStats.velocity, orgStats.defVelocity);
        }
    }

    /// <summary>
    /// Функция учёта эффектов, происходящих при надевании того или иного элемента доспехов
    /// </summary>
    protected void ConsiderArmor(ArmorClass armor, bool consider)
    {
        if (armor != null)
        {
            if (consider)
            {
                orgStats.defence += armor.defence;
                orgStats.velocity += armor.velocity;
                foreach (BuffClass buff in armor.buffs)
                {
                    buffList.AddBuff(buff);
                }
                ConsiderSetBuff(armor);
            }
            else
            {
                orgStats.defence -= armor.defence;
                orgStats.velocity -= armor.velocity;
                foreach (BuffClass buff in armor.buffs)
                {
                    buffList.RemoveBuff(buff);
                }
                buffList.RemoveBuff(armor.setBuff);
            }
            orgStats.OnParametersChanged(new OrganismEventArgs(100f));
        }
    }

    /// <summary>
    /// Учесть, что добавив новый элемент доспеха, персонаж мог приобрести бонус сета
    /// </summary>
    protected void ConsiderSetBuff(ArmorClass armor)
    {
        ArmorSet armorSet = equip.armor;
        int count = 0;
        count += CompareSets(armor, armorSet.helmet);
        count += CompareSets(armor, armorSet.cuirass);
        count += CompareSets(armor, armorSet.pants);
        count += CompareSets(armor, armorSet.gloves);
        count += CompareSets(armor, armorSet.boots);
        if (count >= armor.setNumber)
        {
            buffList.AddBuff(armor.setBuff);
        }
    }

    /// <summary>
    /// Проверка на то, составляют ли 2 элемента доспеха один и тот же сет
    /// </summary>
    /// <returns>1- если составляют, 0 - если из разных сетов</returns>
    protected int CompareSets(ArmorClass armor1, ArmorClass armor2)
    {
        if (armor1 == null)
        {
            return 0;
        }
        if (armor2 == null)
        {
            return 0;
        }
        if (armor1.setBuff == armor2.setBuff)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Сменить доспех, оружие, кольцо или используемый предмет
    /// </summary>
    public void ChangeItem(ItemBunch itemBunch, string itemType)
    {
        #region init
        ItemClass item;
        if (itemBunch == null)
        {
            item = null;
        }
        else
        {
            item = itemBunch.item;
        }

        CharacterAnimator anim = GetComponentInChildren<CharacterAnimator>();
        List<PartController> childParts = new List<PartController>();

        List<ItemClass> removeItems = equip.ChangeEquipmentElement(itemBunch, itemType);

        #endregion //init

        if (!string.Equals(itemType, "rightWeapon2") && !string.Equals(itemType, "leftWeapon2"))
        {
            #region visualize

            AddPart(item, childParts, anim);

            #endregion //visualize

            #region changeEquipment

            ArmorClass armor;

            for (int i = 0; i < removeItems.Count; i++)
            {
                if (removeItems[i] != null)
                {
                    if (removeItems[i] is ArmorClass)
                    {
                        armor = (ArmorClass)removeItems[i];
                        ConsiderArmor(armor, false);
                    }
                }
            }

            if (item != null)
            {
                if (string.Equals(item.type, "armor"))
                {
                    armor = (ArmorClass)item;
                    ConsiderArmor(armor, true);
                    if (pActions != null)
                    {
                        pActions.SetSpeeds(orgStats.velocity, orgStats.defVelocity);
                    }
                }
            }
            if (string.Equals(itemType, "rightWeapon1") || string.Equals(itemType, "leftWeapon1"))
            {
                if (pActions != null)
                {
                    pActions.SetWeapon(equip.rightWeapon, "Main");
                    pActions.SetWeapon(equip.leftWeapon, "Secondary");
                }
                if (equip.rightWeapon == null)
                {
                    if (equip.leftWeapon == null)
                    {
                        pActions.SetWeapon(defaultWeapon2, "Main");
                    }
                    else
                    {
                        pActions.SetWeapon(defaultWeapon1, "Main");
                    }
                }
            }

            for (int i = 0; i < removeItems.Count; i++)
            {
                DeletePart(removeItems[i], childParts, anim);
            }

            #endregion //changeEquipment

            #region setChildren

            SetChildren(anim.parts, childParts);
        }
        #endregion //setChildren

    }

    /// <summary>
    /// Функция, вызываемая при добавлении новой части
    /// </summary>
    public void AddPart(ItemClass item, List<PartController> childParts, CharacterAnimator anim)
    {
        if (item != null)
        {
            List<PartController> parts = anim.parts;
            PartController part = null;
            if (item.type != "weapon")
            {
                foreach (ItemVisual itemVis in item.itemVisuals)
                {
                    GameObject obj = Instantiate(itemVis.part) as GameObject;
                    obj.transform.parent = anim.transform;
                    obj.transform.localPosition = itemVis.pos;
                    obj.transform.localScale = new Vector3(1f, 1f, 1f);
                    part = obj.GetComponent<PartController>();
                    if (part != null)
                    {
                        parts.Add(part);
                        if (dependedPartTypes.Contains(part.partType))
                        {
                            childParts.Add(part);
                        }
                    }
                }
            }
            else
            {
                HumanoidActorActions haa = (HumanoidActorActions)pActions;
                if (haa.isFightMode)
                {
                    GameObject obj = Instantiate(item.itemVisuals[1].part) as GameObject;
                    obj.transform.parent = anim.transform;
                    obj.transform.localPosition = item.itemVisuals[1].pos;
                    obj.transform.localScale = new Vector3(1f, 1f, 1f);
                    part = obj.GetComponent<PartController>();
                    if (part != null)
                    {
                        parts.Add(part);
                        if (dependedPartTypes.Contains(part.partType))
                        {
                            childParts.Add(part);
                        }
                    }
                }
                else
                {
                    GameObject obj = Instantiate(item.itemVisuals[0].part) as GameObject;
                    obj.transform.parent = anim.transform;
                    obj.transform.localPosition = item.itemVisuals[0].pos;
                    obj.transform.localScale = new Vector3(1f, 1f, 1f);
                    part = obj.GetComponent<PartController>();
                    if (part != null)
                    {
                        parts.Add(part);
                        if (dependedPartTypes.Contains(part.partType))
                        {
                            childParts.Add(part);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Функция, вызываемая при добавлении новой части
    /// </summary>
    public void AddPart(GameObject partObj, List<PartController> childParts, CharacterAnimator anim)
    {
        List<PartController> parts = anim.parts;
        PartController part = null;
        GameObject obj = Instantiate(partObj) as GameObject;
        part = obj.GetComponent<PartController>();
        obj.transform.parent = anim.transform;
        obj.transform.localPosition = part.transform.position;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        if (part != null)
        {
            parts.Add(part);
            if (dependedPartTypes.Contains(part.partType))
            {
                childParts.Add(part);
            }
        }
    }

    /// <summary>
    /// Функция, вызываемая для удаления не использующихся частей
    /// </summary>
    public void DeletePart(ItemClass item, List<PartController> childParts, CharacterAnimator anim)
    {
        if (item != null)
        {
            List<PartController> parts = anim.parts;
            PartController part = null;
            foreach (ItemVisual itemVis in item.itemVisuals)
            {
                part = null;
                foreach (PartController _part in parts)
                {
                    if (_part.gameObject.name.Contains(itemVis.part.name))
                    {
                        part = _part;
                        break;
                    }
                }
                if (part != null)
                {
                    parts.Remove(part);
                    if (part.childParts != null)
                    {
                        foreach (PartController _part in part.childParts)
                        {
                            if (!childParts.Contains(_part))
                            {
                                childParts.Add(_part);
                            }
                        }
                    }
                    DestroyImmediate(part.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Функция, вызываемая для удаления не использующихся частей
    /// </summary>
    public void DeletePart(string partName, List<PartController> childParts, CharacterAnimator anim)
    {
        List<PartController> parts = anim.parts;
        PartController part = null;
        foreach (PartController _part in parts)
        {
            Debug.Log(partName + "    " + _part.gameObject.name);
            if (_part.gameObject.name.Contains(partName))
            {
                part = _part;
                break;
            }
        }
        if (part != null)
        {
            parts.Remove(part);
            if (part.childParts != null)
            {
                foreach (PartController _part in part.childParts)
                {
                    if (!childParts.Contains(_part))
                    {
                        childParts.Add(_part);
                    }
                }
            }
            DestroyImmediate(part.gameObject);
        }
    }

    /// <summary>
    /// Функция, используемая для установления зависимостей между частями
    /// </summary>
    public void SetChildren(List<PartController> parts, List<PartController> childParts)
    {
        foreach (PartController child in childParts)
        {
            if (!partDependencies.ContainsKey(child.partType))
            {
                break;
            }
            foreach (PartController _part in parts)
            {
                if (string.Equals(partDependencies[child.partType], _part.partType))
                {
                    _part.childParts.Add(child);
                    break;
                }
            }
        }
    }

    #endregion //equipment

    public void Connect()
    {
        AI.enabled = true;
        enabled = false;
    }

    public void Disconnect()
    {
        AI.enabled = false;
        enabled = true;
    }

    public void FollowPlayer(object sender, RoomChangedEventArgs e)
    {
        transform.position = ((MonoBehaviour)sender).transform.position;
        currentRoom = e.Room;
    }
    
    public void FindActions()
    {
        if (pActions is NPCActions)
        {
            pActions = GetComponent<HumanoidActorActions>();
        }
        else
        {
            pActions = GetComponent<NPCActions>();
        }
               
    }
}

#if UNITY_EDITOR
/// <summary>
/// Редактор клавиатурного контроллера
/// </summary>
[CustomEditor(typeof(KeyboardActorController))]
public class PartnerControllerEditor : Editor
{

    private Direction direction;
    private OrganismStats orgStats;
    private EnvironmentStats envStats;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        KeyboardActorController obj = (KeyboardActorController)target;
        direction = obj.GetDirection();
        orgStats = obj.GetOrgStats();
        envStats = obj.GetEnvStats();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.IntField("direction", (int)direction.dir);
        EditorGUILayout.EnumPopup(envStats.groundness);
        EditorGUILayout.EnumPopup(envStats.obstacleness);
        EditorGUILayout.EnumPopup(envStats.interaction);
        //orgStats.maxHealth = EditorGUILayout.FloatField("Max Health", orgStats.maxHealth);
        //orgStats.velocity = EditorGUILayout.FloatField("Velocity", orgStats.velocity);
        //EditorGUILayout.FloatField("Health", orgStats.health);
    }
}
#endif