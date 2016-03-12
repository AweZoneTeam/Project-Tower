using UnityEngine;
using System.Collections;

public class InterObjVisual : MonoBehaviour
{

    #region fields
    private Prestats stats;
    protected InterObjAnimator iAnim;//Все простые объекты имеют простейшую версию аниматора
    #endregion fields

    public virtual void Initialize()
    {
        iAnim = GetComponent<InterObjAnimator>();
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
