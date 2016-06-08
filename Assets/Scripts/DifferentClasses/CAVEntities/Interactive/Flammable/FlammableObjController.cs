using UnityEngine;
using System.Collections;


/// <summary>
/// Контроллер поджигаемых объектов.
/// </summary>
public class FlammableObjController : InterObjController
{

    #region fields

    protected bool burned = false;//Подожжён ли объект

    #endregion //fields

    /// <summary>
    /// Функция, ответственная за поджог
    /// </summary>
    public virtual void OnTriggerEnter(Collider other)
    {
        if (!burned)
        {
            HitController hitControl;
            if ((hitControl = other.GetComponent<HitController>()) != null)
            {
                if (hitControl.hitData != null ? hitControl.hitData.fDamage > 0 : false)
                {
                    MakeItBurn(other.transform.position);
                }
            }
        }
    }

    /// <summary>
    /// Заставить объект гореть
    /// </summary>
    public virtual void MakeItBurn()
    {
        burned = true;
        if ((FlammableActions)intActions != null)
        {
            ((FlammableActions)intActions).BurnAction();
        }
    }

    /// <summary>
    /// Заставить объект гореть
    /// </summary>
    public virtual void MakeItBurn(Vector3 flamePosition)
    {
        burned = true;
        if ((FlammableActions)intActions != null)
        {
            ((FlammableActions)intActions).BurnAction(flamePosition);
        }
    }

}
