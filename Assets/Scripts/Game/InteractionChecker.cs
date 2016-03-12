using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, что представляет собой объекты, которые детектируют объекты, готовые к взаимодействию.
/// </summary>
public class InteractionChecker : MonoBehaviour {

    public List<InterObjController> interactions;

    public List<DropClass> dropList=new List<DropClass>();

    public void OnTriggerEnter(Collider other)
    {
        if (string.Equals(other.gameObject.tag, Tags.interactive))
        {
            InterObjController interaction = other.gameObject.GetComponent<InterObjController>();
            bool k = true;
            for (int i = 0; i < interactions.Count; i++)
            {
                if (interactions[i] == interaction)
                {
                    k = false;
                    break;
                }
            }
            if (k)
            {
                interactions.Add(interaction);
            }
        }
        else if (string.Equals(other.gameObject.tag, Tags.drop))
        {
            DropClass drop = other.gameObject.GetComponent<DropClass>();
            bool k = true;
            for (int i = 0; i < dropList.Count; i++)
            {
                if (dropList[i] == drop)
                {
                    k = false;
                    break;
                }
            }
            if (k)
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
    }

    public void OnTriggerExit(Collider other)
    {
        if (string.Equals(other.gameObject.tag, Tags.interactive))
        {
            InterObjController interaction = other.gameObject.GetComponent<InterObjController>();
            bool k = false;
            for (int i = 0; i < interactions.Count; i++)
            {
                if (interactions[i] == interaction)
                {
                    k = true;
                    break;
                }
            }
            if (k)
            {
                interactions.Remove(interaction);
            }
        }
        else if (string.Equals(other.gameObject.tag, Tags.drop))
        {
            DropClass drop = other.gameObject.GetComponent<DropClass>();
            bool k = false;
            for (int i = 0; i < dropList.Count; i++)
            {
                if (dropList[i] == drop)
                {
                    k = true;
                    break;
                }
            }
            if (k)
            {
                dropList.Remove(drop);
            }
        }
    }

}
