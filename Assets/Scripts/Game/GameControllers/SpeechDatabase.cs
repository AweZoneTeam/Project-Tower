using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// База данных всех стартовых диалогов
/// </summary>
public class SpeechDatabase : MonoBehaviour
{

    #region dictionaries

    protected Dictionary<string, Dictionary<string, NPCSpeech>> speechDict=new Dictionary<string, Dictionary<string, NPCSpeech>>();//Словарь всех стартовых речей НПС, отсортированных по этим НПС
    public Dictionary<string, Dictionary<string, NPCSpeech>> SpeechDict { get { return speechDict; } } 

    #endregion //dictionaries

    #region fields

    public List<NPCSpeechDatabase> NPCs = new List<NPCSpeechDatabase>();//Список всех стартовых речей НПС, отсортированных по этим НПС

    #endregion //fields

    public void Awake()
    {
        FormDictionaries();
    }

    /// <summary>
    /// Сформировать словари
    /// </summary>
    void FormDictionaries()
    {
        speechDict = new Dictionary<string, Dictionary<string, NPCSpeech>>();
        foreach (NPCSpeechDatabase NPC in NPCs)
        {
            speechDict.Add(NPC.NPCName, new Dictionary<string, NPCSpeech>());
            foreach (NPCSpeech speech in NPC.speeches)
            {
                speechDict[NPC.NPCName].Add(speech.speechName, speech);
            }
        }
    }

}

/// <summary>
/// База данных всех стартовых речей одного персонажа
/// </summary>
[System.Serializable]
public class NPCSpeechDatabase
{
    public string NPCName;
    public List<NPCSpeech> speeches = new List<NPCSpeech>();
}
