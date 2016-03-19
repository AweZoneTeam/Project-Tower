using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Скрипт, отслеживающий столкновения с объектами заданного слоя.
/// </summary>
public class GroundChecker : MonoBehaviour
{
    public List<GameObject> collisions=new List<GameObject>();

    public string layer;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(layer)) 
        {
            bool k = true;
            for (int i = 0; i < collisions.Count; i++)
            {
                if (collisions[i] == other.gameObject)
                {
                    k = false;
                    break;
                }
            }
            if (k)
            {
                collisions.Add(other.gameObject);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(layer))
        {
            bool k = false;
            for (int i = 0; i < collisions.Count; i++)
            {
                if (collisions[i] == other.gameObject)
                {
                    k = true;
                    break;
                }
            }
            if (k)
            {
                collisions.Remove(other.gameObject);
            }
        }
    }
}
