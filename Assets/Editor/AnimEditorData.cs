using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AnimEditorData : ScriptableObject 
{
	public List<string> animations = new List<string>();
	public List<string> parts = new List<string>();
	public List<GameObject> aCharacters = new List<GameObject>();
}
