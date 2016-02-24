using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class CreateNewAnimWindow :EditorWindow  
{
	public static int number = 0;
	public string dataName="New Visual Data";
	public string characterPath="Assets/Animations/";

    public string[] animatorTypes = { "character", "interactive" };
    public string animatorType="character";
    private int currentIndex=0;

    private string stencilPath = "Assets/Animations/Stencils/";//В этом пути находятся шаблоны, уже созданные объекты, используемые для быстрого старта создания нового аниматора.
    public List<string> stencils=new List<string>();
    public string chosenStencil = "";
    private int stencilIndex=0;

    [HideInInspector]
	public RightAnimator rightAnim;
	[HideInInspector]
	public LeftAnimator leftAnim;

    public void Initialize()
    {
        stencils.Clear();
        Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(stencilPath);
        for (int i = 0; i < assets.Length; i++)
        {
            stencils.Add(assets[i].name);
        }
    }

    void OnGUI()
	{
		dataName = EditorGUILayout.TextField (dataName);
		characterPath =EditorGUILayout.TextField (characterPath);
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
        if (stencils.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Stencil");
                stencilIndex = EditorGUILayout.Popup(stencilIndex, stencils.ToArray());
                chosenStencil = stencils[stencilIndex];
            }
            if (GUILayout.Button("Create Stencil"))
            {
                CreateNew(chosenStencil);
            }
        }
    }

	//Создаём визуальную часть нашего персонажа.
	private void CreateNew()
	{
		GameObject animEditor = GameObject.Find ("AnimationEdit");
		AnimationEditorDataBase animBase = animEditor.GetComponent<AnimationEditorData> ().animBase;
		bool j = true;
		for (int i=0; i<animBase.allCharacters.Count; i++)
			if (string.Equals (animBase.allCharacters [i], dataName))
				j = false;
		if (j)
		{
			VisualData asset = ScriptableObject.CreateInstance<VisualData>();
			asset.name = dataName;
            asset.type = animatorType;
			asset.SetEmpty ();
			AssetDatabase.CreateAsset (asset, "Assets/Database/Visual Data Base/" + dataName + ((number > 0 &&
				(string.Equals ("New Visual Data", dataName))) ? number.ToString () : "") + ".asset");
			AssetDatabase.SaveAssets ();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = asset;
			rightAnim.SaveAndCreate(dataName,characterPath,asset,"");
			animEditor.GetComponent<AnimationEditorData> ().RefreshCharacterList ();
			animBase.usedCharacters.Add (dataName);

		}
	}

    //Создаём визуальную часть нашего персонажа, используя выбранный шаблон
    private void CreateNew(string stencilName)
    {
        GameObject animEditor = GameObject.Find("AnimationEdit");
        AnimationEditorDataBase animBase = animEditor.GetComponent<AnimationEditorData>().animBase;
        bool j = true;
        for (int i = 0; i < animBase.allCharacters.Count; i++)
            if (string.Equals(animBase.allCharacters[i], dataName))
                j = false;
        if (j)
        {
            string visString = AssetDatabase.CreateFolder(characterPath, "Visuals");
            AssetDatabase.CreateFolder(characterPath, "Parts");
            AssetDatabase.CreateFolder(characterPath, "GAFs");
            VisualData asset = ScriptableObject.CreateInstance<VisualData>();
            asset.name = dataName;
            asset.type = animatorType;
            asset.SetEmpty();
            AssetDatabase.CreateAsset(asset, "Assets/Database/Visual Data Base/" + dataName + ((number > 0 &&
                (string.Equals("New Visual Data", dataName))) ? number.ToString() : "") + ".asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            rightAnim.SaveAndCreate(dataName, visString, asset, characterPath+chosenStencil);
            animEditor.GetComponent<AnimationEditorData>().RefreshCharacterList();
            animBase.usedCharacters.Add(dataName);

        }
    }

}
