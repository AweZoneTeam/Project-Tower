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
    private Organism stats;
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
        if ((stats != null)&&(iAnim!=null))
        {
            float hp = stats.health;
            float maxHp = stats.maxHealth;
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

    IEnumerator DeathProcess()
    {
        if (iAnim != null)
        {
            iAnim.Animate("Death");
            yield return new WaitForSeconds(deathTime);
            iAnim.Animate("Dead");
        }
    }

    #endregion //AnimatedActions

    public override void SetStats(Organism _stats)
    {
        stats = _stats;
    }

}
