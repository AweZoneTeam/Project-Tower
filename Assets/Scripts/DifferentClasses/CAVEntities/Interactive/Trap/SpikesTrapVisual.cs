using UnityEngine;
using System.Collections;

/// <summary>
/// Визуальное оформление ловушки с шипаи
/// </summary>
public class SpikesTrapVisual : InterObjVisual
{

    /// <summary>
    /// Анимировать активацию и деактивацию
    /// </summary>
    public override void Activate(bool active)
    {
        if (active)
        {
            iAnim.Animate("Activate");
        }
        else
        {
            iAnim.Animate("Deactivate");
        }
    }
}
