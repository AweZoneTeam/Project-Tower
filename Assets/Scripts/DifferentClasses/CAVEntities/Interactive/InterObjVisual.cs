using UnityEngine;
using System.Collections;

public class InterObjVisual : MonoBehaviour
{

    #region fields
    private Prestats stats;
    protected InterObjAnimator cAnim;
    #endregion fields

    public virtual void Initialize()
    {
        cAnim = GetComponent<InterObjAnimator>();
    }

    public virtual void Awake()
    {
        Initialize();
    }

    public virtual void SetStats(Prestats _stats)
    {
        stats = _stats; 
    }
}
