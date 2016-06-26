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
    public ItemSlot useItemSlot1, useItemSlot2, useItemSlot3, useItemSlot4;

    #endregion //characterDoll

    #region parameters

    private Text healthText;
    private Text parametersText;

    #endregion //parameters

    #region bag

    private List<EquipmentSlot> bagSlots = new List<EquipmentSlot>();
    public List<EquipmentSlot> BagSlots {get { return bagSlots; }}
    private Text goldText, ironKeyText, silverKeyText, goldKeyText;

    #endregion //bag

    #endregion //fields

    public void Awake()
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
        useItemSlot1 = characterDollPanel.FindChild("UsableItemSlot1").GetComponent<ItemSlot>();
        useItemSlot1.Initialize(this);
        useItemSlot2 = characterDollPanel.FindChild("UsableItemSlot2").GetComponent<ItemSlot>();
        useItemSlot2.Initialize(this);
        useItemSlot3 = characterDollPanel.FindChild("UsableItemSlot3").GetComponent<ItemSlot>();
        useItemSlot3.Initialize(this);
        useItemSlot4 = characterDollPanel.FindChild("UsableItemSlot4").GetComponent<ItemSlot>();
        useItemSlot4.Initialize(this);

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
        Transform keysPanel = equipmentPanel.FindChild("BagPanel").FindChild("KeysPanel");
        goldText = keysPanel.FindChild("GoldText").GetComponent<Text>();
        ironKeyText = keysPanel.FindChild("IronKeyText").GetComponent<Text>();
        silverKeyText = keysPanel.FindChild("SilverKeyText").GetComponent<Text>();
        goldKeyText = keysPanel.FindChild("GoldKeyText").GetComponent<Text>();

        #endregion //bag

        #region events

        orgStats.HealthChangedEvent += HealthChangedEventHandler;
        orgStats.ParametersChangedEvent += ParametersChangedEventHandler;
        equip.ActiveItemChangedEvent += ActiveItemChangedEventHandler;
        equip.BagChangedEvent += BagChangedEventHandler;
        equip.ResourceChangedEvent += ResourceChangedEventHandler;

        #endregion //events

    }

    /// <summary>
    /// Проинициализировать куклу персонажа, используя его equipment
    /// </summary>
    public void InitializeCharacterDoll()
    {
        Transform equipmentPanel = transform.FindChild("EquipmentPanel");
        Transform characterDollPanel = equipmentPanel.FindChild("CharacterDollPanel").FindChild("CharacterDoll");

        InitializeSlot(characterDollPanel, "HelmetSlot", equip.armor.helmet);
        InitializeSlot(characterDollPanel, "CuirassSlot", equip.armor.cuirass);
        InitializeSlot(characterDollPanel, "PantsSlot", equip.armor.pants);
        InitializeSlot(characterDollPanel, "BootsSlot", equip.armor.boots);
        InitializeSlot(characterDollPanel, "GlovesSlot", equip.armor.gloves);
        InitializeSlot(characterDollPanel, "LeftRingSlot", equip.armor.leftRing);
        InitializeSlot(characterDollPanel, "RightRingSlot", equip.armor.rightRing);
        useItemSlot1.Initialize(this, new ItemBunch(equip.useItems[0]!=null?equip.useItems[0].item:null));
        useItemSlot2.Initialize(this, new ItemBunch(equip.useItems[1] != null ? equip.useItems[1].item : null));
        useItemSlot3.Initialize(this, new ItemBunch(equip.useItems[2] != null ? equip.useItems[2].item : null));
        useItemSlot4.Initialize(this, new ItemBunch(equip.useItems[3] != null ? equip.useItems[3].item : null));
        rightWeaponSlot1.Initialize(this, new ItemBunch(equip.rightWeapon));
        rightWeaponSlot2.Initialize(this, new ItemBunch(equip.altRightWeapon));
        leftWeaponSlot1.Initialize(this, new ItemBunch(equip.leftWeapon));
        leftWeaponSlot2.Initialize(this, new ItemBunch(equip.altLeftWeapon));
    }

    /// <summary>
    /// Проинициализировать сумку персонажа, используя его equipment.bag
    /// </summary>
    public void InitializeBag(bool fillBag)
    {
        Transform equipmentPanel = transform.FindChild("EquipmentPanel");
        Transform bagPanel = equipmentPanel.FindChild("BagPanel").FindChild("BagScroll").FindChild("Bag");
        if (fillBag)//Если игрок в первый раз начинает игру, то будем считать, что его предметы в рюкзаке расположатся по порядкуы
        {
            for (int i = 0; i < equip.bag.Count; i++)
            {
                AddItemInBag(equip.bag[i]);
            }
        }

        goldText.text = equip.gold.ToString();
        ironKeyText.text = equip.keys[0].ToString();
        silverKeyText.text = equip.keys[1].ToString();
        goldKeyText.text = equip.keys[2].ToString();

    }

    /// <summary>
    /// Добавить предмет в рюкзак в свободное место
    /// </summary>
    public void AddItemInBag(ItemBunch itemBunch)
    {
        if (itemBunch != null)
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
    }

    /// <summary>
    /// Добавить предмет в рюкзак в определённом месте
    /// </summary>
    public void AddItemInBag(ItemBunch itemBunch, int index)
    {
        if (itemBunch != null)
        {
            bagSlots[index].AddItem(itemBunch);
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
        else if (e.ItemType.Contains("usableItem"))
        {
            if (string.Equals(e.ItemType, "usableItem1"))
            {
                useItemSlot1.Initialize(this, e.ItemBunch);
            }
            else if (string.Equals(e.ItemType, "usableItem2"))
            {
                useItemSlot2.Initialize(this, e.ItemBunch);
            }
            else if (string.Equals(e.ItemType, "usableItem3"))
            {
                useItemSlot3.Initialize(this, e.ItemBunch);
            }
            else if (string.Equals(e.ItemType, "usableItem4"))
            {
                useItemSlot4.Initialize(this, e.ItemBunch);
            }
        }
    }

    void BagChangedEventHandler(object sender, ItemChangedEventArgs e)
    {
        AddItemInBag(e.ItemBunch);
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
