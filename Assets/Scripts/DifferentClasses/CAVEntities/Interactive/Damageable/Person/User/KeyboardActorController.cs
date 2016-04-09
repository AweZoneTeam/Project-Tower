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

    protected List<string> dependedPartTypes = new List<string> { "Sword", "Shield"};
    protected Dictionary<string, string> partDependencies = new Dictionary<string, string> { { "Sword", "RightHand" }, { "Shield", "LeftHand" } };

    #endregion //parametres

    #region fields

    private InteractionChecker interactions;

    [SerializeField]
    protected EquipmentClass equip;//Экипировка персонажа

    protected Transform aboveWallCheck;
    protected GroundChecker lowWallCheck, frontWallCheck, highWallCheck;

    #endregion //fields

    //Инициализация полей и переменных
    public override void Awake()
    {
        base.Awake();
        orgStats.maxHealth = 100f;
        orgStats.health = 100f;
    }

    public virtual void Start()
    {
        InitializeEquipment();
    }

    public override void Update()
    {
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

                    if (envStats.interaction == interactionEnum.noInter)
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
                            orientationEnum orientation = (orientationEnum)SpFunctions.RealSign(Input.GetAxis("Horizontal"));
                            pActions.Turn(orientation);
                            pActions.StartWalking(orientation);

                            #region ObstacleInteraction

                            if (envStats.obstacleness != obstaclenessEnum.noObstcl)
                            {
                                pActions.SetMaxSpeed(pActions.RunSpeed);
                                fastRunInputCount = 0;
                                pActions.StopWalking();
                                pActions.WallInteraction();
                                if (envStats.obstacleness == obstaclenessEnum.lowObstcl)
                                {
                                    if (lowWallCheck.collisions.Count > 0)
                                    {
                                        pActions.AvoidLowObstacle(CheckHeight());
                                        envStats.interaction = interactionEnum.lowEdge;
                                    }
                                }
                                if (envStats.obstacleness == obstaclenessEnum.highObstcl)
                                {
                                    if (Input.GetButtonDown("Jump"))
                                    {
                                        pActions.AvoidHighObstacle(CheckHeight());
                                        envStats.interaction = interactionEnum.edge;
                                    }
                                    if (highWallCheck.collisions.Count > 0)
                                    {
                                        pActions.HangHighObstacle();
                                        envStats.interaction = interactionEnum.edge;
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
                                pActions.SetMaxSpeed(pActions.RunSpeed);
                            }
                            pActions.StopWalking();
                        }

                        if (Input.GetButtonDown("Crouch") && Input.GetButtonDown("Jump"))
                        {
                            if (envStats.groundness == groundnessEnum.grounded)
                            {
                                pActions.Flip();
                            }
                        }
                        else if (Input.GetButtonDown("Jump"))
                        {
                            pActions.Jump();
                        }

                        if (Input.GetButtonDown("Interact"))
                        {
                            Interact(this);
                        }

                        pActions.Crouch((Input.GetButton("Crouch")) || (WallIsAbove()));

                        if ((envStats.groundness == groundnessEnum.inAir) && (rigid.velocity.y < -30f) && (rigid.velocity.y > minLedgeSpeed))
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
                            pActions.Attack();
                        }
                    }

                    #endregion //UsualState

                    #region EdgeState

                    else if (envStats.interaction == interactionEnum.edge)
                    {
                        if (Input.GetButtonDown("Jump") || (rigid.velocity.y > 5f))
                        {
                            pActions.AvoidHighObstacle(CheckHeight());
                        }
                        if (envStats.obstacleness == obstaclenessEnum.noObstcl)
                        {
                            pActions.AvoidHighObstacle(0f);
                            envStats.interaction = interactionEnum.noInter;
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
                        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                        {
                            float valueX = Input.GetAxis("Horizontal");
                            float valueY = Input.GetAxis("Vertical");
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
                        if (Input.GetButtonDown("Interact"))
                        {
                            pActions.Hang(false);
                        }

                        if (envStats.interaction == interactionEnum.platform)
                        {
                            if ((Input.GetButton("Jump")) || (rigid.velocity.y > 5f))
                            {
                                pActions.ClimbOntoThePlatform();
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

                #region CameraMovement

                if (rigid.velocity.magnitude <= 5f)
                {
                    float camValue = Input.GetAxis("CamMove");
                    if (Mathf.Abs(camValue) >= 0.5f)
                    {
                        pActions.Observe(new Vector2(0f, Mathf.Sign(camValue)));
                    }
                    else
                    {
                        pActions.Observe(new Vector2(0f, 0f));
                    }
                }
                else
                {
                    pActions.Observe(new Vector2(0f, 0f));
                }

                #endregion //CameraMovement

                #region ChangeItems

                if (Input.GetButtonDown("ChangeRWeapon"))
                {
                    List<PartController> children = new List<PartController> ();
                    CharacterAnimator anim = GetComponentInChildren<CharacterAnimator>();
                    DeletePart(equip.rightWeapon,children,anim);
                    if ((equip.altRightWeapon != null) && (equip.leftWeapon == equip.rightWeapon))
                    {
                        DeletePart(equip.leftWeapon, children, anim);
                    }    
                    equip.ChangeRightWeapon();
                    AddPart(equip.rightWeapon, children, anim);
                    SetChildren(anim.parts, children);
                }

                if (Input.GetButtonDown("ChangeLWeapon"))
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
                }

                if (Input.GetButtonDown("ChangeItem"))
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
        base.Initialize();
        if (pActions != null)
        {
            pActions.SetWeapon(equip.rightWeapon, "Main");
            pActions.SetWeapon(equip.leftWeapon, "Secondary");
        }
        transform.GetComponentInChildren<CharacterAnimator>().SetStats(envStats);
        aboveWallCheck = transform.FindChild("Indicators").FindChild("AboveWallCheck");
        lowWallCheck = transform.FindChild("Indicators").FindChild("LowWallCheck").gameObject.GetComponent<GroundChecker>();
        frontWallCheck = transform.FindChild("Indicators").FindChild("FrontWallCheck").gameObject.GetComponent<GroundChecker>();
        highWallCheck = transform.FindChild("Indicators").FindChild("HighWallCheck").gameObject.GetComponent<GroundChecker>();
        interactions = transform.FindChild("Indicators").gameObject.GetComponentInChildren<InteractionChecker>();
    }

    /// <summary>
    /// Возвращает инвентарь персонажа
    /// </summary>
    public override BagClass GetEquipment()
    {
        return equip;
    }

    #region Interact

    /// <summary>
    /// Здесь описаны все взаимодействия, что могут пройзойти между персонажем и окружающим миром
    /// </summary>
    public override void Interact(InterObjController interactor)
    {
        if (pActions != null)
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
        if (Physics.Raycast(new Ray(trans.position, (int)direction.dir * trans.right), out hit, doorDistance))
        {
            door = hit.collider.gameObject.GetComponent<DoorClass>();
            if (door != null)
            {
                if (door.locker.opened)
                {
                    currentRoom = door.roomPath;
                    pActions.GoThroughTheDoor(door);
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
                    pActions.GoThroughTheDoor(door);
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
            equip.TakeItem(items[i]);
        }
        interactions.dropList.RemoveAt(0);
        Destroy(drop.gameObject);
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

    protected override void DefinePrecipice()
    {
        pActions.PrecipiceIsForward = (!(Physics.OverlapSphere(precipiceCheck.position, precipiceRadius, whatIsGround).Length > 0) && (envStats.groundness == groundnessEnum.grounded)&&(!Input.GetButton("CamMove")));
    }

    /// <summary>
    /// Функция, учитывающая, находятся ли перед персонажем какие-нибудь препятствия
    /// </summary>
    protected void CheckObstacles()
    {
        if ((lowWallCheck.collisions.Count> 0) &&
            (frontWallCheck.collisions.Count == 0))
        {
            envStats.obstacleness = obstaclenessEnum.lowObstcl;
        }
        else if ((frontWallCheck.collisions.Count> 0) &&
                (highWallCheck.collisions.Count== 0))
        {
            envStats.obstacleness = obstaclenessEnum.highObstcl;
        }
        else if ((frontWallCheck.collisions.Count > 0) &&
            (highWallCheck.collisions.Count > 0))
        {
            envStats.obstacleness = obstaclenessEnum.wall;
        }
        else
        {
            envStats.obstacleness = obstaclenessEnum.noObstcl;
        }
    }

    protected override void DefineInteractable()
    {
        if ((envStats.interaction != interactionEnum.noInter) && (envStats.interaction != interactionEnum.interactive))
        {
            if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("thicket")).Length > 0)
            {
                envStats.interaction = interactionEnum.thicket;
            }
            else if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("rope")).Length > 0)
            {
                envStats.interaction = interactionEnum.rope;
            }
            else if (Physics.OverlapSphere(interCheck.position, interRadius, LayerMask.GetMask("stair")).Length > 0)
            {
                envStats.interaction = interactionEnum.stair;
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

    #region equipment

    /// <summary>
    /// Инициализировать инвентарь 
    /// </summary>
    protected void InitializeEquipment()
    {
        List<PartController> parts = new List<PartController>();
        CharacterAnimator cAnim=GetComponentInChildren<CharacterAnimator>();
        ArmorSet armorSet = equip.armor;
        ConsiderArmor(armorSet.helmet, true);
        ConsiderArmor(armorSet.cuirass, true);
        ConsiderArmor(armorSet.pants, true);
        ConsiderArmor(armorSet.gloves, true);
        ConsiderArmor(armorSet.boots, true);
        if (equip.rightWeapon != null)
        {
            AddPart(equip.rightWeapon,parts, cAnim);
        }
        if (equip.leftWeapon != null)
        {
            AddPart(equip.leftWeapon,parts, cAnim);
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
    public void ChangeItem(ItemClass item, string itemType)
    {
        #region init

        CharacterAnimator anim = GetComponentInChildren<CharacterAnimator>();
        List<PartController> childParts = new List<PartController>();

        List<ItemClass> removeItems = equip.ChangeEquipmentElement(item, itemType);

        #endregion //init
        
        if (!string.Equals(itemType,"rightWeapon2")&&!string.Equals(itemType, "leftWeapon2"))
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

                if (string.Equals(item.type, "weapon"))
                {
                    if (pActions != null)
                    {
                        pActions.SetWeapon(equip.rightWeapon, "Main");
                        pActions.SetWeapon(equip.leftWeapon, "Secondary");
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
    protected void AddPart(ItemClass item, List<PartController> childParts, CharacterAnimator anim)
    {
        if (item != null)
        {
            List<PartController> parts = anim.parts;
            PartController part = null;
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
    }

    /// <summary>
    /// Функция, вызываемая для удаления не использующихся частей
    /// </summary>
    protected void DeletePart(ItemClass item, List<PartController> childParts, CharacterAnimator anim)
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
                if (part!=null)
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
    /// Функция, используемая для установления зависимостей между частями
    /// </summary>
    protected void SetChildren(List<PartController> parts,List<PartController> childParts)
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

}

#if UNITY_EDITOR
/// <summary>
/// Редактор клавиатурного контроллера
/// </summary>
[CustomEditor(typeof(KeyboardActorController))]
public class KeyboardActorEditor : Editor
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
        orgStats.maxHealth = EditorGUILayout.FloatField("Max Health", orgStats.maxHealth);
        orgStats.velocity = EditorGUILayout.FloatField("Velocity", orgStats.velocity);
        EditorGUILayout.FloatField("Health", orgStats.health);
    }
}
#endif