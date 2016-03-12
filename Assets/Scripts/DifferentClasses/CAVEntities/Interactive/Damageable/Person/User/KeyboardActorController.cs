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
        interactions = transform.FindChild("Indicators").gameObject.GetComponentInChildren<InteractionChecker>();
    }

    public override void Update ()
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
        Transform trans = gameObject.transform;
        float zDistance = Mathf.Abs(currentRoom.position.z + currentRoom.size.z / 2 - trans.position.z) - 0.5f;
        RaycastHit hit = new RaycastHit();
        DoorClass door;
        if (Physics.Raycast(new Ray(trans.position, (int)stats.direction * trans.right), out hit, doorDistance))
        {
            door = hit.collider.gameObject.GetComponent<DoorClass>();
            if (door!=null)
            {
                if (door.locker.opened)
                {
                    currentRoom = door.roomPath;
                    actions.GoThroughTheDoor(door);
                }
            }
        }

        if (Physics.Raycast(new Ray(trans.position, Input.GetKey(KeyCode.W)?new Vector3(0f,0f,-1f): new Vector3(0f, 0f, 1f)), out hit, zDistance))
        {
            door = hit.collider.gameObject.GetComponent<DoorClass>();
            if (door!=null)
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


