using UnityEngine;
using System.Collections;

public class PlatformComander : MonoBehaviour 
{
	public Transform platformCheck;

	private BoxCollider2D col;

	void Awake()
	{
		col = GetComponent<BoxCollider2D> ();
	}

	void FixedUpdate()
	{
		if (platformCheck.position.y < transform.position.y) 
			col.isTrigger = true;
		else
			col.isTrigger=false;
	}
}
 