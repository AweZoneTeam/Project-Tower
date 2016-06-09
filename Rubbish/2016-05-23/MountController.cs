using UnityEngine;
using System.Collections;

public class MountController : AIController
{

    float tameHP;
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
        if(e.HP<=tameHP)
        {
            tamed = true;
            enemies.Clear();
        }
    }
}
