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

    protected Direction direction;
    protected InterObjActions intActions;

    [SerializeField]
    protected string info;//Информация о том, как взаимодействовать с данным объектом.
    public string Info { get { return info; } }

    //protected InterObjVisual anim;
    #endregion //fields

    public virtual void Awake()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        if (direction == null)
        {
            direction = new Direction();
            direction.dir = (orientationEnum)SpFunctions.RealSign(transform.localScale.x);
        }
        intActions = GetComponent<InterObjActions>();
        if (intActions != null)
        {
            SetAction();
        }
        //anim = transform.GetComponentInChildren < InterObjVisual>();
    }

    protected virtual void SetAction()
    {
        intActions.Initialize();
        intActions.SetDirection(direction);
    }

    /// <summary>
    /// Если к такому объекту подойдёт персонаж и будет взаимодействовать, то контроллер предпримет нужные действия
    /// </summary>
    public virtual void Interact(InterObjController interactor)
    {
        if (intActions != null)
        {
            intActions.SetInteractor(interactor);
            intActions.Interact();
        }
    }

    public Direction GetDirection()
    {
        if (direction == null)
        {
            direction = new Direction();
        }
        return direction;
    }

}

#if UNITY_EDITOR
/// <summary>
/// Редактор контроллеров интерактивных объектов
/// </summary>
[CustomEditor(typeof(InterObjController))]
public class InterObjEditor : Editor
{
    private Direction direction;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        InterObjController obj = (InterObjController)target;
        direction = obj.GetDirection();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.EnumMaskField("direction", direction.dir);
    }
}
#endif