using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterAnimator : MonoBehaviour 
{
	public List<PartConroller> headParts;
	public List<PartConroller> ledParts;
	public List<PartConroller> allParts;

	private SpFunctions sp;

	public void Awake()
	{
		sp=GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SpFunctions> ();
	}

	public void Update()
	{
		sp.AnimateLedGafs (
							ledParts, 
		                   (int)headParts [0].mov.getCurrentFrameNumber () - (int)headParts [0].mov.currentSequence.startFrame);
	}
}

