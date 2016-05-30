using UnityEngine;
using System.Collections;

/// <summary>
/// Скрипт, отвечающий за лестницу
/// </summary>
public class StairActions : InterObjActions
{

    #region parametres

    public float height;

    #endregion //parametres

    /// <summary>
    /// Произвести взаимодействие
    /// </summary>
    public override void Interact()
    {
        if (interactor != null)
        {
            Transform interCheck=interactor.transform.FindChild("Indicators").FindChild("InterCheck");
            Transform trans = interactor.transform;
            Vector3 pos = trans.position;
            if (interCheck!=null)
            {
                interactor.GetDirection().dir = orientationEnum.right;
                trans.localScale = new Vector3((trans.localScale.x < 0f ? -1 : 1) * trans.localScale.x, trans.localScale.y, trans.localScale.z);
                trans.position = new Vector3(transform.position.x - (interCheck.transform.position.x - pos.x), pos.y, pos.z);
                if (interactor is PersonController)
                {
                    PersonController person = (PersonController)interactor;
                    EnvironmentStats _stats = person.GetEnvStats();
                    _stats.interaction = interactionEnum.ladder;
                }
            }
        }

    }

}
