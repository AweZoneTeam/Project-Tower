using UnityEngine;
using System.Collections;

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

    public bool k = false;

    public virtual void Awake()
    {
        Initialize();
    }

    [ExecuteInEditMode]
    public void Update()
    {
        if (k)
        {
            What();
            k = false;
        }
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
            actions.Interact();
    }

}
