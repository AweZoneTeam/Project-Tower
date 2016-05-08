using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Индикатор, обнаруживающий стены
/// </summary>
public class WallChecker : GroundChecker
{
    private List<string> whatIsGround = new List<string> {"ground", "door", "character"};

    protected override void Initialize()
    {
        foreach (string layer in whatIsGround)
        {
            layers.Add(LayerMask.NameToLayer(layer));
        }
    }
}
