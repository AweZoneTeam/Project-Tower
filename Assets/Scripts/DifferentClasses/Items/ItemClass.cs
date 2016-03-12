using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, представляющий собой игровой предмет, который можно засунуть в инвентарь.
/// </summary>
[System.Serializable]
public class ItemClass: ScriptableObject
{
	public string itemName;
    public string type;
    public Sprite image;//Иконка предмета  
}

/// <summary>
/// Класс, представляющий собой оружие и всю базу данных о нём.
/// </summary>
[System.Serializable]
public class WeaponClass : ItemClass
{
    public List<HitClass> hitData=new List<HitClass>();

    public HitClass GetHit(string hitName)
    {
        for (int i=0;i<hitData.Count;i++)
        {
            if (string.Equals(hitData[i].hitName, hitName))
            {
                return hitData[i];
            }
        }
        return null;
    }
}

/// <summary>
/// Специальный класс, воспринимаемый инвентарём, чтобы знать, сколько предметов каждого типа есть у интерактивного объекта
/// </summary>
[System.Serializable]
public class ItemBunch 
{
    public ItemClass item;
    public int quantity;
}