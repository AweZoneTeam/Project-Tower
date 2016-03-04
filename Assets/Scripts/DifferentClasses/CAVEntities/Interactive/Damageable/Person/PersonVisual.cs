using UnityEngine;
using System.Collections;

/// <summary>
/// Скрипт, отвечающий за визуальную часть сложных персонажей
/// </summary>
public class PersonVisual : DmgObjVisual
{
    #region fields
    #endregion //fields

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    #region interface

    /// <summary>
    /// Анимировать отсутствие активности
    /// </summary>
    public virtual void GroundStand()
    {
    }

    /// <summary>
    /// Анимировать передвижение по земле
    /// </summary>
    public virtual void GroundMove()
    {
    }

    /// <summary>
    /// Анимировать передвижение в воздухе
    /// </summary>
    public virtual void AirMove()
    {
    }

    #endregion //interface
}
