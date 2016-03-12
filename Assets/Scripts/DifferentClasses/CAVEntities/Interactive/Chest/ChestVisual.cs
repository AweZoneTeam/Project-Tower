using UnityEngine;
using System.Collections;

public class ChestVisual : InterObjVisual
{
    #region consts

    private const float openTime = 3f;

    #endregion //consts

    #region fields
    #endregion //fields

    public override void Awake()
    {
        base.Awake();
    }

    #region AnimatedActions
    /// <summary>
    /// Анимировать закрытие
    /// </summary>
    public void ClosedCondition()
    {
        if (iAnim != null)
        {
            iAnim.Animate("Closed");
        }
    }

    /// <summary>
    /// Анимировать процесс открытия
    /// </summary>
    public void OpenedCondition()
    {
        StartCoroutine(OpenChest());
    }

    IEnumerator OpenChest()//Процесс открытия сундука
    {
        if (iAnim != null)
        {
            iAnim.Animate("Open");
            yield return new WaitForSeconds(openTime);
            iAnim.Animate("Opened");
        }
    }
    #endregion //AnimatedActions
}
