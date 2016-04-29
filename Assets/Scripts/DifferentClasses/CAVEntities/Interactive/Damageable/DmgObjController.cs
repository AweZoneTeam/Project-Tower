using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Контроллер объекта со здоровьем
/// </summary>
public class DmgObjController : InterObjController
{

    #region consts

    const float dropX1 = -500f;
    const float dropX2 = 500f;
    const float dropY1 = 100f;
    const float dropY2 = 1000f;

    #endregion //consts

    #region fields

    protected DmgObjActions dmgActions;
    protected DmgObjVisual dmgAnim;
    [SerializeField]
    protected OrganismStats orgStats;
    public List<GameObject> dropList=new List<GameObject>();//Какие предметы выпадают из персонажа после его смерти
    public bool death=false;//умер ли персонаж

    #endregion //fields

    public override void Initialize()
    {
        if (direction == null)
        {
            direction = new Direction();
        }
        if (orgStats == null)
        {
            orgStats = new OrganismStats();
        }
        dmgActions = GetComponent<DmgObjActions>();
        if (dmgActions != null)
        {
            SetAction();
        }
        dmgAnim = GetComponentInChildren<DmgObjVisual>();
        if (dmgAnim != null)
        {
            SetVisual();
        }
    }

    protected override void SetAction()
    {
        dmgActions.Initialize();
        dmgActions.SetDirection(direction);
        dmgActions.SetOrgStats(orgStats);
    }

    protected virtual void SetVisual()
    {
        dmgAnim.SetDirection(direction);
        dmgAnim.SetOrgStats(orgStats);
    }

    #region interface

    public virtual void Update()
    {
        if ((orgStats.hitted>0f)&&(orgStats.health >0f))
        {
            Hitted();
        }

        if (orgStats.health <= 0f)
        {
            Death();
        }
    }

    /// <summary>
    /// Эта функция вызывается при нанесении урона
    /// </summary>
    public virtual void Hitted()
    {
        dmgActions.Hitted();
    }

    /// <summary>
    /// Эта функция вызывается при смерти персонажа
    /// </summary>
    public virtual void Death()
    {
        if (!death)
        {
            death = true;
            dmgActions.Death();
        }
    }

    public override void Interact(InterObjController interactor)
    {
        base.Interact(interactor);
    }

    #endregion //interface

    public OrganismStats GetOrgStats()
    {
        if (orgStats == null)
        {
            orgStats = new OrganismStats();
        }
        return orgStats;
    }
}

#if UNITY_EDITOR
/// <summary>
/// Редактор контроллеров объектов со здоровьем
/// </summary>
[CustomEditor(typeof(DmgObjController))]
public class DmgObjEditor : InterObjEditor
{

    private Direction direction;
    private OrganismStats orgStats;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DmgObjController obj = (DmgObjController)target;
        direction = obj.GetDirection();
        orgStats = obj.GetOrgStats();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.EnumMaskField("direction", direction.dir);
        //orgStats.maxHealth = EditorGUILayout.FloatField("Max Health", orgStats.maxHealth);
        //orgStats.health=EditorGUILayout.FloatField("Health", orgStats.health);
        //orgStats.defence.stability = EditorGUILayout.IntField("Stability", orgStats.defence.stability);
        EditorGUILayout.Space();
        //EditorGUILayout.IntField("Hitted", (int)orgStats.hitted);
        //orgStats.microStun = EditorGUILayout.FloatField("MicroStun", orgStats.microStun);
        //orgStats.macroStun = EditorGUILayout.FloatField("MacroStun", orgStats.macroStun);
        //orgStats.defence.pDefence = EditorGUILayout.IntField("Physic Defence", orgStats.defence.pDefence);
        //orgStats.defence.fDefence = EditorGUILayout.IntField("Fire Defence", orgStats.defence.fDefence);
        //orgStats.defence.aDefence = EditorGUILayout.IntField("Poison Defence", orgStats.defence.aDefence);
        //orgStats.defence.dDefence = EditorGUILayout.IntField("Dark Defence", orgStats.defence.dDefence);
    }
}
#endif