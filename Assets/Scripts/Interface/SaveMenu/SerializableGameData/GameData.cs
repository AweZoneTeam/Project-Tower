using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// В этом классе будут храниться все необходимые данные игры. Экземпляр именно этого класса храниться в сохранениях.
/// </summary>
[XmlType("GameData")]
[XmlInclude(typeof(GameStatsData))]
[XmlInclude(typeof(PersonData))]
[XmlInclude(typeof(JournalInfo))]
[XmlInclude(typeof(EquipmentInfo))]
[XmlInclude(typeof(GameEventInfo))]
[XmlInclude(typeof(LocationInfo))]
[XmlInclude(typeof(MapData))]
public class GameData
{
    [XmlElement("GameStats")]
    public GameStatsData gameStats;

    [XmlElement("MainCharacterData")]
    public PersonData mainCharData;

    [XmlElement("JournalInfo")]
    public JournalInfo jInfo;

    [XmlElement("EquipmentInfo")]
    public EquipmentInfo eInfo;

    [XmlElement("GameEventInfo")]
    public GameEventInfo gInfo;

    [XmlElement("LocationInfo")]
    public LocationInfo lInfo;

    [XmlElement("MapInfo")]
    public MapData mInfo;

    public GameData()
    {
    }

    public GameData(GameStatistics _stats, KeyboardActorController _player, JournalWindow jWindow, EquipmentClass equip, List<EquipmentSlot> bagSlots, List<JournalEventScript> _jEvents, Map map, MapWindow mapWindow)
    {
        gameStats = new GameStatsData(_stats);
        mainCharData = new PersonData(_player);
        jInfo = new JournalInfo(jWindow);
        eInfo = new EquipmentInfo(equip, bagSlots);
        gInfo = new GameEventInfo(_jEvents);
        lInfo = new LocationInfo(map);
        mInfo = new MapData(mapWindow);
    }

}
