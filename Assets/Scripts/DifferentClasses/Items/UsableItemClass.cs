using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Класс, представляющий используемые одноразовые предметы
/// </summary>
[System.Serializable]
public class UsableItemClass : ItemClass
{
    public string useItemType;
    public List<ItemActionData> itemActions=new List<ItemActionData>();//Список действий, которые можно произвести предметом (обычно ограничивается одним элементом)
    public bool grounded;//если true, то предмет исользуется только на земле
    public bool charge;//Можно ли этот предмет зарядить?
    public ItemChargeData chargeData;//Как происходит "заряд предмета"
}

#if UNITY_EDITOR
/// <summary>
/// Редактор используемых предметов
/// </summary>
[CustomEditor(typeof(UsableItemClass))]
public class UsableItemEditor : Editor
{

    private Direction direction;
    private OrganismStats orgStats;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UsableItemClass item = (UsableItemClass)target;
        if (!item.charge)
        {
            item.chargeData=null;
        }

    }
}
#endif