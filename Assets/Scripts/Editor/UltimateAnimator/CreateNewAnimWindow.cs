using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class CreateNewAnimWindow :EditorWindow  
{
	public static int number = 0;
	public string name="New Visual Data";
	public string characterPath="Assets/Animations/";

    public string[] animatorTypes = { "character", "interactive" };
    public string animatorType="character";
    private int currentIndex=0;

    [HideInInspector]
	public RightAnimator rightAnim;
	[HideInInspector]
	public LeftAnimator leftAnim;

    void OnGUI()
	{
		name = EditorGUILayout.TextField (name);
		characterPath = EditorGUILayout.TextField (characterPath);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Animator type");
            currentIndex = EditorGUILayout.Popup(currentIndex, animatorTypes);
            animatorType = animatorTypes[currentIndex];
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button ("Create New"))
		{
			CreateNew();
		}
	}

	//Создаём визуальную часть нашего персонажа.
	private void CreateNew()
	{
		GameObject animEditor = GameObject.Find ("AnimationEdit");
		AnimationEditorDataBase animBase = animEditor.GetComponent<AnimationEditorData> ().animBase;
		bool j = true;
		for (int i=0; i<animBase.allCharacters.Count; i++)
			if (string.Equals (animBase.allCharacters [i], name))
				j = false;
		if (j)
		{
			VisualData asset = ScriptableObject.CreateInstance<VisualData>();
			asset.name = name;
            asset.type = animatorType;
			asset.SetEmpty ();
			AssetDatabase.CreateAsset (asset, "Assets/Database/Visual Data Base/" + name + ((number > 0 &&
				(string.Equals ("New Visual Data", name))) ? number.ToString () : "") + ".asset");
			AssetDatabase.SaveAssets ();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = asset;
			rightAnim.SaveAndCreate(name,characterPath,asset);
			animEditor.GetComponent<AnimationEditorData> ().RefreshCharacterList ();
			animBase.usedCharacters.Add (name);
		}
	}

}
