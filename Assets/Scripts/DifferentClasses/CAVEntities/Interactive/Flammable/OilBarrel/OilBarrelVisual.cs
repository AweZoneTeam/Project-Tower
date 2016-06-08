using UnityEngine;
using System.Collections;

/// <summary>
/// Визуальное оформление бочки с маслом
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
}
