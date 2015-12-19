using UnityEngine;
using System.Collections;

public class StatsClass : MonoBehaviour 
{
	[System.Serializable]

	public struct stats
	{
		public Vector2 currentSpeed;
		public int targetSpeedX;
		public int targetSpeedY;
		public int employment;
		public int groundness;
		public int obstacleness;
		public int interaction;
		public int maxInteraction;
		public int hitted;
		public int upness;
		public int specialness;
	}
}
