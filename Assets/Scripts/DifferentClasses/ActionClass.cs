using UnityEngine;
using System.Collections;

public class ActionClass : MonoBehaviour 
{
	[System.Serializable]
	public struct act
	{
		public int actType;
		public int actNumb;
		public int numb;
		public int[] PRM;
		public GameObject[] OBJ;
		public string[] OBJDescription;
	}
}
