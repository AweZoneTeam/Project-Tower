using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionChecker : MonoBehaviour {

    public List<InterObjController> interactions;



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
    }

}
