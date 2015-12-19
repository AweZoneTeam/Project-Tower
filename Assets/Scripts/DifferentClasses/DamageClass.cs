using UnityEngine;
using System.Collections;

public class Damage:MonoBehaviour 
{
	public float pDamage, fDamage, dDamage, aDamage; //физический, огненный, тёмный, ядовитый уроны соответственно
	public int attack;//насколько данная атака пробивает стабилити
	public bool backStab; //учитывает ли данная атака, что цель находится спиной к атаке?
	public float backStabKoof;//во сколько увеличивается урон, если удар нанесён со спины? 
	
	public Damage(float _pDamage, float _fDamage, float _dDamage, float _aDamage,
	                   int _attack,
	                   bool _backStab,
	                   float _backStabKoof)
	{
		pDamage = _pDamage;
		fDamage = _fDamage;
		dDamage = _dDamage;
		aDamage = _aDamage;
		attack = _attack;
		backStab = _backStab;
		backStabKoof = _backStabKoof;

	}

}
