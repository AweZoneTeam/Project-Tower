using UnityEngine;
using System.Collections;

/// <summary>
/// Availible attack type. When adding new one make sure to make MAX last, since it is used to determine number of attack types.
/// </summary>
enum AttackType {
	Fire,
	Poison,
	Prysical,
	MAX
}

public class AttackData {
	public int[] Damage;
	public int Knockback;
	public AttackData(int[] Dam, int Kb) {
		Damage = (int[]) Dam.Clone ();
		Knockback = Kb;
	}
}

