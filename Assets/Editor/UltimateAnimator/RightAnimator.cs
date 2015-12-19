using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class RightAnimator : EditorWindow
{
	public List<string> animations = new List<string>();
	public List<string> parts = new List<string>();
	public List<GameObject> aCharacters = new List<GameObject>();
	public Rect windowRect1 = new Rect(100f, 100f, 200f, 200f);
	public Rect windowRect2 = new Rect(100f, 100f, 200f, 200f);
	public Rect windowRect3 = new Rect(100f, 100f, 200f, 200f);

	void OnGUI () 
	{
		EditorGUILayout.BeginVertical ();
		{
			CharactersList();
			EditorGUILayout.Space();
			AnimationList();
			EditorGUILayout.Space();
			PartList();
		}
		EditorGUILayout.EndVertical ();
	}
	
	
	void PartList() 
	{
		EditorGUILayout.BeginVertical ();
		{
			EditorGUILayout.TextField("Parts");
			EditorGUILayout.Space ();
			for (int i=0;i<parts.Count;i++)
				EditorGUILayout.LabelField(parts[i]);
			if(GUILayout.Button("+Add"))
			{
				Debug.Log ("Добавить часть");
			}

		}
		EditorGUILayout.EndVertical();
	}
	
	void AnimationList () 
	{
		EditorGUILayout.BeginVertical ();
		{
			EditorGUILayout.TextField("Animations");
			EditorGUILayout.Space ();
			for (int i=0;i<animations.Count;i++)
				EditorGUILayout.LabelField(animations[i]);
			if(GUILayout.Button("+Add"))
			{
				Debug.Log ("Добавить анимацию");
			}
			
		}
		EditorGUILayout.EndVertical();	
	}
	
	void CharactersList () 
	{
		EditorGUILayout.BeginVertical ();
		{
			EditorGUILayout.TextField("Characters");
			EditorGUILayout.Space ();
			for (int i=0;i<aCharacters.Count;i++)
				EditorGUILayout.LabelField(aCharacters[i].name);	
			EditorGUILayout.Space ();
			if(GUILayout.Button("+Add"))
			{
				Debug.Log ("Добавить персонажа");
			}
			
		}
		EditorGUILayout.EndVertical();	
	}
}
