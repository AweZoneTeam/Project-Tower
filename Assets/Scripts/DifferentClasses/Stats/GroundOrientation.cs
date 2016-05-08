using UnityEngine;
using System.Collections;

/// <summary>
/// Специальный класс, использующийся персонажами, которые могут стоять не только на земле (например, пауки способны ползти по стенам (горизонтальным поверхностям))
/// </summary>
[System.Serializable]
public class GroundOrientation
{
    public groundOrientationEnum grOrientation = groundOrientationEnum.down;

    public GroundOrientation()
    {
        grOrientation = groundOrientationEnum.down;
    }

}
