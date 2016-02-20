using UnityEngine;
using System.Collections;

/// <summary>
/// Контроллер объекта со здоровьем
/// </summary>
public class DmgObjController : InterObjController
{

    #region fields
    [SerializeField]
    private Organism stats;
    #endregion //fields

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Interact(InterObjController interactor)
    {
        base.Interact(interactor);
    }
}
