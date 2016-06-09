using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, что представляет собой коммуникатор персонажей, способных взаимодействовать с другими персонажами и побирать дроп.
/// </summary>
public class InteractionChecker : PreInteractionChecker {

    [SerializeField]
    protected List<InterObjController> interactions=new List<InterObjController>();

    protected List<DropClass> dropList=new List<DropClass>();

    protected List<string> interactiveTags= new List<string> {"Interactive", "NPC", "Enemy"};

    public override void OnTriggerEnter(Collider other)
    {
        #region GeneralInteractions

        if (interactiveTags.Contains(other.gameObject.tag))
        {
            InterObjController interaction = other.gameObject.GetComponent<InterObjController>();
            if (!interactions.Contains(interaction))
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("ledge"))
                {
                    interactions.Insert(0, interaction);
                }
                else
                {
                    interactions.Add(interaction);
                }
            }
        }

        #endregion //GeneralInteractions

        #region DropInteractions

        else if (string.Equals(other.gameObject.tag, Tags.drop))
        {
            DropClass drop = other.gameObject.GetComponent<DropClass>();
            if (!dropList.Contains(drop))
            {
                if (drop.autoPick)
                {
                    dropList.Insert(0, drop);
                }
                else
                {
                    dropList.Add(drop);
                }
            }
        }

        #endregion //DropInteractions
        
        base.OnTriggerEnter(other);

    }

    public override void OnTriggerExit(Collider other)
    {

        #region GeneralInteractions

        if (interactiveTags.Contains(other.gameObject.tag))
        {
            InterObjController interaction = other.gameObject.GetComponent<InterObjController>();
            if (interactions.Contains(interaction))
            {
                interactions.Remove(interaction);
            }
        }

        #endregion //GeneralInteractions

        #region DropInteractions

        else if (string.Equals(other.gameObject.tag, Tags.drop))
        {
            DropClass drop = other.gameObject.GetComponent<DropClass>();
            if (dropList.Contains(drop))
            {
                dropList.Remove(drop);
            }
        }

        #endregion //DropInteractions

        base.OnTriggerExit(other);

    }

    public override List<InterObjController> GetInteractionList()
    {
        return interactions;
    }

    public override List<DropClass> GetDropList()
    {
        return dropList;
    }

}
