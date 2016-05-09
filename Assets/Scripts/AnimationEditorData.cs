using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimationEditorData : MonoBehaviour 
{
	//В редакторе есть база данных по используемым редактором анимаций персонажей. Зачем он нужен. Честно говоря, для удобства, но на самом деле можно обойтись и без него
	public AnimationEditorDataBase animBase;
	public List<string> aCharacters = new List<string>();

	public string datapath="Assets/Database/Visual Data Base/";

	public void RefreshCharacterList()
	{
		animBase.allCharacters.Clear ();
		DirectoryInfo direct = new DirectoryInfo (datapath);
		FileInfo[] files = direct.GetFiles();
		foreach (FileInfo file in files)
		{
			if (!file.Name.Contains ("meta")) {
				animBase.allCharacters.Add (file.Name.Substring(0,file.Name.IndexOf(".asset")));//Убираем из имени файла название его расширения
			}

		}
		aCharacters = animBase.allCharacters;
	}

	public VisualData FindData(string name)
	{
#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath(datapath+name, typeof(VisualData)) as VisualData;
#endif
        return null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AnimationEditorData))]
public class AnimDataEditorEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();
		AnimationEditorData myTarget = (AnimationEditorData)target;
		if (GUILayout.Button ("RefreshCharacterList"))
			myTarget.RefreshCharacterList ();
	}	
}
#endif