using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class KeyboardActorController : PersonController
{

    #region consts

    private const float doorDistance = 4.5f;

    private const float obstacleRadius = 1f;

    protected const int fastRunMaxCount = 2;
    protected const float fastRunInputTime = 0.3f;

    protected float observeDistance = 1.5f;

    protected const float minLedgeSpeed = -90f;

    #endregion //consts

    #region enums

    protected enum directionClaveEnum { leftHold = -2, leftDown = -1, notPressed = 0, rightDown = 1, rightHold = 2 };

    #endregion //enums

    #region parametres

    protected directionClaveEnum dirClave = directionClaveEnum.notPressed;

    public int fastRunInputCount = 0;
    protected float fastRunInputTimer = 0f;

    #endregion //parametres

    #region fields

    private InteractionChecker interactions;

    protected Transform aboveWallCheck;
    protected GroundChecker lowWallCheck, frontWallCheck, highWallCheck;

    #endregion //fields

    //Инициализация полей и переменных
    public override void Awake()
    {
        base.Awake();
        stats.maxHealth = 100f;
        stats.health = 100f;
    }

    public override void Initialize()
    {
        base.Initialize();
        actions = GetComponent<HumanoidActorActions>();
        if (stats == null)
        {
            stats = new Stats();
        }
        if (actions != null)
        {
            actions.SetStats(stats);
            actions.SetWeapon(equip.rightWeapon);
        }
        rigid = GetComponent<Rigidbody>();
        transform.GetComponentInChildren<CharacterVisual>().SetStats(stats);
        transform.GetComponentInChildren<CharacterAnimator>().SetStats(stats);
        aboveWallCheck = transform.FindChild("Indicators").FindChild("AboveWallCheck");
        lowWallCheck = transform.FindChild("Indicators").FindChild("LowWallCheck").gameObject.GetComponent<GroundChecker>();
        frontWallCheck = transform.FindChild("Indicators").FindChild("FrontWallCheck").gameObject.GetComponent<GroundChecker>();
        highWallCheck = transform.FindChild("Indicators").FindChild("HighWallCheck").gameObject.GetComponent<GroundChecker>();
        interactions = transform.FindChild("Indicators").gameObject.GetComponentInChildren<InteractionChecker>();
    }

    public override void Update()
    {
        if ((stats.hitted > 0f) && (stats.health > 0f))
        {
            Hitted();
        }

        if (stats.health <= 0f)
        {
            Death();
        }
        if (actions != null)
        {
            if (!death)
            {

                #region UsualState

                if (stats.interaction == interactionEnum.noInter)
                {
                    #region FastRunInput

                    if (Input.GetButtonDown("Horizontal"))
                    {
                        if (Mathf.Abs(fastRunInputCount) < fastRunMaxCount)
                        {
                            float value = Input.GetAxis("Horizontal");
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

                    if (Input.GetButton("Horizontal"))
                    {
                        if (Mathf.Abs(fastRunInputCount) == fastRunMaxCount)
                        {

                            actions.SetMaxSpeed(actions.fastRunSpeed);
                        }
                        else
                        {
                            if ((actions.currentMaxSpeed != actions.fastRunSpeed) && (stats.groundness == groundnessEnum.grounded))
                            {
                                actions.SetMaxSpeed(actions.maxSpeed);
                            }
                            if (fastRunInputTimer <= 0f)
                            {
                                fastRunInputCount = 0;
                            }
                        }
                        orientationEnum orientation = (orientationEnum)SpFunctions.RealSign(Input.GetAxis("Horizontal"));
                        actions.Turn(orientation);
                        actions.StartWalking(orientation);

                        #region ObstacleInteraction

                        if (stats.obstacleness != obstaclenessEnum.noObstcl)
                        {
                            actions.SetMaxSpeed(actions.maxSpeed);
                            fastRunInputCount = 0;
                            actions.StopWalking();
                            actions.WallInteraction();
                            if (stats.obstacleness == obstaclenessEnum.lowObstcl)
                            {
                                if (lowWallCheck.collisions.Count > 0)
                                {
                                    actions.AvoidLowObstacle(CheckHeight());
                                    stats.interaction = interactionEnum.lowEdge;
                                }
                            }
                            if (stats.obstacleness == obstaclenessEnum.highObstcl)
                            {
                                if (Input.GetButtonDown("Jump"))
                                {
                                    actions.AvoidHighObstacle(CheckHeight());
                                    stats.interaction = interactionEnum.edge;
                                }
                                if (highWallCheck.collisions.Count>0)
                                {
                                    actions.HangHighObstacle();
                                    stats.interaction = interactionEnum.edge;
                                }
                            }
                        }

                        #endregion //ObstacleInteraction

                    }
                    else
                    {
                        if (fastRunInputTimer <= 0)
                        {
                            fastRunInputCount = 0;
                            actions.SetMaxSpeed(actions.maxSpeed);
                        }
                        actions.StopWalking();
                    }

                    if (Input.GetButtonDown("Crouch") && Input.GetButtonDown("Jump"))
                    {
                        if (stats.groundness == groundnessEnum.grounded)
                        {
                            actions.Flip();
                        }
                    }
                    else if (Input.GetButtonDown("Jump"))
                    {
                        actions.Jump();
                    }

                    if (Input.GetButtonDown("Interact"))
                    {
                        Interact(this);
                    }

                    actions.Crouch((Input.GetButton("Crouch"))||(WallIsAbove()));

                    if ((stats.groundness == groundnessEnum.inAir)&&(rigid.velocity.y<-30f)&&(rigid.velocity.y > minLedgeSpeed))
                    {
                        if (interactions.interactions.Count > 0)
                        {
                            if (interactions.interactions[0].gameObject.layer == LayerMask.NameToLayer("ledge"))
                            {
                                Interact(this);
                            }
                        }
                    }

                    if (Input.GetButtonDown("Attack"))//Стоит ли вводить атаки во всех возможных положениях персонажа?
                    {
                        actions.Attack();
                    }
                }

                #endregion //UsualState

                #region EdgeState

                else if (stats.interaction == interactionEnum.edge)
                {
                    if (Input.GetButtonDown("Jump") || (rigid.velocity.y > 5f))
                    {
                        actions.AvoidHighObstacle(CheckHeight());
                    }
                    if (stats.obstacleness == obstaclenessEnum.noObstcl)
                    {
                        actions.AvoidHighObstacle(0f);
                        stats.interaction = interactionEnum.noInter;
                    }
                }

                #endregion //EdgeState

                #region LowEdgeState

                else if (stats.interaction == interactionEnum.lowEdge)
                {
                    if (rigid.velocity.y > 5f)
                    {
                        actions.AvoidLowObstacle(CheckHeight());
                    }
                    if (stats.obstacleness == obstaclenessEnum.noObstcl)
                    {
                        actions.AvoidLowObstacle(0f);
                        stats.interaction = interactionEnum.noInter;
                    }
                }

                #endregion //LowEdgeState

                #region SpecialMovementState

                else if (stats.interaction != interactionEnum.interactive)
                {
                    if (rigid.useGravity == true)
                    {
                        actions.Hang(true);
                    }
                    if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                    {
                        float valueX = Input.GetAxis("Horizontal");
                        float valueY = Input.GetAxis("Vertical");
                        Vector2 climbingDirection = new Vector2(valueX, valueY).normalized;
                        if (Physics.OverlapSphere(interCheck.position + new Vector3(climbingDirection.x * observeDistance, climbingDirection.y * observeDistance, 0f), interRadius, whatIsInteractable).Length > 0)
                        {
                            actions.StartClimbing(climbingDirection);
                        }
                        else
                        {
                            actions.StopWalking();
                        }
                    }
                    else
                    {
                        actions.StopWalking();
                    }
                    if (Input.GetButtonDown("Interact"))
                    {   
                        actions.Hang(false);
                    }

                    if (stats.interaction == interactionEnum.platform)
                    {
                        if ((Input.GetButton("Jump")) || (rigid.velocity.y > 5f))
                        {
                            actions.ClimbOntoThePlatform();
                        }
                    }
                }

                #endregion //SpecialMovementState

                if (interactions.dropList.Count > 0)
                {
                    if (interactions.dropList[0].autoPick)
                    {
                        TakeDrop();
                    }
                }

               /* if (Input.GetButtonDown("Attack"))//Стоит ли вводить атаки во всех возможных положениях персонажа?
                {
                    actions.Attack();
                }*/

            }

            AnalyzeSituation();
        }
    }

    #region Interact

    /// <summary>
    /// Здесь описаны все взаимодействия, что могут пройзойти между персонажем и окружающим миром
    /// </summary>
    public override void Interact(InterObjController interactor)
    {
        if (actions != null)
        {
            if (interactions.interactions.Count > 0)
            {
                interactions.interactions[0].Interact(this);
            }
            else if (interactions.dropList.Count > 0)
            {
                TakeDrop();
            }
            else
            {
                DoorInteraction();
            }
        }
    }

    protected override void DoorInteraction()
    {
        Transform trans = sight;
        float zDistance = Mathf.Abs(currentRoom.position.z + currentRoom.size.z / 2 - trans.position.z) - 0.5f;
        RaycastHit hit = new RaycastHit();
        DoorClass door;
        if (Physics.Raycast(new Ray(trans.position, (int)stats.direction * trans.right), out hit, doorDistance))
        {
            door = hit.collider.gameObject.GetComponent<DoorClass>();
            if (door != null)
            {
                if (door.locker.opened)
                {
                    currentRoom = door.roomPath;
                    actions.GoThroughTheDoor(door);
                }
            }
        }

        if (Physics.Raycast(new Ray(trans.position, Input.GetKey(KeyCode.W) ? new Vector3(0f, 0f, -1f) : new Vector3(0f, 0f, 1f)), out hit, zDistance))
        {
            door = hit.collider.gameObject.GetComponent<DoorClass>();
            if (door != null)
            {
                if (door.locker.opened)
                {
                    currentRoom = door.roomPath;
                    actions.GoThroughTheDoor(door);
                }
            }
        }
        base.DoorInteraction();
    }

    void TakeDrop()
    {
        DropClass drop = interactions.dropList[0];
        List<ItemBunch> items = drop.drop;
        for (int i = 0; i < items.Count; i++)
        {
            equip.bag.Add(items[i]);
        }
        interactions.dropList.RemoveAt(0);
        Destroy(drop.gameObject);
    }

    /// <summary>
    /// Эта функция вызывается при нанесении урона
    /// </summary>
    public override void Hitted()
    {
        actions.Hitted();
    }

    /// <summary>
    /// Эта функция вызывается при смерти персонажа
    /// </summary>
    public override void Death()
    {
        if (!death)
        {
            death = true;
            actions.Death();
        }
    }

    #endregion //Interact

    #region Analyze

    /// <summary>
    /// Оценивает окружающую персонажа обстановку
    /// </summary>
    protected override void AnalyzeSituation()
    {
        base.AnalyzeSituation();
        CheckObstacles();
        DefineInteractable();
    }

    /// <summary>
    /// Функция, учитывающая, находятся ли перед персонажем какие-нибудь препятствия
    /// </summary>
    protected void CheckObstacles()
    {
        if ((lowWallCheck.collisions.Count> 0) &&
            (frontWallCheck.collisions.Count == 0))
        {
            stats.obstacleness = obstaclenessEnum.lowObstcl;
        }
        else if ((frontWallCheck.collisions.Count> 0) &&
                (highWallCheck.collisions.Count== 0))
        {
            stats.obstacleness = obstaclenessEnum.highObstcl;
        }
        else if ((frontWallCheck.collisions.Count > 0) &&
            (highWallCheck.collisions.Count > 0))
        {
            stats.obstacleness = obstaclenessEnum.wall;
        }
        else
        {
            stats.obstacleness = obstaclenessEnum.noObstcl;
        }
    }

    protected override void DefineInteractable()
    {
        if ((stats.interaction != interactionEnum.noInter) && (stats.interaction != interactionEnum.interactive))
        {
            if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("thicket")).Length > 0)
            {
                stats.interaction = interactionEnum.thicket;
            }
            else if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("rope")).Length > 0)
            {
                stats.interaction = interactionEnum.rope;
            }
            else if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("stair")).Length > 0)
            {
                stats.interaction = interactionEnum.stair;
            }
            else if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("ledge")).Length > 0)
            {
                stats.interaction = interactionEnum.ledge;
            }
            else if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("platform")).Length > 0)
            {
                stats.interaction = interactionEnum.platform;
            }
        }
    }

    protected virtual bool WallIsAbove()
    {
        return Physics.OverlapSphere(aboveWallCheck.position, obstacleRadius, whatIsGround).Length>0;
    }

    protected virtual float CheckHeight()
    {
        float delY = 0.3f;
        Vector3 pos = lowWallCheck.transform.position;
        while (Physics.OverlapSphere(pos, obstacleRadius, whatIsGround).Length>0)
        {
            pos = new Vector3(pos.x, pos.y + delY, pos.z);
        }
        return (pos.y - lowWallCheck.transform.position.y);
    }

    #endregion //Analyze

}

#if UNITY_EDITOR
/// <summary>
/// Редактор клавиатурного контроллера
/// </summary>
[CustomEditor(typeof(KeyboardActorController))]
public class KeyboardActorEditor : Editor
{
    private Stats stats;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        KeyboardActorController obj = (KeyboardActorController)target;
        stats = (Stats)obj.GetStats();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.IntField("direction", (int)stats.direction);
        EditorGUILayout.EnumPopup(stats.groundness);
        EditorGUILayout.EnumPopup(stats.obstacleness);
        EditorGUILayout.EnumPopup(stats.interaction);
        stats.maxHealth = EditorGUILayout.FloatField("Max Health", stats.maxHealth);
        EditorGUILayout.FloatField("Health", stats.health);
    }
}
#endif