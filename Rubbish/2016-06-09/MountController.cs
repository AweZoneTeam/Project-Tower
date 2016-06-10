using UnityEngine;
using System.Collections;

public class MountController : AIController
{

    #region parametrs
    public float tameHP;
    bool tamed;
    #endregion//parametrs

    #region fields
    public KeyboardActorController host;
    #endregion//fields

    protected override void FormActionDictionaries()
    {
        base.FormActionDictionaries();
    }

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        base.Initialize();
        orgStats.HealthChangedEvent += Tame;
    }

    void Tame(object sender, OrganismEventArgs e)
    {
        if(e.HP<=tameHP)
        {
            tamed = true;
            enemies.Clear();
        }
    }

    public override void Interact(InterObjController interactor)
    {
        if (tamed)
        {
            if(interactor is KeyboardActorController)
            {
                host =(KeyboardActorController)interactor;
            }
            base.Interact(interactor);
        }
    }
}
