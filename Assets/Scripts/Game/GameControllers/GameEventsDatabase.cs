using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// База данных, в которой храняться все события игры
/// </summary>
public class GameEventsDatabase : MonoBehaviour {

    #region dictionaries

    protected Dictionary<string, JournalEventScript> jEventDict = new Dictionary<string, JournalEventScript>();//Словарь игровых событий
    public Dictionary<string, JournalEventScript> JEventDict {get { return jEventDict; } }

    #endregion //dictionaries

    #region fields

    public List<JournalEventScript> jEventsList = new List<JournalEventScript>();//Список всех игровых событий, представленных в игре

    #endregion //fields

    public void Awake ()
    {
        FormDictionaries();
	}

    public void FormDictionaries()
    {

        jEventDict = new Dictionary<string, JournalEventScript>();

        foreach (JournalEventScript jEvent in jEventsList)
        {
            jEventDict.Add(jEvent.jScriptName, jEvent);
        }
    }

}
