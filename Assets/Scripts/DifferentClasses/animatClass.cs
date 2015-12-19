using UnityEngine;
using System.Collections;

public class animatClass : MonoBehaviour 
{	
	[System.Serializable]
	public struct animat
	{
		public animClass.anim anim;
		public StatsClass.stats stats;
		public int direction;
		public float speedX;
		public float nSpeedX;
		public float speedY;
		public float nSpeedY;
		public int weaponType;
		public bool weaponInRightHand;
	}
}
