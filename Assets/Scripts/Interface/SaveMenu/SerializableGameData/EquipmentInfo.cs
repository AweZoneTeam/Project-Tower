using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Информация об инвентаре персонаже
/// </summary>
[XmlType("EquipmentInfo")]
[XmlInclude(typeof(BagSlotInfo))]
public class EquipmentInfo
{
    [XmlElement("Helmet")]
    public string helmet;

    [XmlElement("Cuirass")]
    public string cuirass;

    [XmlElement("Pants")]
    public string pants;

    [XmlElement("Boots")]
    public string boots;

    [XmlElement("Gloves")]
    public string gloves;

    [XmlElement("RightWeapon1")]
    public string rightWeapon1;

    [XmlElement("LeftWeapon1")]
    public string leftWeapon1;

    [XmlElement("RightWeapon2")]
    public string rightWeapon2;

    [XmlElement("LeftWeapon2")]
    public string leftWeapon2;

    [XmlElement("UseItem1")]
    public BagSlotInfo useItem1;

    [XmlElement("UseItem2")]
    public BagSlotInfo useItem2;

    [XmlElement("UseItem3")]
    public BagSlotInfo useItem3;

    [XmlElement("UseItem4")]
    public BagSlotInfo useItem4;

    [XmlArray("BagInfo")]
    [XmlArrayItem("BagSlotInfo")]
    public List<BagSlotInfo> bagInfo = new List<BagSlotInfo>();

    [XmlElement("Gold")]
    public int gold;

    [XmlElement("IronKeys")]
    public int ironKeys;

    [XmlElement("SilverKeys")]
    public int silverKeys;

    [XmlElement("GoldKeys")]
    public int goldKeys;

    public EquipmentInfo()
    { }

    public EquipmentInfo (EquipmentClass equip, List<EquipmentSlot> bagSlots)
    {
        ArmorSet armor = equip.armor;
        if (armor.helmet != null)
        {
            helmet = armor.helmet.itemName;
        }
        else
        {
            helmet = string.Empty;
        }

        if (armor.cuirass != null)
        {
            cuirass = armor.cuirass.itemName;
        }
        else
        {
            cuirass = string.Empty;
        }

        if (armor.pants != null)
        {
            pants = armor.pants.itemName;
        }
        else
        {
            pants = string.Empty;
        }

        if (armor.gloves != null)
        {
            gloves = armor.gloves.itemName;
        }
        else
        {
            gloves = string.Empty;
        }

        if (armor.boots != null)
        {
            boots = armor.boots.itemName;
        }
        else
        {
            boots = string.Empty;
        }

        if (equip.rightWeapon != null)
        {
            rightWeapon1 = equip.rightWeapon.itemName;
        }
        else
        {
            rightWeapon1 = string.Empty;
        }

        if (equip.leftWeapon != null)
        {
            leftWeapon1 = equip.leftWeapon.itemName;
        }
        else
        {
            leftWeapon1 = string.Empty;
        }

        if (equip.altRightWeapon != null)
        {
            rightWeapon2 = equip.altRightWeapon.itemName;
        }
        else
        {
            rightWeapon2 = string.Empty;
        }

        if (equip.altLeftWeapon != null)
        {
            leftWeapon2 = equip.altLeftWeapon.itemName;
        }
        else
        {
            leftWeapon2 = string.Empty;
        }

        useItem1 = new BagSlotInfo(-1, equip.useItems[0]);
        useItem2 = new BagSlotInfo(-1, equip.useItems[1]);
        useItem3 = new BagSlotInfo(-1, equip.useItems[2]);
        useItem4 = new BagSlotInfo(-1, equip.useItems[3]);

        for (int i=0;i<bagSlots.Count;i++)
        {
            if (bagSlots[i].itemBunch != null)
            {
                bagInfo.Add(new BagSlotInfo(i, bagSlots[i].itemBunch));
            }
        }

        gold = equip.gold;
        ironKeys = equip.keys[0];
        silverKeys = equip.keys[1];
        goldKeys = equip.keys[2];
    }
}

/// <summary>
/// Информация о слоте рюкзака
/// </summary>
[XmlType("BagSlot")]
public class BagSlotInfo
{
    [XmlElement("Index")]
    public int index;

    [XmlElement("Item")]
    public string item;

    [XmlElement("Quantity")]
    public int quantity;

    public BagSlotInfo()
    { }

    public BagSlotInfo(int _index, ItemBunch itemBunch)
    {
        index = _index;
        if (itemBunch != null ? itemBunch.item != null:false)
        {
            item = itemBunch.item.itemName;
            quantity = itemBunch.quantity;
        }
        else
        {
            item = string.Empty;
        }
    }

}

/// <summary>
/// Информация о дропе
/// </summary>
[XmlType("DropInfo")]
[XmlInclude(typeof(BagSlotInfo))]
public class DropInfo
{
    [XmlElement("Position")]
    public Vector3 position;

    [XmlAttribute("AutoPick")]
    public bool autoPick;

    [XmlArray("ItemBunches")]
    [XmlArrayItem("ItemBunch")]
    public List<BagSlotInfo> itemBunches = new List<BagSlotInfo>();

    [XmlElement("Scale")]
    public Vector3 scale;

    [XmlElement("BoxSize")]
    public Vector3 boxSize;

    [XmlElement("GroundCheckPosition")]
    public Vector3 groundCheckPosition;

    public DropInfo()
    { }

    public DropInfo(DropClass drop)
    {
        position = drop.transform.position;
        autoPick = drop.autoPick;

        foreach (ItemBunch itemBunch in drop.drop)
        {
            itemBunches.Add(new BagSlotInfo(-1, itemBunch));
        }

        scale = drop.transform.localScale;
        boxSize = drop.GetComponent<BoxCollider>().size;
        groundCheckPosition = drop.transform.FindChild("GroundCheck").position;

    }

}