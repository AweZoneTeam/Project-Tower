using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, сожержащий в себе данные об атаке
/// </summary>
[System.Serializable]
public class Damage
{
	public float pDamage, fDamage, dDamage, aDamage; //физический, огненный, тёмный, ядовитый уроны соответственно
	public int attack;//насколько данная атака пробивает стабилити
	public int direction;//Куда направлена атака? 0-отталкивает вперёд, 1-подбрасывает вверх, 2-сбивает с ног, 3-оглушает
	public bool backStab; //учитывает ли данная атака, что цель находится спиной к атаке?
	public float backStabKoof;//во сколько увеличивается урон, если удар нанесён со спины? 
	
	public Damage(float _pDamage, float _fDamage, float _dDamage, float _aDamage,
	                   int _attack,
				   	   int _direction,
	                   bool _backStab,
	                   float _backStabKoof)
	{
		pDamage = _pDamage;
		fDamage = _fDamage;
		dDamage = _dDamage;
		aDamage = _aDamage;
		attack = _attack;
		direction = _direction;
		backStab = _backStab;
		backStabKoof = _backStabKoof;

	}

}
