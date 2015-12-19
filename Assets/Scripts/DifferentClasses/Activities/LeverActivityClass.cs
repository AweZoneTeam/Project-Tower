using UnityEngine;
using System.Collections;

public class LeverActivityClass : MonoBehaviour 
{
	[System.Serializable]
	public struct activities
	{
		public string name;
		public ActionClass.act[] what;
		public int timeToReverse;
		public int actMode;
		public bool chosen;
		public int timeOfAction;
		public animatClass.animat[] howLook;
		public ComparativeClass.compr activated;
	}

}
