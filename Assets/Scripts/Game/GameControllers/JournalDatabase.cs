using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, в котором будет содержаться информация о всех журнальных данных, что есть в игре
/// </summary>
public class JournalDatabase : MonoBehaviour 
{

    #region dictionaries

    protected static Dictionary<string, JournalData> charDict = new Dictionary<string, JournalData>();//Словарь из всех персонажей
    public Dictionary<string, JournalData> CharDict { get { return charDict; } }

    protected static Dictionary<string, JournalData> beastDict = new Dictionary<string, JournalData>();//Словарь из всех монстров
    public Dictionary<string, JournalData> BeastDict { get { return beastDict; } }

    protected static Dictionary<string, JournalData> locationDict = new Dictionary<string, JournalData>();//Словарь из всех локаций игры
    public Dictionary<string, JournalData> LocationDict { get { return locationDict; } }

    protected static Dictionary<string, QuestData> questDict = new Dictionary<string, QuestData>();//Словарь из всех квестов игры
    public Dictionary<string, QuestData> QuestDict { get { return questDict; } }

    #endregion //dictionaries

    #region fields

    public List<JournalData> characters = new List<JournalData>();//Список персонажей
    public List<JournalData> beasts = new List<JournalData>();//Список монстров
    public List<JournalData> locations = new List<JournalData>();//Список мест
    public List<QuestData> quests = new List<QuestData>();//Список квестов

    #endregion //fields

    public void Awake()
    {
        FormDictionaries();
    }

    public void FormDictionaries()
    {
        charDict = new Dictionary<string, JournalData>();
        beastDict = new Dictionary<string, JournalData>();
        locationDict = new Dictionary<string, JournalData>();
        questDict = new Dictionary<string, QuestData>();

        foreach (JournalData jData in characters)
        {
            charDict.Add(jData.dataName, jData);
        }

        foreach (JournalData jData in beasts)
        {
            beastDict.Add(jData.dataName, jData);
        }

        foreach (JournalData jData in locations)
        {
            locationDict.Add(jData.dataName, jData);
        }

        foreach (QuestData jData in quests)
        {
            questDict.Add(jData.dataName, jData);
        }
    }

}
