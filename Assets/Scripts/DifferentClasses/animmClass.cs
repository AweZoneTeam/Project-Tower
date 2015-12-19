using UnityEngine;
using System.Collections;

public class animmClass : MonoBehaviour 
{
	[System.Serializable]
	public struct animm
	{
		public string rsequence;
		public string lsequence;
		public SATClass.sat[] rsats;
		public TAOClass.tao[] taos, ltaos;
		public bool loop;
		public bool notWeaponMove;
		public bool stepByStep;
		public bool stopStepByStep;
		public int FPS;
	}
}
