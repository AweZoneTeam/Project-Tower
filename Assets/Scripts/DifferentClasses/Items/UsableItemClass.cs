using UnityEngine;
using System.Collections;
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
    public ItemActionData itemAction;//Какое действие вызовется при начале использования предмета (подготовка к броску)
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
        ItemActionData itemData = item.itemAction;
        if (!item.charge)
        {
            item.chargeData=null;
        }

    }
}
#endif