using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, отвечающий за ящик с которым можно взаимодействовать (двигать, залезать)
/// </summary>
public class MoveableBoxActions : InterObjActions
{

    #region consts

    private const float xOffset = 2f;

    #endregion //consts

    public bool empty = false;//Если ящик пустой, внутрь него можно залезть.
    protected Transform parent;//какой объект изначально был родительским по отношению к этому ящику?
    
    /// <summary>
    /// Произвести взаимодействие
    /// </summary>
    public override void Interact()
    {
        if (interactor != null)
        {
            if (interactor is PersonController)
            {
                PersonController person = (PersonController)interactor;
                EnvironmentStats _stats = person.GetEnvStats();
                if (transform.parent == person.transform)
                {
                    if (person.GetEnvStats().groundness == groundnessEnum.grounded)
                    {
                        person.SetInteractionObject(null);
                        transform.SetParent(parent);
                        _stats.interaction = interactionEnum.noInter;
                    }
                }
                else
                {
                    Vector3 pos = transform.localPosition;
                    person.SetInteractionObject(this);
                    transform.SetParent(person.transform);
                    //transform.localPosition = new Vector3((transform.lossyScale.x / 2 + xOffset) * Mathf.Sign(person.transform.lossyScale.x), pos.y, pos.z);
                    _stats.interaction = interactionEnum.interactive;
                }                 
            }
        }
    }

    /// <summary>
    /// Сбросить родительский объект
    /// </summary>
    public void ResetParent()
    {
        transform.SetParent(parent);
    }
}
