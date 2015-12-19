using UnityEngine;
using System.Collections;

public class CollisionRegulation : MonoBehaviour 
{

	public LayerMask[] collisions;
	public string[] name;
	public float[] radius;
	public bool[] detector;

	private SpFunctions sp;

	public void Awake()
	{
		sp = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SpFunctions> ();
	}

	public void FixedUpdate()
	{
		for (int i=0; i<collisions.Length; i++)
			detector [i] = Physics2D.OverlapCircle (sp.VectorConvert (transform.position), radius [i], collisions [i]);
	}

}
