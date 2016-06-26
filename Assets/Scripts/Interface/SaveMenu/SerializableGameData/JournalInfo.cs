using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Класс, в котором содержится информация о состоянии журнала
/// </summary>
[XmlType("JournalInfo")]
public class JournalInfo
{
    [XmlArray("Characters")]
    [XmlArrayItem("Character")]
    public List<string> characters = new List<string>();

    [XmlArray("Beasts")]
    [XmlArrayItem("Beast")]
    public List<string> beasts = new List<string>();

    [XmlArray("Locations")]
    [XmlArrayItem("Location")]
    public List<string> locations = new List<string>();

    [XmlArray("ActiveQuests")]
    [XmlArrayItem("ActiveQuest")]
    public List<string> activeQuests = new List<string>();

    [XmlArray("CompletedQuests")]
    [XmlArrayItem("CompleteQuest")]
    public List<string> completedQuests = new List<string>();

    [XmlArray("FailedQuests")]
    [XmlArrayItem("FailedQuest")]
    public List<string> failedQuests = new List<string>();

    public JournalInfo()
    { }

    public JournalInfo(JournalWindow jWindow)
    {
        characters = jWindow.CharactersList.ConvertAll<string>(x=>x.dataName);
        beasts = jWindow.BeastsList.ConvertAll<string>(x => x.dataName);
        locations = jWindow.LocationsList.ConvertAll<string>(x => x.dataName);
        activeQuests = jWindow.ActiveQuests.ConvertAll<string>(x => x.dataName);
        completedQuests = jWindow.CompletedQuests.ConvertAll<string>(x => x.dataName);
        failedQuests = jWindow.FailedQuests.ConvertAll<string>(x => x.dataName);
    }

}
