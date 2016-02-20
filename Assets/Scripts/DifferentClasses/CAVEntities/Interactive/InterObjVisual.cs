using UnityEngine;
using System.Collections;

public class InterObjVisual : MonoBehaviour
{

    #region fields
    [SerializeField]private Prestats stats;
    #endregion fields

    public virtual void Initialize()
    {

    }

    public virtual void Awake()
    {
        Initialize();
    }

    public void SetStats(Prestats _stats)
    {
        stats = _stats; 
    }
}
