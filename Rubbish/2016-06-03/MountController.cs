using UnityEngine;
using System.Collections;

public class MountController : AIController
{

    public float tameHP;
    bool tamed;

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
        Debug.Log(e.HP);
        if(e.HP<=tameHP)
        {
            Debug.Log("Приручён");
            tamed = true;
            enemies.Clear();
        }
    }

    public override void Interact(InterObjController interactor)
    {
        Debug.Log("интеракт");
        if (tamed)
        {
            Debug.Log("маунт приручен, всё норм");
            base.Interact(interactor);
        }
    }
}
