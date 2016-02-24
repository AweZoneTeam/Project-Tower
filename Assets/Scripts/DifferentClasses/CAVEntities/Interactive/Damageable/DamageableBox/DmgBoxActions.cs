using UnityEngine;
using System.Collections;

/// <summary>
/// Список действий, которые могут быть инициированы ломающейся коробкой.
/// </summary>
public class DmgBoxActions : DmgObjActions
{
    #region fields
    private Organism stats;
    private DmgBoxVisual cAnim;
    #endregion //fields

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        cAnim = GetComponentInChildren<DmgBoxVisual>();
    }

    /// <summary>
    /// Как персонаж реагирует на удар
    /// </summary>
    public override void Hitted()
    {
        cAnim.Injured();
    }

    /// <summary>
    /// Процесс смерти персонажа
    /// </summary>
    public override void Death()
    {
        base.Death();
        cAnim.Death();
    }

    public override void SetStats(Organism _stats)
    {
        stats = _stats;
    }

}
