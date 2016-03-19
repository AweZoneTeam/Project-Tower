using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChestController : InterObjController
{

    #region fields
    [SerializeField]private BagClass chestContent;
    private Prestats stats;
    private ChestActions actions;
    #endregion //fields

    public override void Initialize()
    {
        actions = GetComponent<ChestActions>();
        if (actions != null)
        {
            actions.SetStats(stats);
            actions.SetBag(chestContent);
        }
    }

    public override void Interact(InterObjController interactor)
    {
        if (actions != null)
        {
            actions.SetInteractor(interactor);
            actions.Interact();
        }
    }
}

#if UNITY_EDITOR
/// <summary>
/// Редактор сундуков и их содержимого
/// </summary>
[CustomEditor(typeof(ChestController))]
public class ChestEditor : InterObjEditor
{
    private Prestats stats;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ChestController obj = (ChestController)target;
        stats = obj.GetStats();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.IntField("direction", (int)stats.direction);
    }
}
#endif