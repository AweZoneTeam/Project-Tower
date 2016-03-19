using UnityEngine;
using System.Collections;
using UnityEditor;

public class SaveWindow : EditorWindow
{

	public LeftAnimator leftAnim;
	public string characterName;
	public VisualData characterData;

	void OnGUI()
	{
		EditorGUILayout.TextField ("Do you want to save changes?");
		GUILayout.BeginHorizontal ();
		{
			if (GUILayout.Button ("Save"))
			{
				SaveAndDelete ();
				Close ();
			}
			if (GUILayout.Button ("No")) 
			{
				OnlyDelete ();
				Close ();
			}
			if (GUILayout.Button ("Cancel"))
			{
				Close ();
			}
		}
		GUILayout.EndHorizontal();
	}

	void SaveAndDelete()
	{
		leftAnim.SaveChanges ();
		leftAnim.CreateNewInstance (characterName, "", characterData);
	}

	void OnlyDelete()
	{
		leftAnim.CreateNewInstance (characterName, "", characterData);
	}
}
