using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Equipment : MonoBehaviour 
{
	public WeaponClass leftWeapon, rightWeapon;
	public GameObject defLeftWeapon, defRightWeapon;

	public List<GameObject> bag;
	public GameObject arrows;

	public bool battleStance;
	void Awake() 
	{
		if (leftWeapon == null)
			if (defLeftWeapon!=null)
				leftWeapon = defLeftWeapon.GetComponent<WeaponClass>();
		if (rightWeapon ==null)
			if (defRightWeapon!=null)
				rightWeapon = defRightWeapon.GetComponent<WeaponClass>();

	}

	void FixedUpdate () 
	{
	}
}
