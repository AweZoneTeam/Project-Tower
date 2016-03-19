using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
    using UnityEditor;
#endif

/// <summary>
/// таблица, в которой находятся все совершаемые ИИ действия, причины и вероятность  их совершения
/// </summary>
[System.Serializable]
public class BehaviourClass : ScriptableObject
{
    public string behaviourName;
    public List<AIActionData> activities = new List<AIActionData>();

    public BehaviourClass(BehaviourClass original)
    {
        name = original.name;
        behaviourName = original.behaviourName;
        activities = new List<AIActionData>();
        for (int i = 0; i < original.activities.Count; i++)
        {
            activities.Add(new AIActionData(original.activities[i]));
        }
    }
}
