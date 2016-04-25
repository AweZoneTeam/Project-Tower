using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, представляющий используемые одноразовые предметы
/// </summary>
[System.Serializable]
public class UsableItemClass : ItemClass
{
    public string useItemType;
    public GameObject itemObject; //Игровой объект, что ассоциируется с этим предметом.
    public ActionSign itemAction;//Какое действие вызовется при использовании предмета
    public bool grounded;//если true, то предмет исользуется только на земле
    public float itemTime;// как долго используется данный предмет
    public int employment;//Очки занятости, которые приобретёт персонаж при использовании предмета
}
