using UnityEngine;
using System.Collections;

/// <summary>
/// Набор действий, совершаемых разрушаемой стеной.
/// </summary>
public class DestructableWallActions : DmgObjActions
{

    #region consts

    const float deathTime = 3f;

    #endregion //consts

    public override void Death()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        dmgAnim.Death();
        Destroy(gameObject, deathTime);
    }
   

}


