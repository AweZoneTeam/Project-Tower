using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Этот скрипт навешивается на предметы, которые являются дропающимися, то есть выпадающими. 
/// </summary>
public class DropClass : MonoBehaviour {

    const float groundRadius = 1f;

    public List<ItemBunch> drop=new List<ItemBunch>();
    public bool autoPick=true;


    private Rigidbody rigid;
    private Collider col;
    private Transform groundCheck;

    public LayerMask whatIsGround;

    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        if (rigid != null)
        {
            col.isTrigger = false;
        }
        groundCheck = transform.FindChild("GroundCheck");
    }

    public void FixedUpdate()
    {
        if (rigid != null)
        {
            Collider[] cols1 = Physics.OverlapSphere(transform.position, groundRadius, whatIsGround);
            if ((Physics.OverlapSphere(groundCheck.position, groundRadius, whatIsGround).Length > 0) && (!col.isTrigger))
            {
                col.isTrigger = true;
                rigid.isKinematic = true;
            }
        }  
    }

    /// <summary>
    /// Установить параметры дропа, учитывая сохраненную о нём информацию
    /// </summary>
    public void SetDrop(DropInfo info, EquipmentDatabase eBase)
    {
        transform.position = info.position;
        autoPick = info.autoPick;

        drop = new List<ItemBunch>();

        foreach (BagSlotInfo slotInfo in info.itemBunches)
        {
            if (eBase.ItemDict.ContainsKey(slotInfo.item))
            {
                drop.Add(new ItemBunch(eBase.ItemDict[slotInfo.item], slotInfo.quantity));
            }
        }

        transform.localScale = info.scale;
        GetComponent<BoxCollider>().size = info.boxSize;
        transform.FindChild("GroundCheck").position = info.groundCheckPosition;

    }

}
