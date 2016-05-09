using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, обозначающий ячейки, в которых находятся активные предметы
/// </summary>
public class ItemSlot : EquipmentSlot
{

    private static Dictionary<string, List<string>> isSuitableItem = new Dictionary<string, List<string>> {
                                                                                                            { "helmet", new List<string> { "helmet" } },
                                                                                                            { "gloves",new List<string> { "gloves"} },
                                                                                                            {"boots",new List<string> { "boots"} },
                                                                                                            {"cuirass",new List<string> { "cuirass"} },
                                                                                                            {"pants",new List<string> { "pants" } },
                                                                                                            {"rightRing", new List<string> {"ring"} },
                                                                                                            { "leftRing" ,new List<string> { "ring"} },
                                                                                                            {"rightWeapon1", new List<string> {"sword", "spear", "fist", "twoHandedSword", "bow" } },
                                                                                                            {"rightWeapon2", new List<string> {"sword", "spear", "fist","twoHandedSword", "bow" } },
                                                                                                            {"leftWeapon1", new List<string> {"shield", "torch", "fist", "twoHandedSword", "bow" } },
                                                                                                            {"leftWeapon2", new List<string> {"shield", "torch", "fist", "twoHandedSword", "bow" } },
                                                                                                            {"usable1", new List<string> {"usable" } },
                                                                                                            {"usable2", new List<string> {"usable" } },
                                                                                                            {"usable3", new List<string> {"usable" } },
                                                                                                            {"usable4", new List<string> {"usable" } }
                                                                                                          };
    private static Dictionary<string, List<string>> isSuitableTwoHandedItem = new Dictionary<string, List<string>>
    {
        {"twoHandedWeapon1",new List<string> {"twoHandedSword","bow" } },
        {"twoHandedWeapon2",new List<string> {"twoHandedSword","bow" } }
    };

    public string slotType;

    public override void ChooseItem()
    {
        base.ChooseItem();
    }

    public override bool IsItemProper(ItemBunch _itemBunch)
    {
        if (_itemBunch == null)
        {
            return true;
        }
        else
        {
            if (string.Equals(_itemBunch.item.type, "armor"))
            {
                ArmorClass armor = (ArmorClass)_itemBunch.item;
                if (isSuitableItem[slotType].Contains(armor.armorType))
                {
                    return true;
                }
                return false;
            }
            else if (string.Equals(_itemBunch.item.type, "weapon"))
            {
                WeaponClass weapon = (WeaponClass)_itemBunch.item;
                return isSuitableItem[slotType].Contains(weapon.weaponType);
            }
            else if ((string.Equals(_itemBunch.item.type, "ring")) || (string.Equals(_itemBunch.item.type, "usable")))
            {
                if (isSuitableItem[slotType].Contains(_itemBunch.item.type))
                {
                    return true;
                }
                return false;
            }
        }
        return false;
    }

    public override void AddItem(ItemBunch _itemBunch)
    {
        base.AddItem(_itemBunch);
        if (_itemBunch != null)
        {
            if (_itemBunch.item != null)
            {
                if (itemBunch.item is WeaponClass)
                {
                    WeaponClass weapon = (WeaponClass)_itemBunch.item;
                    if (slotType.Contains("1") && (isSuitableTwoHandedItem["twoHandedWeapon1"].Contains(weapon.weaponType)) ||
                        slotType.Contains("2") && (isSuitableTwoHandedItem["twoHandedWeapon2"].Contains(weapon.weaponType)))
                    {
                        if (pair != null)
                        {
                            if (pair.itemBunch == null)
                            {
                                pair.Initialize(equip, _itemBunch);
                            }
                            else
                            {
                                equip.AddItemInBag(pair.itemBunch);
                                pair.Initialize(equip, _itemBunch);
                            }
                        }
                        equip.ChangeCharacterEquipment(itemBunch, slotType.Contains("1") ? "twoHandedWeapon1" : "twoHandedWeapon2");
                        return;
                    }
                    else
                    {
                        //если это одноручное оружие то надо убрать из второй руки оружие если оно двуручное
                        if (pair.itemBunch != null)
                        {
                            if (pair.slotType.Contains("1") && (isSuitableTwoHandedItem["twoHandedWeapon1"].Contains(((WeaponClass)pair.itemBunch.item).weaponType)) ||
                                pair.slotType.Contains("2") && (isSuitableTwoHandedItem["twoHandedWeapon2"].Contains(((WeaponClass)pair.itemBunch.item).weaponType)))
                            {
                                pair.DeleteItem();
                            }
                        }
                    }
                }
            }
            equip.ChangeCharacterEquipment(itemBunch, slotType);
        }
        else
        {
            DeleteItem();
            if (pair != null)
            {
                if (pair.itemBunch != null)
                {
                    WeaponClass weapon = (WeaponClass)pair.itemBunch.item;
                    if (slotType.Contains("1") && (isSuitableTwoHandedItem["twoHandedWeapon1"].Contains(weapon.weaponType)) ||
                        slotType.Contains("2") && (isSuitableTwoHandedItem["twoHandedWeapon2"].Contains(weapon.weaponType)))
                    {
                        pair.DeleteItem();
                        equip.ChangeCharacterEquipment(null, slotType);
                        return;
                    }
                }
            }
            equip.ChangeCharacterEquipment(null, slotType);
        }
    }


    public virtual void Initialize(EquipmentWindow equipWindow, ItemBunch _itemBunch)
    {
        base.Initialize(equipWindow);
        base.AddItem(_itemBunch);
    }



}
