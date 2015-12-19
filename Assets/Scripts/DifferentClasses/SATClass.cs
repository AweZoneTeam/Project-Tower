using UnityEngine;
using System.Collections;

public class SATClass : MonoBehaviour 
{
	[System.Serializable]
	public struct sat
	{
		public bool played;
		public int time;
		public AudioClip[] audios;
	}

}
