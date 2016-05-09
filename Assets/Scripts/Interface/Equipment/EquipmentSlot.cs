using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Класс, ответственный за слоты инвентаря
/// </summary>
public class EquipmentSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{

    protected EquipmentWindow equip;
    protected Image itemImage;
    public ItemSlot pair; //Использовать для слотов с оружием, здесь будет ссылка на вторую руку. 

    protected ItemBunch bunch;
    public ItemBunch itemBunch
    {
        get { return bunch; }
        set { bunch = value; }
    }


    public void Initialize(EquipmentWindow equipWindow)
    {
        equip = equipWindow;
        itemImage = GetComponent<Image>();
        itemImage.color = new Vector4(1f, 1f, 1f, 0f);
    }

    /// <summary>
    /// Что произойдёт, если нажать на слот - будет выбран предмет, либо слоты поменяют своё содержимое местами
    /// </summary>
    public virtual void ChooseItem()
    {
        if (equip.CurrentSlot == null)//Выбор предмета для перетаскивания
        {
            equip.CurrentSlot = this;
        }
        else //Произвести между слотами обмен предметами
        {
            if ((equip.CurrentSlot.IsItemProper(itemBunch)) &&
                (IsItemProper(equip.CurrentSlot.itemBunch)))
            {
                ItemBunch _itemBunch = itemBunch;
                AddItem(equip.CurrentSlot.itemBunch);
                equip.CurrentSlot.AddItem(_itemBunch);
                equip.CurrentSlot = null;
            }
        }
    }

    public virtual bool IsItemProper(ItemBunch _itemBunch)
    {
        return true;
    }

    /// <summary>
    /// Функция добавления предмета в слот инвентаря
    /// </summary>
    public virtual void AddItem(ItemBunch _itemBunch)
    {
        if (_itemBunch != null)
        {
            if (_itemBunch.item != null)
            {
                itemBunch = _itemBunch;
                itemImage.sprite = itemBunch.item.image;
                itemImage.color = new Vector4(1f, 1f, 1f, 1f);
            }
            else
            {
                DeleteItem();
            }
        }
        else
        {
            DeleteItem();
        }
    }

    /// <summary>
    /// Функция удаления предмета из слота инвентаря
    /// </summary>
    public virtual void DeleteItem()
    {
        itemBunch = null;
        if (itemImage.sprite != null)
        {
            itemImage.sprite = null;
        }
        itemImage.color = new Vector4(1f, 1f, 1f, 0f);
    }

    #region events

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (equip.CurrentSlot == null)
        {
            if (itemBunch != null)
            {
                equip.CurrentSlot = this;
                equip.MouseImage.sprite = GetComponent<Image>().sprite;
                GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (equip.MouseImage.sprite != null && equip.CurrentSlot != null)
            Cursor.SetCursor(equip.MouseImage.sprite.texture, Vector2.zero, CursorMode.Auto);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {

        GetComponent<CanvasGroup>().blocksRaycasts = true;
        equip.CurrentSlot = null;
        Cursor.SetCursor(equip.defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        bool b = true;
        if (itemBunch != null && (equip.CurrentSlot!=null? equip.CurrentSlot.itemBunch != null: false))
        {
            if (itemBunch.item is WeaponClass && equip.CurrentSlot.itemBunch.item is WeaponClass)
            {
                b = true;
                WeaponClass weapon = null;
                WeaponClass weapon1 = null;
                if (this.pair != null)
                {
                    if (this.pair.itemBunch != null)
                    {
                        weapon = equip.CurrentSlot.itemBunch != null ? (WeaponClass)equip.CurrentSlot.itemBunch.item : null;
                        weapon1 = itemBunch != null ? (WeaponClass)itemBunch.item : null;
                        b = false;
                    }
                }
                else if (equip.CurrentSlot.pair != null)
                {
                    if (equip.CurrentSlot.pair.itemBunch != null)
                    {
                        weapon1 = equip.CurrentSlot.itemBunch != null ? (WeaponClass)equip.CurrentSlot.itemBunch.item : null;
                        weapon = itemBunch != null ? (WeaponClass)itemBunch.item : null;
                        b = false;
                    }
                }
                if (weapon != null && weapon1 != null)
                {
                    Debug.Log(b);
                    b = b
                        ||
                        (weapon.weaponType == "bow" || weapon.weaponType == "twoHandedSword") &&
                        (weapon1.weaponType != "bow" && weapon1.weaponType != "twoHandedSword") &&
                        equip.HaveEmptySlots(1)
                        ||
                        (weapon.weaponType != "bow" && weapon.weaponType != "twoHandedSword")
                        ||
                        weapon == null;
                }
            }
        }
        if (b)
        {
            if (equip.CurrentSlot != this && equip.CurrentSlot != null)
            {
                if ((equip.CurrentSlot.IsItemProper(itemBunch)) &&
                (IsItemProper(equip.CurrentSlot.itemBunch)))
                {
                    ItemBunch _itemBunch = equip.CurrentSlot.itemBunch;
                    equip.CurrentSlot.AddItem(itemBunch);
                    AddItem(_itemBunch);
                    equip.CurrentSlot = null;
                }
            }
        }
    }

    #endregion //events

}
