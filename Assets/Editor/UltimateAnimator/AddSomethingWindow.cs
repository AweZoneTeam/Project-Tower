using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

//Окно, которое позволяет нам добавить в список используемых на данный момент частей, анимаций или персонажей что-то новое
public class AddSomethingWindow : EditorWindow {
	
	public string dataName="Data Name";
    public int dataIndex = 0;

	public RightAnimator rightAnim;
    public AnimationEditorDataBase animBase;
    public AnimationEditorData animEditor;

    public LeftAnimator leftAnim;
	public bool add;

	private Vector2 scrollPosition=Vector2.zero;

	//Что выводится
	void OnGUI()
	{
        animEditor = GameObject.Find("AnimationEdit").GetComponent<AnimationEditorData>();
        animBase = animEditor.animBase;
        AddCharacter ();
	}
	
	//Достаём из список персонажей и решаем, каким будем пользоваться на данный момент	
	private void AddCharacter()
	{
		GUILayout.BeginVertical ();
		{
            EditorGUILayout.LabelField(dataName);
			scrollPosition=GUI.BeginScrollView(new Rect(0f,20f,400f,200f),scrollPosition,new Rect(0,20,190,400f));
            {
                for (int i = 0; i < animBase.allCharacters.Count; i++)
                { 
                    if (GUILayout.Button(animBase.allCharacters[i]))
                    {
                        dataIndex = i;
                        dataName = animBase.allCharacters[i];
                    }
                }
			}
			GUI.EndScrollView();
            GUILayout.BeginArea(new Rect(0f, 220f, 400f, 20f));
            {
                if (animBase.allCharacters.Count > 0)
                {
                    if (GUILayout.Button("Add"))
                    {
                        bool j = true;
                        for (int k = 0; k < animBase.usedCharacters.Count; k++)
                            if (string.Equals(animBase.allCharacters[dataIndex], animBase.usedCharacters[k]))
                                j = false;
                        if (j)
                        {
                            animBase.usedCharacters.Add(animBase.allCharacters[dataIndex]);
                            rightAnim.SaveAndCreate(animBase.allCharacters[dataIndex], "", animEditor.FindData(animBase.allCharacters[dataIndex] + ".asset"), "");
                        }
                        this.Close();
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        bool j = true;
                        for (int k = 0; k < animBase.usedCharacters.Count; k++)
                        {
                            if (string.Equals(animBase.allCharacters[dataIndex], animBase.usedCharacters[k]))
                            {
                                j = false;
                            }
                        }
                        if (j)
                        {
                            animBase.usedCharacters.Remove(animBase.allCharacters[dataIndex]);
                        }
                        VisualData vis = animEditor.FindData(animBase.allCharacters[dataIndex] + ".asset");
                        List<PartController> parts=vis.visual.GetComponent<InterObjAnimator>().parts;
                        AnimationInterpretator interp;
                        string interpName;
                        string partPath;
                        for (int i = 0; i < vis.animInterpretators.Count; i++)
                        {
                            interp = vis.animInterpretators[i];
                            interpName = parts[i].gameObject.name;
                            partPath = interp.partPath;
                            AssetDatabase.DeleteAsset(interp.partPath+interpName+".asset");
                            AssetDatabase.DeleteAsset(interp.partPath + interpName + ".prefab");
                        }
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(vis.visual));
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(vis.visual));
                        //if (right)
                        animBase.allCharacters.RemoveAt(dataIndex);
                        dataName = "Data Name";
                        dataIndex = 0;
                    }
                }
            }
            GUILayout.EndArea();
        }
		GUILayout.EndVertical ();
	}
		
}
