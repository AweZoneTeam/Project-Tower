using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// Действия, совершаемые рычагами
/// </summary>
public class LeverActions : MechanismActions
{

    #region parametres

    public bool oneTime=false;//Если true, то рычаг используется один раз

    #endregion //parametres

    #region fields

    public List<InterObjController> mechs;//Механизмы, на которые воздействует рычаг

    #endregion //fields

    public virtual void Start()
    {
        if (anim != null)
        {
            anim.Activate(activated);
        }
    }

    public override void Initialize()
    {
        anim = GetComponentInChildren<LeverVisual>();
    }

    public override void Interact()
    {
        if (!activated)
        {
            for (int i = 0; i < mechs.Count; i++)
            {
                ExecuteEvents.Execute<ILeverActivated>(mechs[i].gameObject, null, (x, y) => x.LeverActivation());
            }
            if (anim != null)
            {
                anim.Activate(true);
            }
            activated = true;
        }
        else if ((activated)&&(!oneTime))
        {
            for (int i = 0; i < mechs.Count; i++)
            {
                ExecuteEvents.Execute<ILeverActivated>(mechs[i].gameObject, null, (x, y) => x.LeverActivation());
            }
            if (anim != null)
            {
                anim.Activate(false);
            }
            activated = false;
        }
    }
}
