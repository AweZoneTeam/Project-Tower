using UnityEngine;
using System.Collections;

public class InterObjVisual : MonoBehaviour
{

    #region fields
    protected Direction direction;
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

    public virtual void SetDirection(Direction _direction)
    {
        direction = _direction; 
    }
}
