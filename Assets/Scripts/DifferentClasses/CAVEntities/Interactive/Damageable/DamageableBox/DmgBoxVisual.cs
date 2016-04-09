using UnityEngine;
using System.Collections;

/// <summary>
/// Визуальное оформление ломающегося ящика
/// </summary>
public class DmgBoxVisual : DmgObjVisual
{
    #region consts

    private const float destroyTime = 3f;

    #endregion //consts

    #region fields
    #endregion //fields

    public override void Awake()
    {
        base.Awake();
    }

    #region AnimatedActions

    /// <summary>
    /// визуальный процесс ломания коробки
    /// </summary>
    public void Injured()
    {
        if ((orgStats != null)&&(iAnim!=null))
        {
            float hp = orgStats.health;
            float maxHp = orgStats.maxHealth;
            if (hp >=0.95f * maxHp) { iAnim.Animate("FullHealth"); }
            else  if (hp >=0.8f * maxHp) { iAnim.Animate("DamagedVerySmall"); }
            else if (hp >= 0.6f * maxHp) { iAnim.Animate("DamagedSmall"); }
            else if (hp >= 0.4f * maxHp) { iAnim.Animate("DamagedMedium"); }
            else if (hp >= 0.2f * maxHp) { iAnim.Animate("DamagedLarge"); }
            else { iAnim.Animate("DamagedVeryLarge"); }
        }
    }

    /// <summary>
    /// Вызвать процесс разрушения
    /// </summary>
    public override void Death()
    {
        StartCoroutine(DeathProcess());
    }

    #endregion //AnimatedActions

}
