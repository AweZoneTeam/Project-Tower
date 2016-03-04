using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

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
    private DmgObjActions actions;
    private DmgObjVisual anim;
    private Organism stats=new Organism();
    public List<GameObject> dropList=new List<GameObject>();//Какие предметы выпадают из персонажа после его смерти
    protected bool death=false;//умер ли персонаж
    #endregion //fields

    public override void Initialize()
    {
        actions = GetComponent<DmgObjActions>();
        if (actions != null)
        {
            actions.SetStats(stats);
        }
        anim = GetComponentInChildren<DmgObjVisual>();
        if (anim != null)
        {
            anim.SetStats(stats);
        }
    }

    #region interface

    public virtual void FixedUpdate()
    {
        if ((stats.hitted>0)&&(stats.health >0f))
        {
            Hitted();
        }

        if (stats.health <= 0f)
        {
            Death();
        }
    }

    /// <summary>
    /// Эта функция вызывается при нанесении урона
    /// </summary>
    public virtual void Hitted()
    {
        actions.Hitted();
    }

    /// <summary>
    /// Эта функция вызывается при смерти персонажа
    /// </summary>
    public virtual void Death()
    {
        if (!death)
        {
            death = true;
            actions.Death();
        }
    }

    public override void Interact(InterObjController interactor)
    {
        base.Interact(interactor);
    }
    #endregion //interface

    public override Prestats GetStats()
    {
        if (stats == null)
        {
            stats = new Organism();
        }
        return stats;
    }
}

/// <summary>
/// Редактор контроллеров объектов со здоровьем
/// </summary>
[CustomEditor(typeof(DmgObjController))]
public class DmgObjEditor : InterObjEditor
{
    private Organism stats;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DmgObjController obj = (DmgObjController)target;
        stats = (Organism)obj.GetStats();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.IntField("direction", (int)stats.direction);
        stats.maxHealth = EditorGUILayout.FloatField("Max Health", stats.maxHealth);
        stats.health=EditorGUILayout.FloatField("Health", stats.health);
        stats.stability = EditorGUILayout.IntField("Stability", stats.stability);
        EditorGUILayout.Space();
        EditorGUILayout.IntField("Hitted", (int)stats.hitted);
        stats.microStun = EditorGUILayout.FloatField("MicroStun", stats.microStun);
        stats.macroStun = EditorGUILayout.FloatField("MacroStun", stats.macroStun);
        stats.pDefence = EditorGUILayout.IntField("Physic Defence", stats.pDefence);
        stats.fDefence = EditorGUILayout.IntField("Fire Defence", stats.fDefence);
        stats.aDefence = EditorGUILayout.IntField("Poison Defence", stats.aDefence);
        stats.dDefence = EditorGUILayout.IntField("Dark Defence", stats.dDefence);
    }
}