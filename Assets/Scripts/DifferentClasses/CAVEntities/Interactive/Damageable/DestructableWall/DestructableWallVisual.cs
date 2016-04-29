using UnityEngine;
using System.Collections;

/// <summary>
/// Визуализатор действий, своершаемых разрушаемой стеной
/// </summary>
public class DestructableWallVisual : DmgObjVisual
{
    public GameObject fragileWall;
    public GameObject destroyedWall;

    public Vector3 destructionDirection;

    //Запустить процксс разрушения
    public override void Death()
    {
        Rigidbody shardRigid;
        Destroy(transform.GetChild(0).gameObject);
        GameObject obj = GameObject.Instantiate(destroyedWall);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(i).GetComponent<Rigidbody>().velocity=new Vector3(destructionDirection.x, Random.Range(0f,1f)*destructionDirection.magnitude, destructionDirection.z);
        }
    }
    
}
