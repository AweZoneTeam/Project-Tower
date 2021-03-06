﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Окно, отвечающее за инвентарь персонажа
/// </summary>
public class EquipmentWindow : InterfaceWindow
{

    #region consts

    private const float mouseAreaWidth=900f;
    private const float mouseAreaHeight = 500f;

    #endregion //consts

    #region fields

    private Camera cam;

    private KeyboardActorController player;
    private EquipmentClass equip;
    private OrganismStats orgStats;

    private EquipmentSlot currentSlot;
    public EquipmentSlot CurrentSlot
    {
        get { return currentSlot; }
        set
        {
            currentSlot = value;
            if (currentSlot != null)
            {
                if (currentSlot.itemBunch != null)
                {
                    itemImage.sprite = currentSlot.itemBunch.item.image;
                    itemImage.color = new Vector4(1f, 1f, 1f, 1f);
                    itemDescription = currentSlot.itemBunch.item.description;
                    itemParametres = currentSlot.itemBunch.item.parametres;
                    itemDescriptionText.text = visualizeDescription ? itemDescription : itemParametres;
                }
            }
        }
    }

    private Image mouseImage;
    public Image MouseImage
    {
        get { return mouseImage; }
        set { mouseImage = value; }
    }

    #region itemDescription

    private Text itemNameText;
    private Image itemImage;
    private string itemDescription;
    private string itemParametres;
    private bool visualizeDescription;
    private Text itemDescriptionText;

    #endregion //itemDescription

    #region characterDoll

    private ItemSlot rightWeaponSlot1, rightWeaponSlot2, leftWeaponSlot1, leftWeaponSlot2;
    private List<ItemSlot> usableItemSlots=new List<ItemSlot>();

    #endregion //characterDoll

    #region parameters

    private Text healthText;
    private Text parametersText;

    #endregion //parameters

    #region bag

    private List<EquipmentSlot> bagSlots = new List<EquipmentSlot>();
    private Text goldText, ironKeyText, silverKeyText, goldKeyText;

    #endregion //bag

    #endregion //fields

