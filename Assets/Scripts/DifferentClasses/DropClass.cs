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
        col.isTrigger = false;
        groundCheck = transform.FindChild("GroundCheck");
    }

    public void FixedUpdate()
    {
        Collider[] cols1= Physics.OverlapSphere(transform.position, groundRadius, whatIsGround);
        if ((Physics.OverlapSphere(groundCheck.position, groundRadius, whatIsGround).Length>0)&&(!col.isTrigger))
        {
            col.isTrigger = true;
            rigid.isKinematic = true;
        }     
    }

}
