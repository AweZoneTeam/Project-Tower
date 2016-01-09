using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterAnimator : MonoBehaviour 
{
	public List<PartConroller> parts=new List<PartConroller>();
	public VisualData visualData;

	public List<animList> animTypes=new List<animList>();

	private SpFunctions sp;

	public void Awake()
	{
		sp=GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SpFunctions> ();
	}

	public void Update()
	{
		sp.AnimateLedGafs (parts, 
		                   (int)parts [0].mov.getCurrentFrameNumber () - (int)parts [0].mov.currentSequence.startFrame);
	}


}

[System.Serializable]
public class animList
{
	public string typeName;
	public List<string>	animations=new List<string>();

	public animList (string name, string animName)
	{
		animations=new List<string>();
		typeName=name;
		animations.Add (animName);
	}
}