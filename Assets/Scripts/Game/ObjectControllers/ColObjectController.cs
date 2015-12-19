using UnityEngine;
using System.Collections;

public class ColObjectController : ObjectController {
	public Collider2D col;

	public void ReviseCollider()
	{
		if (gameObject.GetComponent<Collider2D> () == null)
			Destroy (gameObject);
	}

	public virtual void Awake () {
	}

	public void Update () 
	{
		ReviseCollider ();	
	}
}
