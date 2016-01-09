using UnityEngine;
using System.Collections;

[System.Serializable]
public class OrgActivityClass
{
	[System.Serializable]
	public struct activities
	{
		public string name;
		[HideInInspector] public int numb;
		public OrganismCondition.organConditions why;
		public ActionClass.act[] what;
		public int timeToReverse;
		public int actMode;
		public bool chosen;
		public int timeOfAction;
		public animatClass.animat[] howLook;
	}
	public string name;
	[HideInInspector] public int numb;
	public OrganismCondition.organConditions why;
	public ActionClass.act[] what;
	public int timeToReverse;
	public int actMode;
	public bool chosen;
	public int timeOfAction;
	public animatClass.animat[] howLook;

}
