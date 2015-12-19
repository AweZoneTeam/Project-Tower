using UnityEngine;
using System.Collections;

public class NMoveableObjectController : ColObjectController {
	
	protected CharacterAnimator animator;
	protected SpFunctions sp;
	

	public virtual void Awake () 
	{
		animator = GetComponent<CharacterAnimator> ();
		sp = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<SpFunctions> ();
	}

	public void Update () 
	{
	}
}
