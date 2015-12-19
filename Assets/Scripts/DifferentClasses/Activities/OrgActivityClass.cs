using UnityEngine;
using System.Collections;

public class OrgActivityClass : MonoBehaviour 
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


}
