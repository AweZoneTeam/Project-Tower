using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Здесь собрана вся информация об уровне
/// </summary>
[XmlType("LocationInfo")]
[XmlInclude(typeof(RoomData))]
public class LocationInfo
{
    [XmlArray("RoomsData")]
    [XmlArrayItem("RoomData")]
    public List<RoomData> roomsData = new List<RoomData>();

    public LocationInfo()
    {
    }

    public LocationInfo(Map map)
    {
        roomsData = new List<RoomData>();

        foreach (AreaClass room in map.rooms)
        {
            roomsData.Add(new RoomData(room));
        }
    }

}

/// <summary>
/// Здесь собрана вся информация о комнате
/// </summary>
[XmlType("RoomData")]
[XmlInclude(typeof(InterObjData))]
[XmlInclude(typeof(ChestData))]
[XmlInclude(typeof(FlameObjData))]
[XmlInclude(typeof(MechData))]
[XmlInclude(typeof(CharacterData))]
[XmlInclude(typeof(PersonData))]
[XmlInclude(typeof(AIData))]
[XmlInclude(typeof(SpiderData))]
[XmlInclude(typeof(DropInfo))]
[XmlInclude(typeof(DoorData))]
public class RoomData
{
    [XmlElement("RoomName")]
    public string roomName;

    [XmlElement("MaxRegisterNumber")]
    public int maxRegistrationNumber;

    [XmlArray("ObjectsData")]
    [XmlArrayItem("ObjectData")]
    public List<InterObjData> objsData = new List<InterObjData>();

    [XmlArray("DropsData")]
    [XmlArrayItem("DropData")]
    public List<DropInfo> dropsInfo = new List<DropInfo>();

    [XmlArray("DoorsInfo")]
    [XmlArrayItem("DoorInfo")]
    public List<DoorData> doorsInfo = new List<DoorData>();

    public RoomData()
    { }

    public RoomData(AreaClass room)
    {
        roomName = room.id.areaName;
        maxRegistrationNumber = room.GetMaxRegistrationNumber();

        objsData = new List<InterObjData>();
        dropsInfo = new List<DropInfo>();
        doorsInfo = new List<DoorData>();

        foreach (InterObjController intObj in room.container)
        {
            objsData.Add(intObj.GetInfo());
        }

        foreach (DropClass drop in room.drops)
        {
            dropsInfo.Add(new DropInfo(drop));
        }

        int index = 0;
        DoorClass door;
        foreach (RoomConnection rConnection in room.neigbAreas)
        {
            if ((door=rConnection.door.GetComponent<DoorClass>())!=null)
            {
                doorsInfo.Add(new DoorData(door, index));
            }
            index++;
        }

    }

}

/// <summary>
/// Здесь собрана информация о двери
/// </summary>
[XmlType("DoorData")]
public class DoorData
{
    [XmlElement("Index")]
    public int index;

    [XmlAttribute("Opened")]
    public bool opened;

    public DoorData()
    {
    }

    public DoorData(DoorClass door, int _index)
    {
        index = _index;
        opened = door.locker.opened;
    }

}