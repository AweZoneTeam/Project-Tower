using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Объект, что хранит в себе информацию о всех сделанных игроком сохранениях.
/// </summary>
[XmlType("SavesInfo")]
[XmlInclude(typeof(SaveInfo))]
public class SavesInfo 
{
    [XmlArray("Saves")]
    [XmlArrayItem("SaveInfo")]
    public List<SaveInfo> saves = new List<SaveInfo>();

    public SavesInfo()
    {
    }

    public SavesInfo(float f)
    {
        saves = new List<SaveInfo>();
    }

}

/// <summary>
/// Класс, в котором хранится информация об одном сохранении
/// </summary>
[XmlType ("SaveInformation")]
public class SaveInfo
{
    [XmlElement("SaveName")]
    public string saveName;

    [XmlElement("SaveTime")]
    public string saveTime;

    public SaveInfo()
    {
    }

    public SaveInfo(string sName, string sTime)
    {
        saveName = sName;
        saveTime = sTime;
    }
}