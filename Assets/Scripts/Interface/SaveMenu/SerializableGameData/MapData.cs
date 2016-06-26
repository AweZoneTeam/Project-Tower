using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Класс, в котором хранится информация о карте
/// </summary>
[XmlType("MapData")]
[XmlInclude(typeof(MapLayerData))]
public class MapData
{
    [XmlArray("MapLayers")]
    [XmlArrayItem("MapLayer")]
    public List<MapLayerData> mapLayersData = new List<MapLayerData>();

    public MapData()
    { }

    public MapData(MapWindow mapWindow)
    {
        mapLayersData = new List<MapLayerData>();

        foreach (MapLayer mapLayer in mapWindow.MapLayers)
        {
            mapLayersData.Add(new MapLayerData(mapLayer));
        }
    }

}

/// <summary>
/// Класс, в котором хранится информация о слое карты
/// </summary>
[XmlType("MapLayerData")]
[XmlInclude(typeof(RoomMapData))]
public class MapLayerData
{
    [XmlElement("LayerName")]
    public string layerName;

    [XmlArray("RoomMaps")]
    [XmlArrayItem("RoomMap")]
    public List<RoomMapData> roomMapsData = new List<RoomMapData>();

    public MapLayerData()
    { }

    public MapLayerData(MapLayer mapLayer)
    {

        layerName = mapLayer.layer;

        roomMapsData = new List<RoomMapData>();

        foreach (RoomMap roomMap in mapLayer.RoomMaps)
        {
            roomMapsData.Add(new RoomMapData(roomMap));
        }
    }

}

/// <summary>
/// Информация, в котором хранится информация о карте одной комнаты
/// </summary>
[XmlType("RoomMapData")]
[XmlInclude(typeof(IconData))]
public class RoomMapData
{
    [XmlElement("RoomName")]
    public string roomName;

    [XmlAttribute("TerraIncognitta")]
    public bool incognitta;

    [XmlArray("Icons")]
    [XmlArrayItem("Icon")]
    public List<IconData> icons = new List<IconData>();

    public RoomMapData()
    {
    }

    public RoomMapData(RoomMap roomMap)
    {

        roomName = roomMap.gameObject.name;

        icons = new List<IconData>();

        for (int i = 0; i < roomMap.Doors.childCount; i++)
        {
            icons.Add(new IconData(roomMap.Doors.GetChild(i).gameObject));    
        }

        foreach (GameObject _enemyIcon in roomMap.Enemies)
        {
            icons.Add(new IconData(_enemyIcon));
        }

        foreach (GameObject _characterIcon in roomMap.Characters)
        {
            icons.Add(new IconData(_characterIcon));
        }

        foreach (GameObject _itemIcon in roomMap.Items)
        {
            icons.Add(new IconData(_itemIcon));
        }

        foreach (GameObject _questIcon in roomMap.Quests)
        {
            icons.Add(new IconData(_questIcon));
        }

        incognitta = roomMap.Incognitta;

    }

}

/// <summary>
/// Информация об иконке на карте
/// </summary>
public class IconData
{
    [XmlElement("IconType")]
    public string iconType;

    [XmlElement("IconPosition")]
    public Vector3 iconPosition;

    public IconData()
    {
    }

    public IconData(GameObject _icon)
    {
        iconType = _icon.name.Substring(0,_icon.name.IndexOf("Icon"));
        iconPosition = _icon.transform.localPosition;
    }

}