using UnityEngine;
using System.Collections;

public class LeverVisual : InterObjVisual
{
    #region consts

    private const float activationTime = 3f;

    #endregion //consts

    #region fields
    private InterObjAnimator cAnim;
    #endregion //fields


    public override void Initialize()
    {
        cAnim = GetComponent<InterObjAnimator>();
    }

    public override void Awake()
    {
        Initialize();
    }

    #region AnimatedActions
    /// <summary>
    /// Анимировать неактивность
    /// </summary>
    public void ClosedCondition()
    {
        if (cAnim != null)
        {
            cAnim.Animate("Deactivated");
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
        if (cAnim != null)
        {
            cAnim.Animate("Activation");
            yield return new WaitForSeconds(activationTime);
            cAnim.Animate("Activated");
        }
    }
    #endregion //AnimatedActions
}
