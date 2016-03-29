using UnityEngine;
using System;
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

    #region eventHandlers

    public EventHandler<ItemChangedEventArgs> ItemChangedEvent;

    #endregion //eventHandlers

    #region fields

    public int[] keys={0,0,0};// Сколько ключей каждого типа есть у персонажа?

    public WeaponClass rightWeapon, leftWeapon;
    public WeaponClass altRightWeapon, altLeftWeapon;

    public UsableItemClass useItem;
    public List<UsableItemClass> useItems = new List<UsableItemClass>();

    #endregion //fields

    public int GetKeysNumber(int index)
    {
        return keys[index];
    }

    public void UseKey(int index)
    {
        keys[index]--;
    }

    /// <summary>
    /// Сменить оружие в правой руке
    /// </summary>
    public void ChangeRightWeapon()
    {
        WeaponClass weapon = rightWeapon;
        rightWeapon = altRightWeapon;
        altRightWeapon = weapon;
        OnItemChanged(new ItemChangedEventArgs(rightWeapon, "rightWeapon"));
    }
    
    /// <summary>
    /// Сменить оружие в левой руке
    /// </summary>
    public void ChangeLeftWeapon()
    {
        WeaponClass weapon = leftWeapon;
        leftWeapon = altLeftWeapon;
        altLeftWeapon = weapon;
        OnItemChanged(new ItemChangedEventArgs(leftWeapon, "leftWeapon"));

    }

    /// <summary>
    /// Сменить используемый предмет
    /// </summary>
    public void ChangeItem()
    {
        if (useItems.Count > 0)
        {
            if (useItems.IndexOf(useItem) + 1 == useItems.Count)
            {
                useItem = useItems[0];
            }
            else
            {
                useItem = useItems[useItems.IndexOf(useItem) + 1];
            }
            OnItemChanged(new ItemChangedEventArgs(useItem, "usable"));
        }
    }

    #region events

    public void OnItemChanged(ItemChangedEventArgs e)
    {
        EventHandler<ItemChangedEventArgs> handler = ItemChangedEvent;

        if (handler != null)
        {
            handler(this, e);
        }
    }

    #endregion //events


}