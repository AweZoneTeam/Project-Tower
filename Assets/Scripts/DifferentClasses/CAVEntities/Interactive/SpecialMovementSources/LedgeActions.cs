﻿using UnityEngine;
using System.Collections;

public class LedgeActions : InterObjActions
{

    /// <summary>
    /// Произвести взаимодействие
    /// </summary>
    public override void Interact()
    {
        if (interactor != null)
        {
            Transform interCheck = interactor.transform.FindChild("Indicators").FindChild("InterCheck");
            if (interCheck != null)
            {
                Transform trans = interactor.transform;
                Vector3 pos = trans.position;
                if (pos.y < transform.position.y)
                {
                    interactor.GetDirection().dir = orientationEnum.right;
                    trans.localScale = new Vector3((trans.localScale.x < 0f ? -1 : 1) * trans.localScale.x, trans.localScale.y, trans.localScale.z);
                    trans.position = new Vector3(pos.x, transform.position.y - (interCheck.transform.position.y - pos.y), pos.z);
                    if (interactor is PersonController)
                    {
                        PersonController person = (PersonController)interactor;
                        EnvironmentStats _stats = person.GetEnvStats();
                        _stats.interaction = interactionEnum.ledge;
                    }
                }
            }
        }

    }

}
