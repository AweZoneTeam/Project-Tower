using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TorchController : MonoBehaviour {

	public WeaponClass weapon;
	public int[] cautNumb;
	public Light light;
	public Collider2D col;

	
	public void FixedUpdate () {
		bool k=false;
		int i;
		if (weapon.active)
			light.enabled = true;
		else
			light.enabled = true;
		for (i=0; i<cautNumb.Length; i++)
			if (weapon.moveset [cautNumb [i]].chosen)
				k = true;
		col.enabled = k;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<FlameController> () != null)
			other.gameObject.GetComponent<FlameController> ().fired = true;
	}
}
