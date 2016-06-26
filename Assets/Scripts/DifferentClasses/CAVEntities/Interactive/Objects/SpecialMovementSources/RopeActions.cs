using UnityEngine;
using System.Collections;


/// <summary>
/// Скрипт, отвечающий за верёвку
/// </summary>
public class RopeActions : InterObjActions
{

    #region consts

    protected const float offsetX = -5f;

    #endregion //consts

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
                trans.localScale = new Vector3((trans.localScale.x < 0f ? -1 : 1) * trans.localScale.x, trans.localScale.y, trans.localScale.z);
                interactor.GetDirection().dir = orientationEnum.right;
                trans.position = new Vector3(transform.position.x - (interCheck.transform.position.x - pos.x)+offsetX, pos.y, pos.z);
                if (interactor is PersonController)
                {
                    PersonController person = (PersonController)interactor;
                    EnvironmentStats _stats = person.GetEnvStats();
                    _stats.interaction = interactionEnum.rope;
                }
            }
        }
    }
}