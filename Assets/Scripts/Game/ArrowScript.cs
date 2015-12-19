using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour {

	private Rigidbody2D rigid;
	private SpFunctions sp;
	private CollisionRegulation col;
	private HitController hitControl;

	public void Awake () 
	{
		rigid = gameObject.GetComponent<Rigidbody2D>();
		sp = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SpFunctions> ();
		col=GetComponentInChildren<CollisionRegulation>();
		hitControl = GetComponentInChildren<HitController> ();
	}


	public void FixedUpdate () 
	{  
		transform.Rotate (0f, 0f, sp.RealAngle (sp.VectorConvert(transform.right),
		                                      rigid.velocity));
		hitControl.actTime++;
		if (col.detector [0])
		{
			hitControl.col.enabled=false;
			Destroy (gameObject,1f);
		}
	}
}
