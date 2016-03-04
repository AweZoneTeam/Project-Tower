using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class KeyboardActorController : PersonController
{

    #region consts
    private const float doorDistance = 4.5f;
    #endregion //consts

    #region fields
    private HumanoidActorActions actions;
    
    private InteractionChecker interactions;
    #endregion //fields

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
            actions.SetWeapon(equip.rightWeapon);
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
                actions.Turn(orientationEnum.right);
                actions.StartWalking(orientationEnum.right);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                actions.StopWalking();
            }
            if (Input.GetKey(KeyCode.A))
            {
                actions.Turn(orientationEnum.left);
                actions.StartWalking(orientationEnum.left);
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

        if (interactions.dropList.Count > 0)
        {
            if (interactions.dropList[0].autoPick)
            {
                TakeDrop();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact(this);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            actions.Attack();
        }

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

    void DoorInteraction()
    {
        Transform trans = gameObject.transform;
        AreaClass area = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameStatisics>().currentArea;
        float zDistance = Mathf.Abs(area.position.z + area.size.z / 2 - trans.position.z) - 0.5f;
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(new Ray(trans.position, (int)stats.direction * trans.right), out hit, doorDistance))
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

    #endregion //Interact

    #region Analyze

    /// <summary>
    /// Оценивает окружающую персонажа обстановку
    /// </summary>
    protected override void AnalyzeSituation()
    {
        base.AnalyzeSituation();
    }

    #endregion //Analyze


}

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
        stats.maxHealth = EditorGUILayout.FloatField("Max Health", stats.maxHealth);
        EditorGUILayout.FloatField("Health", stats.health);
    }
}


