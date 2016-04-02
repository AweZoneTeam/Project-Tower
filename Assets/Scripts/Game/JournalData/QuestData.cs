using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Данные, что составляют информацию о любом задании
/// </summary>
[System.Serializable]
public class QuestData : JournalData
{
    [TextArea(3, 10)]
    public string conditions;
    public bool passed = false;
    public bool failed = false;
}
