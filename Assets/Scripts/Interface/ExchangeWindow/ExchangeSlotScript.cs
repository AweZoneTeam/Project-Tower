using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExchangeSlotScript : MonoBehaviour
{

    private InterfaceExchangeWindow exchWindow;
    private Image image, parentImage;

    private ItemBunch itemBunch;
    public ItemBunch Item
    {
        get { return itemBunch; }
    }

    private bool chosen = false;

    public void ChooseItem()
    {
        if (itemBunch != null)
        {
            if (!chosen)
            {
                exchWindow.SlotsToTake.Add(this);
                exchWindow.LastSlot = this;
                SetChosen(true);
            }
            else
            {
                exchWindow.TakeItem(this);
                if (exchWindow.SlotsToTake.Contains(this))
                {
                    exchWindow.SlotsToTake.Remove(this);
                }
                SetChosen(false);
            }
        }
    }

    public void UseIt()
    {
        itemBunch=null;
        image.color = new Vector4(1f, 1f, 1f, 0f);
        image.sprite = null;

    }

    public void SetChosen(bool _chosen)
    {
        chosen = _chosen;
        parentImage.color=new Vector4(1f,1f,chosen?0.5f:1f,0.25f);
    }

    public void Initialize(InterfaceExchangeWindow _exchWindow)
    {
        exchWindow = _exchWindow;
        image = GetComponent<Image>();
        parentImage = transform.parent.GetComponent<Image>();
    }

    public void InitializeSlot(ItemBunch _itemBunch)
    {
        parentImage.color = new Vector4(1f, 1f, 1f, 0.25f);
        if (_itemBunch!=null)
        {
            itemBunch = _itemBunch;
            if (itemBunch.item != null)
            {
                image.sprite = itemBunch.item.image;
                image.color = new Vector4(1f, 1f, 1f, 1f);

            }
        }
        else
        {
            itemBunch = null;
            image.color = new Vector4(1f, 1f, 1f, 0f);
        }
    }

}
