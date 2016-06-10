using UnityEngine;
using System.Collections;

public class MountActions : AIActions
{

    #region MountStats
    public float tameSpeed;
    public float height;
    public float tameJumpForce;
    public SimpleWeapon mountWeapon;
    public GameObject part;
    #endregion//MountStats

    #region fields
    private MountController MountInteractor;
    #endregion//fields

    public override void Initialize()
    {
        base.Initialize();
        MountInteractor = (MountController)interactor;
    }

    public override void Interact()
    {
        if (interactor is KeyboardActorController)
        {
            ((KeyboardActorController)interactor).GetEnvStats().interaction = interactionEnum.mount;
            ((KeyboardActorController)interactor).UseMount(this);
        }       
    }

    protected void ReturnToCheckpoint()
    {
        
    }

    public void Appear(object sender, MountEventArgs m)
    {
        gameObject.active = true;
        transform.position = ((MonoBehaviour)sender).transform.position;
    }
}
