using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Контроллер простейших интерактивных объектов. Они могут стоять на месте и принимать сигналы взаимодействия. 
/// </summary>
public class InterObjController : MonoBehaviour
{
    #region fields
    private Prestats stats;
    private InterObjActions actions;

    //protected InterObjVisual anim;
    #endregion //fields

    public virtual void Awake()
    {
        Initialize();
    }

    public void What()
    {
        stats = new Stats();
    }

    public virtual void Initialize()
    {
        actions = GetComponent<InterObjActions>();
        if (actions != null)
        {
            actions = GetComponent<InterObjActions>();
            actions.SetStats(stats);
        }
        //anim = transform.GetComponentInChildren < InterObjVisual>();
    }

    /// <summary>
    /// Если к такому объекту подойдёт персонаж и будет взаимодействовать, то контроллер предпримет нужные действия
    /// </summary>
    public virtual void Interact(InterObjController interactor)
    {
        if (actions != null)
        {
            actions.SetInteractor(interactor);
            actions.Interact();
        }
    }

    public virtual Prestats GetStats()
    {
        if (stats == null)
        {
            stats = new Prestats();
        }
        return stats;
    }

}

#if UNITY_EDITOR
/// <summary>
/// Редактор контроллеров интерактивных объектов
/// </summary>
[CustomEditor(typeof(InterObjController))]
public class InterObjEditor : Editor
{
    private Prestats stats;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        InterObjController obj = (InterObjController)target;
        stats = obj.GetStats();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.IntField("direction", (int)stats.direction);
    }
}
#endif