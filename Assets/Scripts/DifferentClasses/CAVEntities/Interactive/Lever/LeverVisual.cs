using UnityEngine;
using System.Collections;

public class LeverVisual : InterObjVisual
{
    #region consts

    private const float activationTime = 3f;

    #endregion //consts

    #region fields
    #endregion //fields

    public override void Awake()
    {
        base.Awake();
    }

    #region AnimatedActions
    /// <summary>
    /// Анимировать неактивность
    /// </summary>
    public void ClosedCondition()
    {
        if (iAnim != null)
        {
            iAnim.Animate("Deactivated");
        }
    }

    /// <summary>
    /// Анимировать процесс нажатия
    /// </summary>
    public void OpenedCondition()
    {
        StartCoroutine(ActivationLever());
    }

    IEnumerator ActivationLever()//Процесс нажатия на рычаг
    {
        if (iAnim != null)
        {
            iAnim.Animate("Activation");
            yield return new WaitForSeconds(activationTime);
            iAnim.Animate("Activated");
        }
    }
    #endregion //AnimatedActions
}