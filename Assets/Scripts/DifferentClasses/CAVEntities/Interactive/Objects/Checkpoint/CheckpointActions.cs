using UnityEngine;
using System.Collections;

public class CheckpointActions : InterObjActions
{

    #region parameters

    public AreaClass currentRoom;

    protected bool activated=false;
    public bool Activated {set { activated = value; }}

    #endregion

    /// <summary>
    /// Произвести взаимодействие
    /// </summary>
    public override void Interact()
    {
        if (interactor != null)
        {
            if (interactor is KeyboardActorController)
            {
                KeyboardActorController actor = (KeyboardActorController)interactor;
                OrganismStats orgStats = actor.GetOrgStats();

                //Исцеляющие свойства чекпоинта
                orgStats.health = orgStats.maxHealth;
                for (int i = 0; i < actor.buffList.Count; i++)
                {
                    if (actor.buffList[i].removable)
                    {
                        actor.buffList.RemoveBuff(actor.buffList[i]);
                        i--;
                    }
                }

                activated = true;
                if (actor.GetEnvStats().interaction == interactionEnum.noInter)
                {
                    SpFunctions.OpenSaveWindow(true, this);
                }
            }
        }
    }
}
