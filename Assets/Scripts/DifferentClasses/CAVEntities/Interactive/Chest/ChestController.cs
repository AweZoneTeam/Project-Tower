using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChestController : InterObjController
{

    #region fields
    [SerializeField]private BagClass chestContent;
    #endregion //fields

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void SetAction()
    {
        base.SetAction();
        intActions.SetBag(chestContent);
    }

    public override void Interact(InterObjController interactor)
    {
        if (intActions != null)
        {
            intActions.SetInteractor(interactor);
            intActions.Interact();
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
    private Direction direction;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ChestController obj = (ChestController)target;
        direction = obj.GetDirection();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.EnumMaskField("direction", direction.dir);
    }
}
#endif