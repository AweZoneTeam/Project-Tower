using UnityEngine;
using System.Collections;

/// <summary>
/// Визуальное оформление бочки с маслом (или динамита)
/// </summary>
public class OilBarrelVisual : InterObjVisual
{
    /// <summary>
    /// Санимировать состояние взрыва
    /// </summary>
    public void Explode()
    {
        iAnim.Animate("Explosion");
    }

    /// <summary>
    /// Санимировать состояние перед взрывом
    /// </summary>
    public void PreBurn()
    {
        iAnim.Animate("PreExplosion");
    }

}
