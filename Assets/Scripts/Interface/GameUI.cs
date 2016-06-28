using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Интерфейс, что видит игрок во время игры
/// </summary>
public class GameUI : InterfaceWindow
{

    #region consts

    private const float maxHealthWidth = 230f;

    #endregion //consts

    #region parametres

    private float maxHealth = 0f;
    private float partnerMaxHealth = 0f;

    #endregion //parametres

    #region fields

    private Text roomNumberText, heightText, healthText;
    private RectTransform healthBar;
    private List<Image> buffsImages = new List<Image>();
    private Image rightWeaponImage, leftWeaponImage, itemImage;
    private GameObject messagePanel;
    private Text message1, message2;

    private Text partnerName, partnerHealthText;
    private RectTransform partnerHealthBar;
    private List<Image> partnerBuffsImages = new List<Image>();
    private Image partnerRightWeaponImage, partnerLeftWeaponImage, partnerItemImage;

    private PersonController player, partner;
    private Transform playerTrans;

    private OrganismStats playerStats, partnerStats;

    #endregion //fields

    public void Start()
    {
        Initialize();
    }

    public void Update()
    {
        heightText.text = GetHeight(playerTrans.position.y).ToString();
    }
    
    public override void Initialize()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PersonController>();

        #region roomData

        playerTrans = player.transform;
        roomNumberText = transform.Find("RoomNumberText").GetComponent<Text>();
        AreaID roomID = player.currentRoom.id;
        player.RoomChangedEvent += HandleRoomChangedEvent;
        roomNumberText.text = roomID.plane + "." + roomID.floor.ToString() + "." + roomID.room.ToString();
        heightText = transform.Find("HeightText").GetComponent<Text>();

        #endregion //roomData

        #region hpData

        playerStats = player.GetOrgStats();
        maxHealth = playerStats.maxHealth;
        playerStats.HealthChangedEvent += HandleHealthChangedEvent;
        healthText = transform.Find("HealthText").GetComponent<Text>();
        healthBar = transform.Find("Health").GetComponent<RectTransform>();

        #endregion //hpData

        #region buffsPanel

        buffsImages.Clear();
        Transform buffsPanel = transform.Find("BuffsPanel");
        for (int i = 0; i < buffsPanel.childCount; i++)
        {
            buffsImages.Add(buffsPanel.GetChild(i).GetComponent<Image>());
        }
        player.buffList.BuffsChangedEvent += HandleBuffsChangedEvent;

        #endregion //buffsPanel

        #region itemsPanel

        Transform itemsPanel = transform.Find("ItemsPanel");
        rightWeaponImage = itemsPanel.FindChild("RightWeaponPanel").FindChild("RightWeaponImage").GetComponent<Image>();
        leftWeaponImage = itemsPanel.FindChild("LeftWeaponPanel").FindChild("LeftWeaponImage").GetComponent<Image>();
        itemImage = itemsPanel.FindChild("ItemPanel").FindChild("ItemImage").GetComponent<Image>();
        EquipmentClass equip = (EquipmentClass)player.GetEquipment();
        if (equip.rightWeapon != null)
        {
            rightWeaponImage.sprite = equip.rightWeapon.image;
            rightWeaponImage.color = new Color(1f,1f,1f,1f);
        }
        if (equip.leftWeapon != null)
        {
            leftWeaponImage.sprite = equip.leftWeapon.image;
            leftWeaponImage.color = new Color(1f, 1f, 1f, 1f);
        }
        if (equip.useItem != null)
        {
            if (equip.useItem.item != null)
            {
                itemImage.sprite = equip.useItem.item.image;
                itemImage.color = new Color(1f, 1f, 1f, 1f);
            }
        }
        equip.ActiveItemChangedEvent += HandleItemChangedEvent;

        #endregion //itemsPanel

        #region messagePanel

        messagePanel = transform.FindChild("MessagePanel").gameObject;
        message1 = messagePanel.transform.FindChild("Message1").GetComponent<Text>();
        message2 = messagePanel.transform.FindChild("Message2").GetComponent<Text>();
        messagePanel.SetActive(false);
        GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameStatistics>().MessageSentEvent += HandleMessageSentEvent;
        #endregion //messagePanel

