using UnityEngine;
using System.Collections;

public class DmgObjVisual : InterObjVisual
{

    #region timers

    protected float deathTime = 3f;

    #endregion //timers

    #region fields
    #endregion //fields

    #region parametres

    protected bool death = false;

    #endregion //parametres

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    #region AnimatedActionsInterface

    /// <summary>
    /// Вызвать процесс разрушения
    /// </summary>
    public virtual void Death()
    {
        StartCoroutine(DeathProcess());
    }

    protected virtual IEnumerator DeathProcess()
    {
        if (iAnim != null)
        {
            death = true;
            iAnim.Animate("Death");
            yield return new WaitForSeconds(deathTime);
            iAnim.Animate("Dead");
        }
    }

    #endregion //AnimatedActionsInterface

    public virtual void SetStats(OrganismStats _stats)
    {
        
    }
}
