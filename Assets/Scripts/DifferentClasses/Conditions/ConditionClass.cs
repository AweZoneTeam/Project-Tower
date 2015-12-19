using UnityEngine;
using System.Collections;

public class ConditionClass : MonoBehaviour
{
	[System.Serializable]
	public struct conditions
	{
		public ClaveNoteClass1.CLV claves;
		public float speedX;
		public float speedY;
		public int direction;
		public int employment;
		public int groundness;
		public int obstacleness;
		public ComparativeClass.compr maxinteraction;
		public ComparativeClass.compr interaction;
		public ComparativeClass.compr weaponReady;
		public int hitted;
		public int upness;
		public int specialness;
	}
}
