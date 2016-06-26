using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, представляющий собой игровой предмет, который можно засунуть в инвентарь.
/// </summary>
[System.Serializable]
public class ItemClass: ScriptableObject
{
	public string itemName;
    public string type;
    public Sprite image;//Иконка предмета  
    [TextArea(3,10)]
    public string description;
    [TextArea(3, 10)]
    public string parametres;
    public int maxCount;//Сколько максимум предметов этого типа может находится в одном экземпляре класса itemBunch



    public List<ItemVisual> itemVisuals = new List<ItemVisual>();//части тела, которые добавляются персонажу при использовании предмета.

}

/// <summary>
/// Специальный класс, воспринимаемый инвентарём, чтобы знать, сколько предметов каждого типа есть у интерактивного объекта
/// </summary>
[System.Serializable]
public class ItemBunch 
{
    public ItemClass item;
    public int quantity;

    public ItemBunch(ItemClass _item)
    {
        item = _item;
        quantity = 1;
    }

    public ItemBunch(ItemClass _item, int _quantity)
    {
        item = _item;
        quantity = _quantity;
    }

    public void ConsumeItem()
    {
        quantity--;
        if (quantity <= 0)
        {
            item = null;
        }
    }

}

/// <summary>
/// Специальный класс, нужный для смены предметов
/// </summary>
[System.Serializable]
public class ItemVisual
{
    public GameObject part;
    public Vector3 pos;

    public ItemVisual(Vector3 _pos)
    {
        part = null;
        pos = _pos;
    }
}