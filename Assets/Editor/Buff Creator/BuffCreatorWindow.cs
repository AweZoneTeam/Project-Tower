using UnityEditor;
using UnityEngine;
using System.Collections;

/// <summary>
/// Окно, в котором мы создаём новые баффы
/// </summary>
public class BuffCreateWindow : EditorWindow
{
    public string buffName = "New Buff";

    void OnGUI()
    {
        buffName = EditorGUILayout.TextField(buffName);
        if (GUILayout.Button("Create New"))
        {
            CreateNewBuff();
        }
    }

    //Создаём новый бафф.
    private void CreateNewBuff()
    {
        BuffClass asset = ScriptableObject.CreateInstance<BuffClass>();
        asset.buffName = buffName;
        AssetDatabase.CreateAsset(asset, "Assets/Database/Buffs/" + buffName + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
