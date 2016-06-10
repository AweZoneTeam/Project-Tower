using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Окно создания моделей поведения, которые используют ИИ для определения совершаемых действий
/// </summary>
public class BehaviourCreateWindow : EditorWindow
{
    public string behaviourName = "New Behaviour";

    public string behaviourPath = "Assets/Database/Behaviours/";

    void OnGUI()
    {
        behaviourName = EditorGUILayout.TextField(behaviourName);
        behaviourPath = EditorGUILayout.TextField(behaviourPath);
        if (GUILayout.Button("Create New"))
        {
            CreateNewBehavior();
        }
    }

    //Создаём новую модель поведения для ИИ
    private void CreateNewBehavior()
    {
        BehaviourClass asset = ScriptableObject.CreateInstance<BehaviourClass>();
        asset.behaviourName = behaviourName;
        asset.activities = new List<AIActionData>();
        AssetDatabase.CreateAsset(asset, behaviourPath + behaviourName + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
