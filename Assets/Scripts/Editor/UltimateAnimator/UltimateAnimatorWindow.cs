using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

class UltimateAnimatorWindow : EditorWindow
{
	public List<GameObject> aCharacters;
	public Rect windowRect = new Rect(100f, 100f, 200f, 200f);
	public EditorWindow sceneWindow;

	void OnGUI () 
	{
		EditorGUILayout.BeginHorizontal ();
		{
			EditorGUILayout.BeginVertical ();
			{
				for (int i=0; i<aCharacters.Count; i++)
					EditorGUILayout.LabelField (aCharacters [i].name);
				EditorGUILayout.Space();
				BeginWindows();
				{	
					windowRect = GUILayout.Window(1, windowRect, DoWindow, aCharacters[0].name);
				}
				EndWindows();
			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.Space();
		}
		EditorGUILayout.EndHorizontal ();
	}

	void DoWindow(int unusedWindowID) 
	{
		EditorGUILayout.LabelField("HI THERE");
		GUI.DragWindow();
	}

}
