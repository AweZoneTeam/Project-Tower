using UnityEngine;
using System.Collections;
using UnityEditor;

//Окно, которое позволяет нам добавить в список используемых на данный момент частей, анимаций или персонажей что-то новое
public class AddSomethingWindow : EditorWindow {
	
	public string name="New Visual Data";
	
	public RightAnimator rightAnim;
	public LeftAnimator leftAnim;
	public bool add;

	private Vector2 scrollPosition=Vector2.zero;

	//Что выводится
	void OnGUI()
	{
		AddCharacter ();
	}
	
	//Достаём из список персонажей и решаем, каким будем пользоваться на данный момент	
	private void AddCharacter()
	{
		AnimationEditorData animEditor = GameObject.Find ("AnimationEdit").GetComponent<AnimationEditorData>();
	   	AnimationEditorDataBase animBase = animEditor.animBase;
		GUILayout.BeginVertical ();
		{
			scrollPosition=GUI.BeginScrollView(new Rect(0f,0f,400f,200f),scrollPosition,new Rect(0,0,190,400f));
			{
				for (int i=0;i<animBase.allCharacters.Count;i++)
					if (GUILayout.Button(animBase.allCharacters[i]))
				{
					bool j=true;
					for (int k=0;k<animBase.usedCharacters.Count;k++)
						if (string.Equals(animBase.allCharacters[i],animBase.usedCharacters[k]))
							j=false;
					if (j)
					{
						animBase.usedCharacters.Add (animBase.allCharacters[i]);
							rightAnim.SaveAndCreate(animBase.allCharacters[i], "", animEditor.FindData(animBase.allCharacters[i]+".asset"),"");
					}
					this.Close();
				}
			}
			GUI.EndScrollView();
		}
		GUILayout.EndVertical ();
	}
		
}
