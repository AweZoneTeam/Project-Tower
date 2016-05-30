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
    /// Анимировать активацию или деактивацию
    /// </summary>
    /// <param name="active"></param>
    public override void Activate(bool active)
    {
        if (active)
        {
            StartCoroutine(ActivationLever());
        }
        else
        {
            if (iAnim != null)
            {
                iAnim.Animate("Deactivated");
            }
        }
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