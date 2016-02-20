using UnityEngine;
using System.Collections;

public class KeyboardActorController : PersonController
{

    #region parametres
    private const float groundRadius = 0.2f;
    private const float preGroundRadius = 0.7f;
    private const float doorDistance = 4.5f;
    #endregion //parametres

    #region fields
    private Stats stats;//Параметры персонажа
    private EquipmentClass equip;//Экипировка персонажа
    private HumanoidActorActions actions;

    private Transform groundCheck; //Индикатор, оценивающий расстояние до земли
    private InteractionChecker interactions;
    #endregion //fields

    #region variables

    public LayerMask whatIsGround;
    public bool direction;

    #endregion //variables

    #region enums
    public enum groundness {grounded=1,crouch,preGround,inAir};
    public enum maxInteraction { noInter, stairs, rope, thicket, margin, tMargin, edge, mech, NPC };
    #endregion //enums

    //Инициализация полей и переменных
    public override void Awake ()
	{
        base.Awake();
        stats.maxHealth = 100f;
        stats.health = 100f;
	}

    public override void Initialize()
    {
        actions = GetComponent<HumanoidActorActions>();
        if (stats == null)
        {
            stats = new Stats();
        }
        if (actions != null)
        {
            actions.SetStats(stats);
        }
        transform.GetComponentInChildren<CharacterVisual>().SetStats(stats);
        transform.GetComponentInChildren<CharacterAnimator>().SetStats(stats);
        groundCheck = transform.FindChild("Indicators").FindChild("GroundCheck");
        interactions = transform.FindChild("Indicators").gameObject.GetComponentInChildren<InteractionChecker>();
    }

    // Update is called once per frame
    void Update ()
	{
        if (actions != null)
        {
            if (Input.GetKey(KeyCode.D))
            {
                actions.Turn(OrientationEnum.Right);
                actions.StartWalking(OrientationEnum.Right);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                actions.StopWalking();
            }
            if (Input.GetKey(KeyCode.A))
            {
                actions.Turn(OrientationEnum.Left);
                actions.StartWalking(OrientationEnum.Left);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                actions.StopWalking();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                actions.Jump();
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact(this);
        }

        direction = Input.GetKey(KeyCode.W);

        AnalyzeSituation();
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
            else
            {
                DoorInteraction();
            }
        }
    }

    void DoorInteraction()
    {
        Transform trans = gameObject.transform;
        AreaClass area = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameStatisics>().currentArea;
        float zDistance = Mathf.Abs(area.position.z + area.size.z / 2 - trans.position.z) - 0.5f;
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(new Ray(trans.position, stats.direction * trans.right), out hit, doorDistance))
        {
            if (string.Equals(hit.collider.gameObject.tag, Tags.door))
            {
                if (hit.collider.gameObject.GetComponent<DoorClass>().locker.opened)
                {
                    actions.GoThroughTheDoor(hit.collider.gameObject.GetComponent<DoorClass>());
                }
            }
        }

        if (Physics.Raycast(new Ray(trans.position, Input.GetKey(KeyCode.W)?new Vector3(0f,0f,-1f): new Vector3(0f, 0f, 1f)), out hit, zDistance))
        {
            if (string.Equals(hit.collider.gameObject.tag, Tags.door))
            {
                if (hit.collider.gameObject.GetComponent<DoorClass>().locker.opened)
                {
                    actions.GoThroughTheDoor(hit.collider.gameObject.GetComponent<DoorClass>());
                }
            }
        }
    }

    #endregion //Interact

    #region Analyze

    /// <summary>
    /// Оценивает окружающую персонажа обстановку
    /// </summary>
    void AnalyzeSituation()
    {
        CheckGroundness();
    }

    /// <summary>
    /// Функция, определяющая, где персонаж находится по отношению к твёрдой поверхности 
    /// </summary>
    void CheckGroundness()
    {
        if (Physics2D.OverlapCircle(SpFunctions.VectorConvert(groundCheck.position), groundRadius, whatIsGround))
        {
            stats.groundness = (int)groundness.grounded;
        }
        else if (Physics2D.OverlapCircle(SpFunctions.VectorConvert(groundCheck.position), preGroundRadius, whatIsGround))
        {
            stats.groundness = (int)groundness.preGround;
        }
        else
        {
            stats.groundness = (int)groundness.inAir;
        }
    }

    #endregion //Analyze


}

