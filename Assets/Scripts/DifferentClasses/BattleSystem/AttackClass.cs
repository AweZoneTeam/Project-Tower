using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AttackClass : ActionClass
{
	public groundnessEnum groundnessState;
	public string attackName;
	public ComboClass combo;
	public ChargeAttack chargeAttack;
}
