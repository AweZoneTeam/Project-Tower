using System;
using System.Collections.Generic;

public class SimpleWeapon : WeaponClass
{


	public List<AttackClass> attackData = new List<AttackClass>();


	public ComboClass GetCombo(string hitName)
	{
		for (int i = 0; i < attackData.Count; i++)
		{
			if (string.Equals(attackData[i].attackName, hitName))
			{
				return attackData[i].combo;
			}
		}
		return null;
	}

	public AttackClass GetAttack(string hitName)
	{
		for (int i = 0; i < attackData.Count; i++)
		{
			if (string.Equals(attackData[i].attackName, hitName))
			{
				return attackData[i];
			}
		}
		return null;
	}
}

