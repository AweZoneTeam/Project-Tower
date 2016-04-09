using UnityEngine;
using System.Collections;

/// <summary>
/// Список действий, которые могут быть инициированы ломающейся коробкой.
/// </summary>
public class DmgBoxActions : DmgObjActions
{
    #region fields

    #endregion //fields

    public override void Initialize()
    {
        dmgAnim = GetComponentInChildren<DmgBoxVisual>();
    }

    /// <summary>
    /// Как персонаж реагирует на удар
    /// </summary>
    public override void Hitted()
    {
        dmgAnim.Injured();
    }

    /// <summary>
    /// Процесс смерти персонажа
    /// </summary>
    public override void Death()
    {
        base.Death();
        dmgAnim.Death();
    }

}
