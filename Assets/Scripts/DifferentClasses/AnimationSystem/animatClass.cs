using UnityEngine;
using System.Collections;

/// <summary>
/// Этот класс ранее использовался для описания совершаемых анимаций и условия совершения этих анимаций
/// </summary>
[System.Serializable]
public class AnimConditionClass
{	
	public AnimClass anim;
	public EnvironmentStats stats;
	public int direction;
	public float speedX;
	public float nSpeedX;
	public float speedY;
	public float nSpeedY;
	public int weaponType;
	public bool weaponInRightHand;
}
