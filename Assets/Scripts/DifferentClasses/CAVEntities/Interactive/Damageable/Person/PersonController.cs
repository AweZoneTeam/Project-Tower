using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Контроллер персонажей
/// </summary>
public class PersonController : DmgObjController
{

    #region consts
    protected const float groundRadius = 0.2f;
    protected const float preGroundRadius = 0.7f;
    #endregion //consts

    #region fields
    /*private*/protected Stats stats;
    [SerializeField]protected EquipmentClass equip;//Экипировка персонажа
    private PersonController actions;

    protected Transform groundCheck; //Индикатор, оценивающий расстояние до земли
    #endregion //fields

    #region parametres

    public LayerMask whatIsGround;

    #endregion //parametres

    #region Interface

    public virtual EquipmentClass GetEquipment()
    {
        return equip;
    }

    public override Prestats GetStats()
    {
        if (stats == null)
        {
            stats = new Stats();
        }
        return stats;
    }
    
    //Функция, что анализирует обстановку, окружающую персонажа
    protected virtual void AnalyzeSituation()
    {
        DefineGroundness();
    }

    /// <summary>
    /// Функция, определяющая, где персонаж находится по отношению к твёрдой поверхности 
    /// </summary>
    protected virtual void DefineGroundness()
    {
        if (Physics2D.OverlapCircle(SpFunctions.VectorConvert(groundCheck.position), groundRadius, whatIsGround))
        {
            stats.groundness = groundnessEnum.grounded;
        }
        else if (Physics2D.OverlapCircle(SpFunctions.VectorConvert(groundCheck.position), preGroundRadius, whatIsGround))
        {
            stats.groundness = groundnessEnum.preGround;
        }
        else
        {
            stats.groundness = groundnessEnum.inAir;
        }
    }

    #endregion //Interface
}

/// <summary>
/// Редактор контроллеров персонажей
/// </summary>
[CustomEditor(typeof(PersonController))]
public class PersonEditor : DmgObjEditor
{
    private Stats stats;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PersonController obj = (PersonController)target;
        stats = (Stats)obj.GetStats();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.IntField("direction", (int)stats.direction);
        stats.maxHealth = EditorGUILayout.FloatField("Max Health", stats.maxHealth);
        EditorGUILayout.FloatField("Health", stats.health);
    }
}
