using UnityEngine;
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

    private const float width = 500f;
    private const float height = 500f;

    #endregion //consts

    #region fields

    private Camera cam;

    private KeyboardActorController player;
    public EquipmentClass equip;
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

    public Texture2D defaultCursor;

    #region itemDescription

    private Text itemNameText;
    private Image itemImage;
    private string itemDescription;
    private string itemParametres;
    private bool visualizeDescription;
    private Text itemDescriptionText;

    #endregion //itemDescription

    #region characterDoll

    public ItemSlot rightWeaponSlot1, rightWeaponSlot2, leftWeaponSlot1, leftWeaponSlot2;

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
        if (defaultCursor != null)
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }

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

        InitializeSlot(characterDollPanel, "HelmetSlot", equip.armor.helmet);
        InitializeSlot(characterDollPanel, "CuirassSlot", equip.armor.cuirass);
        InitializeSlot(characterDollPanel, "PantsSlot", equip.armor.pants);
        InitializeSlot(characterDollPanel, "BootsSlot", equip.armor.boots);
        InitializeSlot(characterDollPanel, "GlovesSlot", equip.armor.gloves);
        InitializeSlot(characterDollPanel, "LeftRingSlot", equip.armor.leftRing);
        InitializeSlot(characterDollPanel, "RightRingSlot", equip.armor.rightRing);
        InitializeSlot(characterDollPanel, "UsableItemSlot1", equip.useItems[0].item);
        InitializeSlot(characterDollPanel, "UsableItemSlot2", equip.useItems[1].item);
        InitializeSlot(characterDollPanel, "UsableItemSlot3", equip.useItems[2].item);
        InitializeSlot(characterDollPanel, "UsableItemSlot4", equip.useItems[3].item);
        rightWeaponSlot1.Initialize(this, new ItemBunch(equip.rightWeapon));
        rightWeaponSlot2.Initialize(this, new ItemBunch(equip.altRightWeapon));
        leftWeaponSlot1.Initialize(this, new ItemBunch(equip.leftWeapon));
        leftWeaponSlot2.Initialize(this, new ItemBunch(equip.altLeftWeapon));


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

    public void AddItemInBag(ItemBunch itemBunch)
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
    /// проверяет есть ли нужное количество сободных слотов
    /// </summary>
    public bool HaveEmptySlots(byte count)
    {
        byte _count = 0;
        for (int i = 0; i < bagSlots.Count; i++)
        {
            if (bagSlots[i].itemBunch == null)
            {
                _count++;
            }
        }
        return count <= _count;
    }

    public void InitializeSlot(Transform trans, string childName, ItemClass item)
    {
        ItemSlot slot = trans.FindChild(childName).GetComponent<ItemSlot>();
        slot.Initialize(this, new ItemBunch(item));
    }

    public void ChangeCharacterEquipment(ItemBunch itemBunch, string changeType)
    {
        player.ChangeItem(itemBunch, changeType);
    }

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

    #region eventHandlers

    void HealthChangedEventHandler(object sender, OrganismEventArgs e)
    {
        healthText.text = "Здоровье " + e.HP.ToString() + "/" + e.MAXHP.ToString();
    }

    void ParametersChangedEventHandler(object sender, OrganismEventArgs e)
    {
        healthText.text = "Здоровье " + e.HP.ToString() + "/" + e.MAXHP.ToString();
        parametersText.text = "Защита: \n физическая " + e.DEFENCE.pDefence.ToString() + "\n огненная " + e.DEFENCE.fDefence.ToString() +
                            "\n ядовитая " + e.DEFENCE.aDefence.ToString() + "\n теневая " + e.DEFENCE.dDefence.ToString() +
                            "\n\n Устойчивость " + e.DEFENCE.stability + "\n\n Скорость " + e.VELOCITY.ToString();
    }

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
    }

    void ResourceChangedEventHandler(object sender, ResourceChangedEventArgs e)
    {
        goldText.text = e.Gold.ToString();
        ironKeyText.text = e.IronKey.ToString();
        silverKeyText.text = e.SilverKey.ToString();
        goldKeyText.text = e.GoldKey.ToString();
    }

    #endregion //eventHandlers

}
