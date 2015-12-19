using UnityEngine;
using System.Collections;

public class InfoGetClass : MonoBehaviour 
{
	[System.Serializable]
	public struct infoGet
	{
		public string name;
		public StatsClass.stats stats, elseStats;
		public int typeOfInfo;
		public int numbOfInfo;
		public GameObject[] OBJ;
		public int [] PRM;
		public float[] PRM2;
		public Vector2[] VCT;
		public LayerMask LYR;
		public string[] TGS;
	}
}
