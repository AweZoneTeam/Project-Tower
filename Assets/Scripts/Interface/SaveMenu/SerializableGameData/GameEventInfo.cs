using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Класс, в котором хранится информация об игровых событиях, которым ещё предстоит произойти
/// </summary>
[XmlType("GameEventInfo")]
public class GameEventInfo
{
    [XmlArray("JournalEvents")]
    [XmlArrayItem("JournalEvent")]
    public List<string> jEvents = new List<string>();

    public GameEventInfo()
    {
    }

    public GameEventInfo(List<JournalEventScript> _jEvents)
    {
        jEvents = _jEvents.ConvertAll<string>(x => x.jScriptName);
    }

}
