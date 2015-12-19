using UnityEngine;
using System.Collections;

public class WeaponClass : MonoBehaviour {
	
	public string name;
	public int type;
	public int handEmployment;
	public int orientation;
	public ActivityClass.activites[] moveset;
	public bool active;
	public int ready;

	public PartConroller weaponFx;

	void Awake()
	{
		for (int i=0;i<moveset.Length;i++)
			moveset[i].numb=i;
		if (weaponFx != null)
			weaponFx.isWeaponFx = true;
	}

	void Update () {
	
	}
}
