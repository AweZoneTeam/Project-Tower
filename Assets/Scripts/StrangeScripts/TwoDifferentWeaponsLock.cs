using UnityEngine;
using System.Collections;

public class TwoDifferentWeaponsLock : MonoBehaviour {

	public int lWeaponType, rWeaponType;

	private Equipment equip;
	private ActivityClass.activites activity;
	private RootCharacterController controller;
	private WeaponClass weapon;
	private int  numb;
	private SpFunctions sp;

	public void SetEquip(Equipment e)
	{
		sp = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SpFunctions> ();
		equip = e;
	}

	public void SetValues(RootCharacterController c, WeaponClass w, int n)
	{
		weapon = w;
		controller = c;
		numb = n;
		activity = weapon.moveset [numb];
	}

	public void Work()
	{
		int i;
		if ((equip.leftWeapon.type!=lWeaponType)||((equip.rightWeapon.type!=rWeaponType)))
			for (i=0; i<controller.whatToEmploy.Count; i++)
				if ((controller.whatToEmploy [i].numb == numb) && (controller.whatToEmploy [i].weapon == weapon))
					sp.ChangeTimer(1,controller.whatToEmploy [i],controller.actions.activities);
	}
}
