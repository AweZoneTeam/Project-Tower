using UnityEngine;
using System.Collections;

/// <summary>
/// Простеёший тип статов. характеризующее ориентацию персонажа
/// </summary>
[System.Serializable]
public class Direction
{
    public orientationEnum dir;

    public Direction()
    {
        dir = orientationEnum.right;
    }
}