        ((KeyboardActorController)player).OnAddPartner += AddPartner;
        ((KeyboardActorController)player).OnRemovePartner += RemovePartner;

    }

    /// <summary>
    /// Добавление интерфейса партнёра
    /// </summary>
    public void AddPartner(object sender, PartnerEventArgs _partner)
    {
        if (_partner == null) return;
        if (partner != null)
        {
            RemovePartner(null, null);
        }
        transform.Find("Partner").gameObject.SetActive(true);
        partner = _partner.partner;
        transform.Find("Partner").Find("PName").GetComponent<Text>().text = ((PartnerController)partner).partnerName;

        #region hpData

        partnerStats = partner.GetOrgStats();
        partnerMaxHealth = partnerStats.maxHealth;
        partnerStats.HealthChangedEvent += HandlePartnerHealthChangedEvent;
        partnerHealthText = GameObject.Find("PHealthText").GetComponent<Text>();
        partnerHealthBar = GameObject.Find("PHealth").GetComponent<RectTransform>();

        #endregion //hpData

        #region buffsPanel

        partnerBuffsImages.Clear();
        Transform partnerBuffsPanel = GameObject.Find("PBuffsPanel").transform;
        for (int i = 0; i < partnerBuffsPanel.childCount; i++)
        {
            partnerBuffsImages.Add(partnerBuffsPanel.GetChild(i).GetComponent<Image>());
        }
        partner.buffList.BuffsChangedEvent += HandlePartnerBuffsChangedEvent;

        #endregion //buffsPanel

        #region itemsPanel

        Transform itemsPanel = GameObject.Find("PItemsPanel").transform;
        partnerRightWeaponImage = itemsPanel.FindChild("RightWeaponPanel").FindChild("RightWeaponImage").GetComponent<Image>();
        partnerLeftWeaponImage = itemsPanel.FindChild("LeftWeaponPanel").FindChild("LeftWeaponImage").GetComponent<Image>();
        partnerItemImage = itemsPanel.FindChild("ItemPanel").FindChild("ItemImage").GetComponent<Image>();
        EquipmentClass equip = (EquipmentClass)partner.GetEquipment();
        if (equip.rightWeapon != null)
        {
            partnerRightWeaponImage.sprite = equip.rightWeapon.image;
            partnerRightWeaponImage.color = new Color(1f, 1f, 1f, 1f);
        }
        if (equip.leftWeapon != null)
        {
            partnerLeftWeaponImage.sprite = equip.leftWeapon.image;
            partnerLeftWeaponImage.color = new Color(1f, 1f, 1f, 1f);
        }
        if (equip.useItem != null)
        {
            if (equip.useItem.item != null)
            {
                partnerItemImage.sprite = equip.useItem.item.image;
                partnerItemImage.color = new Color(1f, 1f, 1f, 1f);
            }
        }
        equip.ActiveItemChangedEvent += HandlePartnerItemChangedEvent;

        #endregion //itemsPanel
    }

    /// <summary>
    /// Удаление интерфейса партнёра
    /// </summary>
    public void RemovePartner(object sender, PartnerEventArgs _p)
    {
        if (partner == null) return;
        partnerStats.HealthChangedEvent -= HandlePartnerHealthChangedEvent;
        partner.buffList.BuffsChangedEvent -= HandlePartnerBuffsChangedEvent;
        ((EquipmentClass)partner.GetEquipment()).ActiveItemChangedEvent -= HandlePartnerItemChangedEvent;
        transform.Find("Partner").gameObject.SetActive(false);
        
    }

    /// <summary>
    /// Мониторим здоровье персонажа
    /// </summary>
    void HandleHealthChangedEvent(object sender, OrganismEventArgs e)
    {
        healthBar.sizeDelta = new Vector2(maxHealthWidth * e.HP / maxHealth, healthBar.sizeDelta.y);
        healthText.text = e.HP.ToString() + "/" + maxHealth.ToString();    
    }

    /// <summary>
    /// Мониторим здоровье партнёра
    /// </summary>
    void HandlePartnerHealthChangedEvent(object sender, OrganismEventArgs e)
    {
        partnerHealthBar.sizeDelta = new Vector2(maxHealthWidth * e.HP / partnerMaxHealth, partnerHealthBar.sizeDelta.y);
        partnerHealthText.text = e.HP.ToString() + "/" + partnerMaxHealth.ToString();
    }

    /// <summary>
    /// Мониторим местонахождение персонажа
    /// </summary>
    void HandleRoomChangedEvent(object sender, RoomChangedEventArgs e)
    {
        AreaID roomID = e.Room.id;
        roomNumberText.text = roomID.plane + "." + roomID.floor.ToString() + "." + roomID.room.ToString();
    }

    /// <summary>
    /// Мониторим вооружение персонажа
    /// </summary>
    void HandleItemChangedEvent(object sender, ItemChangedEventArgs e)
    {
        ItemClass item = e.Item;
        string itemType = e.ItemType;
        if (string.Equals(itemType, "rightWeapon"))
        {
            SetImage(ref rightWeaponImage, e.Item);
        }
        else if (string.Equals(itemType, "leftWeapon"))
        {
            SetImage(ref leftWeaponImage, e.Item);
        }
        else if (string.Equals(itemType, "usable"))
        {
            SetImage(ref itemImage, e.ItemBunch);
        }
    }

    /// <summary>
    /// Мониторим вооружение партнёра
    /// </summary>
    void HandlePartnerItemChangedEvent(object sender, ItemChangedEventArgs e)
    {
        ItemClass item = e.Item;
        string itemType = e.ItemType;
        if (string.Equals(itemType, "rightWeapon"))
        {
            SetImage(ref partnerRightWeaponImage, e.Item);
        }
        else if (string.Equals(itemType, "leftWeapon"))
        {
            SetImage(ref partnerLeftWeaponImage, e.Item);
        }
        else if (string.Equals(itemType, "usable"))
        {
            SetImage(ref partnerItemImage, e.ItemBunch);
        }
    }

    void SetImage(ref Image _image, ItemBunch _itemBunch)
    {
        if (_itemBunch == null ? true : _itemBunch.quantity <= 0)
        {
            _image.sprite = null;
            _image.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            _image.sprite = _itemBunch.item.image;
            _image.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    void SetImage(ref Image _image, ItemClass _item)
    {
        if (_item == null )
        {
            _image.sprite = null;
            _image.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            _image.sprite = _item.image;
            _image.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    /// <summary>
    /// Мониторим баффы персонажа
    /// </summary>
    void HandleBuffsChangedEvent(object sender, BuffsChangedEventArgs e)
    {
        BuffsList buffs = player.buffList;
        for (int i = 0; i < buffsImages.Count; i++)
        {
            if (i < buffs.Count)
            {
                buffsImages[i].sprite = buffs[i].buffImage;
                buffsImages[i].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                buffsImages[i].sprite = null;
                buffsImages[i].color = new Color(1f, 1f, 1f, 0f);
            }
        }
    }

    /// <summary>
    /// Мониторим баффы напарника
    /// </summary>
    void HandlePartnerBuffsChangedEvent(object sender, BuffsChangedEventArgs e)
    {
        BuffsList buffs = partner.buffList;
        for (int i = 0; i < partnerBuffsImages.Count; i++)
        {
            if (i < buffs.Count)
            {
                partnerBuffsImages[i].sprite = buffs[i].buffImage;
                partnerBuffsImages[i].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                partnerBuffsImages[i].sprite = null;
                partnerBuffsImages[i].color = new Color(1f, 1f, 1f, 0f);
            }
        }
    }

    /// <summary>
    /// Мониторить важные игровые сообщения, которые должен увидеть игрок.
    /// </summary>
    void HandleMessageSentEvent(object sender, MessageSentEventArgs e)
    {
        if (e.TextNumb == 3)
        {
            messagePanel.SetActive(true);
            message2.text = e.Message;
        }
        else if (e.TextNumb == 4)
        {
            message2.text = "";
            messagePanel.SetActive(false);
        }
        StartCoroutine(GetMessage(e.Message, e.TextNumb, e.TextTime));
    }

    IEnumerator GetMessage(string text, int textNumb, float textTime)
    {
        messagePanel.SetActive(true);
        if (textNumb == 1)
        {
            message1.text = text;
        }
        else if (textNumb==2)
        {
            message2.text = text;
        }
        yield return new WaitForSeconds(textTime);
        if ((textNumb == 1) && (string.Equals(message1.text, text)))
        {
            message1.text = "";
        }
        else if ((textNumb == 2) && (string.Equals(message2.text, text)))
        {
            message2.text = "";
        }
        if ((string.Equals(message1.text, "")) && (string.Equals(message2.text, "")))
        {
            messagePanel.SetActive(false);
        }
    }


    float GetHeight (float y)
    {
        return y;
    }

}
