using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Окно, в котором создаются новые журнальные данные
/// </summary>
public class JournalDataCreatorWindow : EditorWindow
{
    public string jDataName = "New Journal Data";

    void OnGUI()
    {
        jDataName = EditorGUILayout.TextField(jDataName);
        if (GUILayout.Button("Create New"))
        {
            CreateNewJournalData();
        }
    }

    //Создаём новое журнальное событие.
    private void CreateNewJournalData()
    {
        JournalData asset = ScriptableObject.CreateInstance<JournalData>();
        asset.dataName = jDataName;
        AssetDatabase.CreateAsset(asset, "Assets/Database/JournalDatabase/" + jDataName + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

}

/// <summary>
/// Окно, в котором создаются новые задания
/// </summary>
public class QuestCreatorWindow : EditorWindow
{
    public string questName = "Quest";

    void OnGUI()
    {
        questName = EditorGUILayout.TextField(questName);
        if (GUILayout.Button("Create New"))
        {
            CreateNewQuest();
        }
    }

    //Создаём новый квест.
    private void CreateNewQuest()
    {
        QuestData asset = ScriptableObject.CreateInstance<QuestData>();
        asset.dataName = questName;
        AssetDatabase.CreateAsset(asset, "Assets/Database/JournalDatabase/Quests/" + questName + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

}

/// <summary>
/// Окно, в котором создаются журнальные события
/// </summary>
public class JournalScriptCreatorWindow : EditorWindow
{
    public string journalScriptName = "New Journal Script";

    void OnGUI()
    {
        journalScriptName = EditorGUILayout.TextField(journalScriptName);
        if (GUILayout.Button("Create New"))
        {
            CreateNewJournalScript();
        }
    }

    //Создаём новый квест.
    private void CreateNewJournalScript()
    {
        JournalEventScript asset = ScriptableObject.CreateInstance<JournalEventScript>();
        asset.jScriptName = journalScriptName;
        AssetDatabase.CreateAsset(asset, "Assets/Database/JournalScripts/" + journalScriptName + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

}