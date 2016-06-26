using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// База данных по всем элементам инвентаря
/// </summary>
public class EquipmentDatabase : MonoBehaviour
{

    #region dictionaries

    protected Dictionary<string, ArmorClass> armorDict=new Dictionary<string, ArmorClass>();//Словарь, состоящий из элементов доспеха
    public Dictionary<string,ArmorClass> ArmorDict { get { return armorDict; } }

    protected Dictionary<string, WeaponClass> weaponDict = new Dictionary<string, WeaponClass>();//Словарь, состоящий из видов оружия
    public Dictionary<string, WeaponClass> WeaponDict { get { return weaponDict; } }

    protected Dictionary<string, UsableItemClass> useItemDict = new Dictionary<string, UsableItemClass>();//Словарь, состоящий из используемых предметов
    public Dictionary<string, UsableItemClass> UseItemDict { get { return useItemDict; } }

    protected Dictionary<string, ItemClass> itemDict = new Dictionary<string, ItemClass>();//Словарь, состоящий из предметов
    public Dictionary<string, ItemClass> ItemDict { get { return itemDict; } }

    #endregion //dictionaries

    #region fields

    public List<ArmorClass> armorElements = new List<ArmorClass>();//Список всех элементов доспеха, представленных в игре
    public List<WeaponClass> weapons = new List<WeaponClass>();//Список всех видов оружия, представленных в игре
    public List<UsableItemClass> useItems = new List<UsableItemClass>();//Список всех ипользуемых предметов, представленных в игре
    public List<ItemClass> items = new List<ItemClass>();//Список всех предметов, представленных в игре

    #endregion //fields

    public void Awake()
    {
        FormDictionaries();
    }

    void FormDictionaries()
    {
        foreach (ArmorClass armor in armorElements)
        {
            armorDict.Add(armor.itemName, armor);
        }

        foreach (WeaponClass weapon in weapons)
        {
            weaponDict.Add(weapon.itemName, weapon);
        }

        foreach (UsableItemClass useItem in useItems)
        {
            useItemDict.Add(useItem.itemName, useItem);
        }

        foreach (ItemClass item in items)
        {
            itemDict.Add(item.itemName, item);
        }
    }

}
