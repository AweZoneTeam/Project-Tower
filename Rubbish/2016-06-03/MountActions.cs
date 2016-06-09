using UnityEngine;
using System.Collections;

public class MountActions : AIActions
{

    #region MountStats
    public float tameSpeed;
    public float height;
    public SimpleWeapon mountWeapon;
    public GameObject part;
    #endregion//MountStats
    

    public override void Interact()
    {
        if (interactor is KeyboardActorController)
        {
            Debug.Log("Кейборд - интерактор");
            ((KeyboardActorController)interactor).UseMount(this);
        }       
    }
}
