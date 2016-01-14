using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Аниматор главного героя. Он говорит частям тела персонажа, как им надо двигаться.
/// </summary>
public class CharacterAnimator : BaseAnimator
{
	public List<PartController> parts=new List<PartController>();
	public VisualData visualData;

	public List<animList> animTypes=new List<animList>();

	public void Update()
	{
		SpFunctions.AnimateIt (parts, 
		                   anim);
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