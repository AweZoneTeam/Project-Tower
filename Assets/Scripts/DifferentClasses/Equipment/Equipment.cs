﻿using UnityEngine;
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

    public EventHandler<ItemChangedEventArgs> ActiveItemChangedEvent;
    public EventHandler<ResourceChangedEventArgs> ResourceChangedEvent;

    #endregion //eventHandlers

    #region fields

    public int[] keys={0,0,0};// Сколько ключей каждого типа есть у персонажа?
    public int gold = 0;//Сколько золота?

    public WeaponClass rightWeapon, leftWeapon;
    public WeaponClass altRightWeapon, altLeftWeapon;

    public ArmorSet armor;

    public UsableItemClass useItem;
    public List<UsableItemClass> useItems = new List<UsableItemClass>();

    #endregion //fields

    /// <summary>
    /// Конструктор
    /// </summary>
    public EquipmentClass()
    {
    }

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
        OnActiveItemChanged(new ItemChangedEventArgs(rightWeapon, "rightWeapon"));

        if (altLeftWeapon == rightWeapon)
        {
            ChangeLeftWeapon();
        }
    }
    
    /// <summary>
    /// Сменить оружие в левой руке
    /// </summary>
    public void ChangeLeftWeapon()
    {
        WeaponClass weapon = leftWeapon;
        leftWeapon = altLeftWeapon;
        altLeftWeapon = weapon;
        OnActiveItemChanged(new ItemChangedEventArgs(leftWeapon, "leftWeapon"));
        if (altRightWeapon == leftWeapon)
        {
            ChangeRightWeapon();
        }

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
            OnActiveItemChanged(new ItemChangedEventArgs(useItem, "usable"));
        }
    }

    /// <summary>
    /// Сменить предмет в инвентаре
    /// </summary>
    public List<ItemClass> ChangeEquipmentElement(ItemClass item, string itemType)
    {
        List<ItemClass> removeItems=new List<ItemClass>();

        #region weapons

        if (item? string.Equals(item.type, "weapon"): true)
        {
            if (string.Equals("rightWeapon1", itemType))
            {
                removeItems.Add(rightWeapon);
                if (leftWeapon==rightWeapon)
                {
                    leftWeapon = null;
                }
                rightWeapon = (WeaponClass)item;
            }

            else if (string.Equals("rightWeapon2", itemType))
            {
                if (altLeftWeapon == altRightWeapon)
                {
                    altLeftWeapon = null;
                }
                altRightWeapon = (WeaponClass)item;
            }

            else if (string.Equals("leftWeapon1", itemType))
            {
                removeItems.Add(leftWeapon);
                if (rightWeapon == leftWeapon)
                {
                    rightWeapon = null;
                }
                leftWeapon = (WeaponClass)item;
            }

            else if (string.Equals("leftWeapon2", itemType))
            {
                if (altRightWeapon == altLeftWeapon)
                {
                    altRightWeapon = null;
                }
                altLeftWeapon = (WeaponClass)item;
            }

            else if (string.Equals("twoHandedWeapon1", itemType))
            {
                removeItems.Add(leftWeapon);
                removeItems.Add(rightWeapon);
                leftWeapon = (WeaponClass)item;
                rightWeapon = (WeaponClass)item;
            }

            else if (string.Equals("twoHandedWeapon2", itemType))
            {
                removeItems.Add(altLeftWeapon);
                removeItems.Add(altRightWeapon);
                altLeftWeapon = (WeaponClass)item;
                altRightWeapon = (WeaponClass)item;
            }
        }

        #endregion //weapons

        #region armor

        else if (item? string.Equals(item.type, "armor"): true)
        {
            if (string.Equals("helmet", itemType))
            {
                removeItems.Add(armor.helmet);
                armor.helmet = (ArmorClass)item;
            }
            else if (string.Equals("cuirass", itemType))
            {
                removeItems.Add(armor.cuirass);
                armor.cuirass = (ArmorClass)item;
            }
            else if (string.Equals("pants", itemType))
            {
                removeItems.Add(armor.pants);
                armor.pants = (ArmorClass)item;
            }
            else if (string.Equals("gloves", itemType))
            {
                removeItems.Add(armor.gloves);
                armor.gloves = (ArmorClass)item;
            }
            else if (string.Equals("boots", itemType))
            {
                removeItems.Add(armor.boots);
                armor.boots = (ArmorClass)item;
            }

        }

        #endregion //armor

        #region usableItems

        else if (item? string.Equals(item.type, "usable"):true)
        {
            int index = 0;
            if (string.Equals("usable1", itemType))
            {
                index = 0;
            }
            else if (string.Equals("usable2", itemType))
            {
                index = 1;
            }
            else if (string.Equals("usable3", itemType))
            {
                index = 2;
            }
            else if (string.Equals("usable4", itemType))
            {
                index = 3;
            }
            if (useItem != null)
            {
                if (useItems.IndexOf(useItem) == index)
                {
                    useItem = (UsableItemClass)item;
                }
            }
            removeItems.Add(useItems[index]);
            useItems.Insert(index, (UsableItemClass)item);
            OnActiveItemChanged(new ItemChangedEventArgs(useItem, "usable"));
        }

        #endregion //usableItems

        #region rings

        else if (string.Equals(item.type, "ring"))
        {
            if (string.Equals("rightRing", itemType))
            {
                removeItems.Add(armor.rightRing);
                armor.rightRing = item;
            }
            else if (string.Equals("leftRing", itemType))
            {
                removeItems.Add(armor.leftRing);
                armor.leftRing = item;
            }
        }

        #endregion //rings

        for (int i = 0; i < removeItems.Count; i++)
        {
            
        }

        return removeItems;
    }

    /// <summary>
    /// Функция подбора новых предметов
    /// </summary>
    public void TakeItem(ItemBunch itemBunch)
    {
        ItemBunch _itemBunch;
        if (string.Equals(itemBunch.item.type, "money"))
        {
            gold += itemBunch.item.maxCount;
            OnResourceChanged(new ResourceChangedEventArgs());
        }
        else if (string.Equals(itemBunch.item.itemName, "Железный ключ"))
        {
            keys[0] += itemBunch.item.maxCount;
            OnResourceChanged(new ResourceChangedEventArgs());
        }
        else if (string.Equals(itemBunch.item.itemName, "Серебряный ключ"))
        {
            keys[1] += itemBunch.item.maxCount;
            OnResourceChanged(new ResourceChangedEventArgs());
        }
        else if (string.Equals(itemBunch.item.itemName, "Золотой ключ"))
        {
            keys[2] += itemBunch.item.maxCount;
            OnResourceChanged(new ResourceChangedEventArgs());
        }
        else
        {
            for (int i = 0; i < bag.Count; i++)
            {
                if (bag[i].item == itemBunch.item)
                {
                    _itemBunch = bag[i];
                    int reserve = _itemBunch.item.maxCount - _itemBunch.quantity - itemBunch.quantity;
                    itemBunch.quantity = (reserve >= 0 ? 0 : -reserve);
                    _itemBunch.quantity = (reserve >= 0 ? _itemBunch.quantity + itemBunch.quantity : _itemBunch.item.maxCount);
                }
                if (itemBunch.quantity == 0)
                {
                    return;
                }
            }
            bag.Add(itemBunch);
        }
    }

    #region events

    /// <summary>
    /// Вызывается, как только меняется активный предмет.
    /// </summary>
    public void OnActiveItemChanged(ItemChangedEventArgs e)
    {
        EventHandler<ItemChangedEventArgs> handler = ActiveItemChangedEvent;

        if (handler != null)
        {
            handler(this, e);
        }
    }

    /// <summary>
    /// Вызавается, как только меняются ресурсы персонажа (золото, ключи)
    /// </summary>
    public void OnResourceChanged(ResourceChangedEventArgs e)
    {
        EventHandler<ResourceChangedEventArgs> handler = ResourceChangedEvent;

        if (handler != null)
        {
            e.Gold = gold;
            e.IronKey = keys[0];
            e.SilverKey = keys[1];
            e.GoldKey = keys[2];
            handler(this, e);
        }

    }

    #endregion //events

}

/// <summary>
/// Класс, характеризующий собой сет доспехов
/// </summary>
[System.Serializable]
public class ArmorSet
{
    public ArmorClass helmet, cuirass, pants, gloves, boots;
    public ItemClass leftRing, rightRing;
}