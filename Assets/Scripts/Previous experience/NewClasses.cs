using UnityEngine;
using System.Collections;

public class Foo : MonoBehaviour
{  
	[System.Serializable]
	public struct movement 
	{ 	
		public string name;
		public float mSpeed; 
		public float grav;
		public bool crouching;
		public int timer;
		public int perehodTimer;
		public Vector2 hitBSize;
		public Vector2 hitBCenterR;
		public Vector2 hitBCenterL;
		public int hitBTimer;
		public int hitETimer;
		public bool fallen;
		public float vSpeed;
		public int damage;
		public bool backStab;
		public int backStabKoof;
		public bool rangeAttack;
		public GameObject bullet;
		public Transform gunL;
		public Transform gunR;

	}
}
