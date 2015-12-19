using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;


public class LeftAnimator : EditorWindow
{
	public List<GameObject> aCharacters;
	public Rect windowRect2 = new Rect(100f, 100f, 200f, 200f);
	public Rect windowRect1 = new Rect(100f, 100f, 200f, 200f);
	public string[] lOptions ={"new","open","close"};
	public string orientation = "Left";
	public int index=0;
	public bool depended=false;
	public int sound, layer=0;
	public bool loop=false;
	public int numb=0, type=0, number=0;
	public string animName="anim ame";

	void OnGUI () 
	{
		EditorGUILayout.BeginVertical ();
		{
			EditorGUILayout.Popup(index,lOptions);
			EditorGUILayout.Space ();
			EditorGUILayout.TextField("Name");
			EditorGUILayout.TextField("Animation Name");
			EditorGUILayout.Space();
			PartParamWindow();
			EditorGUILayout.Space();
			AnimationParamWindow();
			if(GUILayout.Button("Reverse"))
			{
				Debug.Log ("Reverse");
			}
		}
		EditorGUILayout.EndVertical ();
		if (GUI.changed) 
		{

		}
	}
		

	void PartParamWindow() 
	{
		EditorGUILayout.BeginVertical ();
		{
			EditorGUILayout.TextField("Part parametres");
			if(GUILayout.Button(orientation))
			{
				ChangeAnimaionOrientation();
			}
			depended=EditorGUILayout.Toggle("depend",depended);
			sound=EditorGUILayout.IntField("sound",sound);
			layer=EditorGUILayout.IntField("layer",layer);
		}
		EditorGUILayout.EndVertical ();
	}

	void AnimationParamWindow () 
	{
		EditorGUILayout.BeginVertical ();
		{
			EditorGUILayout.TextField("Animation parametres");
			loop=EditorGUILayout.Toggle("loop",loop);
			number=EditorGUILayout.IntField("number", number);
			type=EditorGUILayout.IntField("type", type);
			animName=EditorGUILayout.TextField(animName);
		}
		EditorGUILayout.EndVertical();

	}

	void ChangeAnimaionOrientation()
	{
		if (string.Equals(orientation,"Right"))
			orientation="Left";
		else 
			orientation="Right";
	}
}
