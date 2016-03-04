using UnityEngine;
using System.Collections;

/// <summary>
/// Набор действий сложных персонажей, которые сильно меняют игровой процесс
/// </summary>
public class PersonActions : DmgObjActions
{

    #region parametres
    protected orientationEnum movingDirection;
    protected orientationEnum orientation;
    protected bool moving;
    #endregion //parametres

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
    /// <param name="direction"></param>
    public virtual void Turn(orientationEnum direction)
    {
        if (orientation != direction)
        {
            Vector3 newScale = this.gameObject.transform.localScale;
            newScale.x *= -1;
            orientation = direction;
            this.gameObject.transform.localScale = newScale;
        }
    }

    /// <summary>
    /// Начать передвижение
    /// </summary>
    public virtual void StartWalking(orientationEnum direction)
    {
        if (!moving)
        {
            moving = true;
            movingDirection = direction;
        }
    }

    /// <summary>
    /// Прекратить передвижение
    /// </summary>
    public virtual void StopWalking()
    {
        moving = false;
    }


    /// <summary>
    /// Совершить прыжок
    /// </summary>
    public virtual void Jump()
    {
    }

    #endregion //Interface

    /// <summary>
    /// Задать поле статов
    /// </summary>
    public virtual void SetStats(Stats _stats)
    {
    }

}
