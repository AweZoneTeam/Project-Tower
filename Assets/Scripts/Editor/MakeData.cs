using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeData 
{
	[MenuItem("Assets/Create/Project Tower Data/Visual Data")]
	public static void CreateVisualData()
	{
		AnimationEditorDataBase asset = ScriptableObject.CreateInstance<AnimationEditorDataBase> ();
		AssetDatabase.CreateAsset (asset, "Assets/Scripts/Editor/Animated Characters DataBase.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}
}
