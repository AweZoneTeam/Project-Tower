using UnityEngine;
using System.Collections;

public class ChestController : InterObjController
{

    #region fields
    public BagClass chestContent;
    [SerializeField]private Prestats stats;
    [SerializeField]private ChestActions actions;
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
