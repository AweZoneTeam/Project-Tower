using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, представляющий собой оружие и всю базу данных о нём.
/// </summary>
[System.Serializable]
public class WeaponClass : ItemClass
{
    public List<HitClass> hitData = new List<HitClass>();

    public HitClass GetHit(string hitName)
    {
        for (int i = 0; i < hitData.Count; i++)
        {
            if (string.Equals(hitData[i].hitName, hitName))
            {
                return hitData[i];
            }
        }
        return null;
    }
}