    public void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {

        cam = GameObject.FindGameObjectWithTag(Tags.cam).GetComponent<Camera>();

        player = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<KeyboardActorController>();
        equip = (EquipmentClass)player.GetEquipment();
        orgStats = player.GetOrgStats();
        Transform equipmentPanel = transform.FindChild("EquipmentPanel");

        mouseImage = equipmentPanel.FindChild("MouseImage").GetComponent<Image>();

        #region itemDescription

        Transform itemDescriptionPanel = equipmentPanel.FindChild("ItemDescriptionPanel");
        itemNameText = itemDescriptionPanel.FindChild("ItemNameText").GetComponent<Text>();
        itemImage = itemDescriptionPanel.FindChild("ItemImagePanel").FindChild("Image").GetComponent<Image>();
        itemDescription = "";
        itemParametres = "";
        visualizeDescription = true;
        itemDescriptionText = itemDescriptionPanel.FindChild("Description").FindChild("DescriptionScroll").FindChild("DescriptionText").GetComponent<Text>();

        #endregion //itemDescription

        #region characterDoll

        Transform characterDollPanel = equipmentPanel.FindChild("CharacterDollPanel").FindChild("CharacterDoll");
        rightWeaponSlot1 = characterDollPanel.FindChild("RightWeaponSlot1").GetComponent<ItemSlot>();
        rightWeaponSlot1.Initialize(this);
        rightWeaponSlot2 = characterDollPanel.FindChild("RightWeaponSlot2").GetComponent<ItemSlot>();
        rightWeaponSlot2.Initialize(this);
        leftWeaponSlot1 = characterDollPanel.FindChild("LeftWeaponSlot1").GetComponent<ItemSlot>();
        leftWeaponSlot1.Initialize(this);
        leftWeaponSlot2 = characterDollPanel.FindChild("LeftWeaponSlot2").GetComponent<ItemSlot>();
        leftWeaponSlot2.Initialize(this);
        usableItemSlots.Add(characterDollPanel.FindChild("UsableItemSlot1").GetComponent<ItemSlot>());
        usableItemSlots.Add(characterDollPanel.FindChild("UsableItemSlot2").GetComponent<ItemSlot>());
        usableItemSlots.Add(characterDollPanel.FindChild("UsableItemSlot3").GetComponent<ItemSlot>());
        usableItemSlots.Add(characterDollPanel.FindChild("UsableItemSlot4").GetComponent<ItemSlot>());
        foreach (ItemSlot slot in usableItemSlots)
        {
            slot.Initialize(this);
        }

        InitializeSlot(characterDollPanel, "HelmetSlot", equip.armor.helmet);
        InitializeSlot(characterDollPanel, "CuirassSlot", equip.armor.cuirass);
        InitializeSlot(characterDollPanel, "PantsSlot", equip.armor.pants);
        InitializeSlot(characterDollPanel, "BootsSlot", equip.armor.boots);
        InitializeSlot(characterDollPanel, "GlovesSlot", equip.armor.gloves);
        InitializeSlot(characterDollPanel, "LeftRingSlot", equip.armor.leftRing);
        InitializeSlot(characterDollPanel, "RightRingSlot", equip.armor.rightRing);
        rightWeaponSlot1.Initialize(this,new ItemBunch(equip.rightWeapon));
        rightWeaponSlot2.Initialize(this,new ItemBunch(equip.altRightWeapon));
        leftWeaponSlot1.Initialize(this,new ItemBunch(equip.leftWeapon));
        leftWeaponSlot2.Initialize(this,new ItemBunch(equip.altLeftWeapon));
        for (int i=0; i<usableItemSlots.Count;i++)
        {
            usableItemSlots[i].Initialize(this, equip.useItems[i]);
        }


        #endregion //characterDoll

        #region parameters

        Transform ParametersPanel = equipmentPanel.FindChild("ParametersPanel");
        parametersText = ParametersPanel.FindChild("ParametersText").GetComponent<Text>();
        healthText = ParametersPanel.FindChild("HealthText").GetComponent<Text>();
        healthText.text = "Здоровье " + orgStats.health.ToString() + "/" + orgStats.maxHealth.ToString();
        parametersText.text = "Защита: \n физическая " + orgStats.defence.pDefence.ToString() + "\n огненная " + orgStats.defence.fDefence.ToString() +
                            "\n ядовитая " + orgStats.defence.aDefence.ToString() + "\n теневая " + orgStats.defence.dDefence.ToString() +
                            "\n\n Устойчивость " + orgStats.defence.stability + "\n\n Скорость " + orgStats.velocity.ToString();

        #endregion //parameters

        #region bag

        Transform bagPanel = equipmentPanel.FindChild("BagPanel").FindChild("BagScroll").FindChild("Bag");
        EquipmentSlot equipSlot;
        bagSlots.Clear();
        for (int i = 0; i < bagPanel.childCount; i++)
        {
            equipSlot = bagPanel.GetChild(i).GetComponentInChildren<EquipmentSlot>();
            bagSlots.Add(equipSlot);
            equipSlot.Initialize(this);
        }
        for (int i = 0; i < equip.bag.Count; i++)
        {
            AddItemInBag(equip.bag[i]);
        }
        Transform keysPanel = equipmentPanel.FindChild("BagPanel").FindChild("KeysPanel");
        goldText = keysPanel.FindChild("GoldText").GetComponent<Text>();
        goldText.text = equip.gold.ToString();
        ironKeyText = keysPanel.FindChild("IronKeyText").GetComponent<Text>();
        ironKeyText.text = equip.keys[0].ToString();
        silverKeyText = keysPanel.FindChild("SilverKeyText").GetComponent<Text>();
        silverKeyText.text = equip.keys[1].ToString();
        goldKeyText = keysPanel.FindChild("GoldKeyText").GetComponent<Text>();
        goldKeyText.text = equip.keys[2].ToString();

        #endregion //bag

        #region events

        orgStats.HealthChangedEvent += HealthChangedEventHandler;
        orgStats.ParametersChangedEvent += ParametersChangedEventHandler;
        equip.ActiveItemChangedEvent += ActiveItemChangedEventHandler;
        equip.ResourceChangedEvent += ResourceChangedEventHandler;

        #endregion //events

    }

    /// <summary>
    /// Функция добавления нового предмета в рюкзак
    /// </summary>
    void AddItemInBag(ItemBunch itemBunch)
    {
        for (int i = 0; i < bagSlots.Count; i++)
        {
            if (bagSlots[i].itemBunch == null)
            {
                bagSlots[i].AddItem(itemBunch);
                break;
            }
        }
    }

