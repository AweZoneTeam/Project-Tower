using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Окно обмена предметами между двумя объектами класса BagClass
/// </summary>
public class InterfaceExchangeWindow : InterfaceWindow
{

    #region consts

    const float maxDistance = 40f;

    #endregion //consts

    #region fields

    protected BagClass bag;
    protected EquipmentClass equip;

    protected Transform exchanger1 = null, exchanger2 = null;
    protected Canvas canvas;

    protected Transform bagPanel;

    public float distance;

    private ExchangeSlotScript lastSlot;
    public ExchangeSlotScript LastSlot
    {
        get { return lastSlot; }
        set { lastSlot = value; }
    }

    protected List<ExchangeSlotScript> slotsToTake = new List<ExchangeSlotScript>();
    public List<ExchangeSlotScript> SlotsToTake
    {
        get { return slotsToTake; }
    }

    #endregion //fields

    public void Update()
    {
        if (canvas.enabled)
        {
            if ((exchanger1 != null) && (exchanger2 != null))
            {
                distance = Vector3.Distance(exchanger1.position, exchanger2.position);
                if (Vector3.Distance(exchanger1.position, exchanger2.position) > maxDistance)
                {
                    canvas.enabled = false;
                }
            }
        }
    }

    public void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        exchanger1 = null;
        exchanger2 = null;
        canvas = GetComponent<Canvas>();
        bagPanel = transform.FindChild("Bag");
        ExchangeSlotScript slot;
        for (int i = 0; i < bagPanel.childCount; i++)
        {
            slot = bagPanel.GetChild(i).GetComponentInChildren<ExchangeSlotScript>();
            slot.Initialize(this);
        }
        slotsToTake = new List<ExchangeSlotScript>();
    }

    /// <summary>
    /// Установить набор предметов для обмена
    /// </summary>
    public void SetBag(BagClass _bag, EquipmentClass _equip, Transform _exchanger1, Transform _exchanger2)
    {
        exchanger1 = _exchanger1;
        exchanger2 = _exchanger2;
        bag = _bag;
        equip = _equip;
        ExchangeSlotScript slot;
        bagPanel = transform.FindChild("Bag");
        for (int i = 0; i < bagPanel.childCount; i++)
        {
            slot = bagPanel.GetChild(i).GetComponentInChildren<ExchangeSlotScript>();
            if (i < bag.bag.Count)
            {
                slot.InitializeSlot(bag.bag[i]);
            }
            else
            {
                slot.InitializeSlot(null);
            }
        }
        slotsToTake.Clear();
        lastSlot = null;
    }

    /// <summary>
    /// Активный персонаж забирает предметы во всех выделенных слотах
    /// </summary>
    public void TakeItem()
    {
        if (bag.bag.Count > 0)
        {
            for (int i = 0; i < slotsToTake.Count; i++)
            {
                equip.TakeItem(slotsToTake[i].Item);
                bag.bag.Remove(slotsToTake[i].Item);
                slotsToTake[i].SetChosen(false);
                slotsToTake[i].UseIt();
            }
        }
        slotsToTake.Clear();
    }

    /// <summary>
    /// Активный персонаж забирает предмет, что находится в выбранном слоте
    /// </summary>
    public void TakeItem(ExchangeSlotScript _slot)
    {
        if (_slot != null)
        {
            equip.TakeItem(_slot.Item);
        }
        bag.bag.Remove(_slot.Item);
        _slot.UseIt();
    }

    /// <summary>
    /// Персонаж забирает все предметы в контейнере
    /// </summary>
    public void TakeAllItems()
    {
        ExchangeSlotScript slot;
        bagPanel = transform.FindChild("Bag");
        if (bag.bag.Count > 0)
        {
            for (int i = bag.bag.Count-1; i >=0 ; i--)
            {
                equip.TakeItem(bag.bag[i]);
                bag.bag.Remove(bag.bag[i]);
            }
        }
        for (int i = 0; i < bagPanel.childCount; i++)
        {
            slot = bagPanel.GetChild(i).GetComponentInChildren<ExchangeSlotScript>();
            slot.UseIt();
        }
        slotsToTake.Clear();
    }

    /// <summary>
    /// Выйти из экрана
    /// </summary>
    public void Exit()
    {
        gameObject.GetComponent<Canvas>().enabled=false;
    }
}
