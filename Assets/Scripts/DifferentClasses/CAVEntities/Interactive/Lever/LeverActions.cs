using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// Действия, совершаемые рычагами
/// </summary>
public class LeverActions : InterObjActions
{
    public List<InterObjController> mechs;//Механизмы, на которые воздействует рычаг

    public bool activated;

    private LeverVisual anim;

    public override void Awake()
    {
        base.Awake();
    }

    public virtual void Start()
    {
        if (anim != null)
        {
            if (activated)
            {
                anim.OpenedCondition();
            }
            else
            {
                anim.ClosedCondition();
            }
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
                anim.OpenedCondition();
            }
            activated = true;
        }
    }

    
}
