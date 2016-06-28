using UnityEngine;
using System.Collections;


/// <summary>
/// Список действий, совершаемых поджигаемыми объектами
/// </summary>
public class FlammableActions : InterObjActions
{

    protected bool burned = false;

    /// <summary>
    /// Действие, что совершается при поджигании объекта
    /// </summary>
    public virtual void BurnAction()
    {
        burned = true;
    }

    public virtual void BurnAction(Vector3 flamePosition)
    {
        burned = true;
    }

    /// <summary>
    /// Действие, что совершается при поджигании объекта слабым огнём
    /// </summary>
    public virtual void SmallBurnAction()
    {
    }

    /// <summary>
    /// Действие, что совершается при поджигании объекта слабым огнём
    /// </summary>
    public virtual void SmallBurnAction(Vector3 flamePosition)
    {
    }

}
