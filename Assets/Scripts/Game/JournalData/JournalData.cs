using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

/// <summary>
/// Данные, что составляют журнал
/// </summary>
[System.Serializable]
public class JournalData: ScriptableObject
{
    public string dataName;
    public string dataType;
    public Sprite dataImage;
    public string dataSecondName;
    [TextArea(3, 10)]
    public string dataDescription;

}
