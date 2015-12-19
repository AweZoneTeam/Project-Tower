using UnityEngine;
using System.Collections;

public class NewActivityClass : MonoBehaviour {


	public string name;
	public WeaponClass weapon;
	public bool hasOwnActivities;
	[HideInInspector] public int numb;
	public ConditionClass.conditions  why;
	public ActionClass.act[] what;
	public int timeToReverse;
	public ActionClass.act[] whatIf;
	public animatClass.animat[] howLook;
	public StatsClass.stats soWhat;
	public int actMode;
	public int employ;
	public bool chosen;
	public int timeOfAction;
}
