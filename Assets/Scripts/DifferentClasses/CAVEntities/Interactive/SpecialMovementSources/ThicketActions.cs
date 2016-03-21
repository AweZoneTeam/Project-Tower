using UnityEngine;
using System.Collections;

/// <summary>
/// Скрипт,управляющий зарослями, на которые можно взобраться
/// </summary>
public class ThicketActions : InterObjActions
{

    #region consts

    protected const float del = 0.2f;

    protected const float radius = 0.1f;

    #endregion //consts

    #region parametres

    protected LayerMask whatIsthicket = LayerMask.GetMask("thicket");

    #endregion //parametres

    /// <summary>
    /// Произвести взаимодействие
    /// </summary>
    public override void Interact()
    {
        if (interactor != null)
        {
            Transform interCheck = interactor.transform.FindChild("Indicators").FindChild("InterCheck");
            Transform trans = interactor.transform;
            Vector3 pos = trans.position;
            if (interCheck != null)
            {
                if (Physics.OverlapSphere(interCheck.position, radius, LayerMask.GetMask("thicket")).Length > 0)
                {
                    trans.localScale = new Vector3((trans.localScale.x < 0f ? -1 : 1) * trans.localScale.x, trans.localScale.y, trans.localScale.z);
                    interactor.GetStats().direction = orientationEnum.right;
                    if (interactor is PersonController)
                    {
                        PersonController person = (PersonController)interactor;
                        Stats _stats = (Stats)person.GetStats();
                        _stats.interaction = interactionEnum.thicket;
                    }
                }
            }
        }
    }
}
