using UnityEngine;
using System.Collections;

/// <summary>
/// Набор действий сложных персонажей, которые сильно меняют игровой процесс
/// </summary>
public class PersonActions : DmgObjActions
{

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {

    }

    #region Interface
    
    /// <summary>
    /// Повернуть персонажа 
    /// </summary>
    /// <param name="Direction"></param>
    public virtual void Turn(OrientationEnum Direction)
    {}

    /// <summary>
    /// Начать передвижение
    /// </summary>
    public virtual void StartWalking(OrientationEnum Direction)
    { }

    /// <summary>
    /// Прекратить передвижение
    /// </summary>
    public virtual void StopWalking()
    { }

    /// <summary>
    /// Совершить прыжок
    /// </summary>
    public virtual void Jump()
    {
    }

    #endregion //Interface

}
