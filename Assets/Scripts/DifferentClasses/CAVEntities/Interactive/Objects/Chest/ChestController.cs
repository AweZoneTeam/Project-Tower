using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChestController : InterObjController
{

    #region fields

    [SerializeField]private BagClass chestContent;
   
    #endregion //fields

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void SetAction()
    {
        base.SetAction();
        intActions.SetBag(chestContent);
    }

    public override void Interact(InterObjController interactor)
    {
        if (intActions != null)
        {
            intActions.SetInteractor(interactor);
            intActions.Interact();
        }
    }

    /// <summary>
    /// Проинициализировать обхект в соответствии с сохранёнными данными
    /// </summary>
    public override void AfterInitialize(InterObjData intInfo, Map map, Dictionary<string, InterObjController> savedIntObjs)
    {
        base.AfterInitialize(intInfo, map, savedIntObjs);
        EquipmentDatabase eBase = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponentInChildren<EquipmentDatabase>();
        ChestData chInfo = (ChestData)intInfo;
        chestContent.bag = new List<ItemBunch>();
        if (eBase != null)
        {
            foreach (BagSlotInfo itemInfo in chInfo.chestContent)
            {
                if (eBase.ItemDict.ContainsKey(itemInfo.item))
                {
                    chestContent.bag.Add(new ItemBunch(eBase.ItemDict[itemInfo.item], itemInfo.quantity));
                }
            }
            if (intActions != null)
            {
                intActions.SetBag(chestContent);
                ChestActions chActions;
                if ((chActions = (ChestActions)intActions) != null)
                {
                    chActions.chestLock.opened = chInfo.opened;
                }
            }
        }
    }

    /// <summary>
    /// Получить информацию об объекте
    /// </summary>
    public override InterObjData GetInfo()
    {
        ChestData intInfo = new ChestData();
        intInfo.objId = objId;
        if (spawnId != null ? !string.Equals(spawnId, string.Empty) : false)
        {
            intInfo.spawnId = spawnId;
        }
        else
        {
            intInfo.spawnId = string.Empty;
        }
        intInfo.position = transform.position;
        intInfo.roomPosition = currentRoom.id.areaName;
        intInfo.orientation = (int)direction.dir;
        ChestActions chActions;
        intInfo.opened=(chActions=GetComponent<ChestActions>())!=null?chActions.chestLock.opened:false;
        intInfo.chestContent = new List<BagSlotInfo>();
        foreach (ItemBunch itemBunch in chestContent.bag)
        {
            intInfo.chestContent.Add(new BagSlotInfo(-1, itemBunch));
        } 
        return intInfo;
    }

    public BagClass GetChest()
    {
        return chestContent;
    }

}

#if UNITY_EDITOR
/// <summary>
/// Редактор сундуков и их содержимого
/// </summary>
[CustomEditor(typeof(ChestController))]
public class ChestEditor : InterObjEditor
{
    private Direction direction;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ChestController obj = (ChestController)target;
        direction = obj.GetDirection();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.EnumMaskField("direction", direction.dir);
    }
}
#endif