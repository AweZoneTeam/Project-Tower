using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Контроллер персонажей
/// </summary>
public class PersonController : DmgObjController
{

    #region fields
    private Stats stats;
    [SerializeField]protected EquipmentClass equip;//Экипировка персонажа
    private PersonController actions;
    #endregion //fields

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
        EditorGUILayout.IntField("direction", stats.direction);
        stats.maxHealth = EditorGUILayout.FloatField("Max Health", stats.maxHealth);
        EditorGUILayout.FloatField("Health", stats.health);
    }
}
