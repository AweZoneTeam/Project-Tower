using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Время, в котором идут события игры (сериализация)
/// </summary>
[XmlType("GameTime")]
public class SerializedGameTime
{
    [XmlElement("Moment")]
    public float timer {get; set;}

    [XmlElement("Month")]
    public int monthNumb;

    [XmlElement("Day")]
    public int day;

    [XmlElement("Hour")]
    public int hour;

    [XmlElement("Min")]
    public int min;

    public SerializedGameTime() { } //пустой конструктор для сериализации

    public SerializedGameTime(float what)
    {
        timer = GameTime.timer;
        monthNumb = GameTime.monthNumb;
        day = GameTime.day;
        hour = GameTime.hour;
        min = GameTime.min;
    }

}

/// <summary>
/// Игровая статистика (сериализация)
/// </summary>
[XmlType("GameStatsData")]
[XmlInclude(typeof(SerializedGameTime))]
public class GameStatsData
{
    [XmlElement("GameTime")]
    public SerializedGameTime gameTime;

    [XmlElement("LastCheckpoint")]
    public string lastCheckpoint;

    [XmlElement("CurrentRoom")]
    public string currentRoom;

    [XmlElement("DeathTimes")]
    public int deathTimes;

    public GameStatsData()
    {
    }

    public GameStatsData(GameStatistics _gameStats)
    {
        gameTime = new SerializedGameTime(0f);
        lastCheckpoint = _gameStats.lastCheckPoint.currentRoom.id.areaName;
        currentRoom = GameStatistics.currentArea.id.areaName;
        deathTimes = _gameStats.deathNumber;
    }

}
