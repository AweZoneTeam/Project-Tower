using UnityEngine;
using System.Collections;

/// <summary>
/// Набор простейших действи, которые может совершать объект, имеющий здоровье
/// </summary>
public class DmgObjActions : InterObjActions
{
    public override void Initialize()
    {
        base.Initialize();
    }

    /// <summary>
    /// Произвести взаимодействие
    /// </summary>
    public virtual void Interact()
    {
        base.Interact();
    }

    public virtual void SetStats(Prestats _stats)
    {
        base.SetStats(_stats);
    }
}
