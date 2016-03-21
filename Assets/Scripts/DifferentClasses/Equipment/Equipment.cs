using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Самая простая коллекция игровых предметов.
/// </summary>
[System.Serializable]
public class BagClass
{
    public List<ItemBunch> bag;

    public BagClass()
    {
        bag = new List<ItemBunch>();
    }

    public List<ItemBunch> GetItems()
    {
        return bag;
    }
}

/// <summary>
/// Экипировка главного героя 
/// </summary>
[System.Serializable]
public class EquipmentClass: BagClass
{
    public int[] keys={0,0,0};// Сколько ключей каждого типа есть у персонажа?

    public WeaponClass rightWeapon, leftWeapon;

    public int GetKeysNumber(int index)
    {
        return keys[index];
    }

    public void UseKey(int index)
    {
        keys[index]--;
    }
    
}