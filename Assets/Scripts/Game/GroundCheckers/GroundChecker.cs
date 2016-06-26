using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Скрипт, отслеживающий столкновения с объектами заданного слоя.
/// </summary>
public class GroundChecker : MonoBehaviour
{
    [SerializeField]protected List<GameObject> collisions=new List<GameObject>();

    private List<string> whatIsGround=new List<string> {"ground","wood","grass", "metal"};
    protected List<int> layers=new List<int>();

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        layers.Clear();
        foreach (string layer in whatIsGround)
        {
            layers.Add(LayerMask.NameToLayer(layer));
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (layers.Contains(other.gameObject.layer)) 
        {
            if (!collisions.Contains(other.gameObject))
            { 
                collisions.Add(other.gameObject);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (layers.Contains(other.gameObject.layer))
        {
            if (collisions.Contains(other.gameObject))
            {
                collisions.Remove(other.gameObject);
            }
        }
    }

    public virtual int GetCount()
    {
        return collisions.Count;
    }

}
