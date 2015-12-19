using UnityEngine;
using System.Collections;
using UnityEditor;

public class SOManager : MonoBehaviour {
	[MenuItem("Assets/Create/My Scriptable Object")]
	public static void CreateMyAsset()
	{
		AnimEditorData asset = ScriptableObject.CreateInstance<AnimEditorData> ();
		AssetDatabase.CreateAsset (asset, "Asset/Anim Editor Data.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

}
