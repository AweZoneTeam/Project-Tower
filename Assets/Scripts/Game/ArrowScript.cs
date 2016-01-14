using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, который может управлять стрелой.
/// Он уничтожает стрелу, если она столкнулась с чем-нибудь твёрдым.
/// Он поворачивает стрелу по направлению скорости.
/// </summary>
public class ArrowScript : MonoBehaviour {

	private Rigidbody2D rigid;
	private CollisionRegulation col;
	private HitController hitControl;

	public void Awake () 
	{
		rigid = gameObject.GetComponent<Rigidbody2D>();
		col=GetComponentInChildren<CollisionRegulation>();
		hitControl = GetComponentInChildren<HitController> ();
	}


	public void FixedUpdate () 
	{  
		transform.Rotate (0f, 0f, SpFunctions.RealAngle (SpFunctions.VectorConvert(transform.right),
		                                      rigid.velocity));
		hitControl.actTime++;
		if (col.detector [0])
		{
			hitControl.col.enabled=false;
			Destroy (gameObject,1f);
		}
	}
}