    /// <summary>
    /// Инициализировать слот
    /// </summary>
    public void InitializeSlot(Transform trans, string childName, ItemClass item)
    {
        ItemSlot slot = trans.FindChild(childName).GetComponent<ItemSlot>();
        slot.Initialize(this, new ItemBunch(item));
    }

    private EquipmentSlot GetSlot(Transform trans, string childName)
    {
        return trans.FindChild(childName).GetComponent<EquipmentSlot>();
    }

    /// <summary>
    /// Сменить какой-то предмет в экипировке персонажа
    /// </summary>
    public void ChangeCharacterEquipment(ItemBunch itemBunch, string changeType)
    {
        player.ChangeItem(itemBunch, changeType);
    }

    /// <summary>
    /// В окне описания предмета задать описание интересующего предмета
    /// </summary>
    public void ChangeDescription(bool _visualizeDescription)
    {
        if (_visualizeDescription)
        {
            itemDescriptionText.text = itemDescription;
            visualizeDescription = true;
        }
        else
        {
            itemDescriptionText.text = itemParametres;
            visualizeDescription = false;
        }
    }

    /// <summary>
    /// Сменить положение изображения перетаскиваемого предмета
    /// </summary>
    public void ChangeMouseImagePosition()
    {
        Vector3 pos = Input.mousePosition;
        pos.x -= cam.pixelWidth/2;
        pos.x = pos.x / cam.pixelWidth * mouseAreaWidth;
        pos.y -= cam.pixelHeight/2;
        pos.y = pos.y / cam.pixelHeight*mouseAreaHeight;
        mouseImage.transform.localPosition = pos;
    }

    #region eventHandlers

    /// <summary>
    /// Обработчик события "Изменение ХП"
    /// </summary>
    void HealthChangedEventHandler(object sender, OrganismEventArgs e)
    {
        healthText.text = "Здоровье " + e.HP.ToString() + "/" + e.MAXHP.ToString();
    }

    /// <summary>
    /// Обработчик события "Изменение параметров"
    /// </summary>
    void ParametersChangedEventHandler(object sender, OrganismEventArgs e)
    {
        healthText.text = "Здоровье " + e.HP.ToString() + "/" + e.MAXHP.ToString();
        parametersText.text="Защита: \n физическая "+ e.DEFENCE.pDefence.ToString()+"\n огненная "+ e.DEFENCE.fDefence.ToString()+
                            "\n ядовитая "+ e.DEFENCE.aDefence.ToString()+"\n теневая "+ e.DEFENCE.dDefence.ToString()+ 
                            "\n\n Устойчивость "+ e.DEFENCE.stability + "\n\n Скорость "+e.VELOCITY.ToString();
    }

    /// <summary>
    /// Обработчик события "Смена экипировки"
    /// </summary>
    void ActiveItemChangedEventHandler(object sender, ItemChangedEventArgs e)
    {
        if (string.Equals(e.ItemType, "rightWeapon"))
        {
            ItemBunch _itemBunch = rightWeaponSlot1.itemBunch;
            rightWeaponSlot1.Initialize(this, rightWeaponSlot2.itemBunch);
            rightWeaponSlot2.Initialize(this, _itemBunch);
        }
        else if (string.Equals(e.ItemType, "leftWeapon"))
        {
            ItemBunch _itemBunch = leftWeaponSlot1.itemBunch;
            leftWeaponSlot1.Initialize(this, leftWeaponSlot2.itemBunch);
            leftWeaponSlot2.Initialize(this, _itemBunch);
        }
        else if (string.Equals(e.ItemType, "usable"))
        {
            foreach (ItemSlot slot in usableItemSlots)
            {
                if (slot.itemBunch != null? slot.itemBunch.quantity <= 0: true)
                {
                    slot.Remove();
                }
            }
        }
    }

    /// <summary>
    /// Обработчик события "изменение числа ресурсов"
    /// </summary>
    void ResourceChangedEventHandler(object sender, ResourceChangedEventArgs e)
    {
        goldText.text = e.Gold.ToString();
        ironKeyText.text = e.IronKey.ToString();
        silverKeyText.text = e.SilverKey.ToString();
        goldKeyText.text = e.GoldKey.ToString();
    }

    #endregion //eventHandlers

}
