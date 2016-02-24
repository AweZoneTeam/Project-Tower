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
        if ((stats != null)&&(cAnim!=null))
        {
            float hp = stats.health;
            float maxHp = stats.maxHealth;
            if (hp >=0.95f * maxHp) { cAnim.Animate("FullHealth"); }
            else  if (hp >=0.8f * maxHp) { cAnim.Animate("DamagedVerySmall"); }
            else if (hp >= 0.6f * maxHp) { cAnim.Animate("DamagedSmall"); }
            else if (hp >= 0.4f * maxHp) { cAnim.Animate("DamagedMedium"); }
            else if (hp >= 0.2f * maxHp) { cAnim.Animate("DamagedLarge"); }
            else { cAnim.Animate("DamagedVeryLarge"); }
        }
    }

    /// <summary>
    /// Вызвать процесс разрушения
    /// </summary>
    public void Death()
    {
        StartCoroutine(DeathProcess());
    }

    IEnumerator DeathProcess()
    {
        if (cAnim != null)
        {
            cAnim.Animate("Death");
            yield return new WaitForSeconds(destroyTime);
            cAnim.Animate("Dead");
        }
    }

    #endregion //AnimatedActions

    public override void SetStats(Organism _stats)
    {
        stats = _stats;
    }

}
