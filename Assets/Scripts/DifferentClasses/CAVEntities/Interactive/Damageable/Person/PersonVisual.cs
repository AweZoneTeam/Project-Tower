using UnityEngine;
using System.Collections;

/// <summary>
/// Скрипт, отвечающий за визуальную часть сложных персонажей
/// </summary>
public class PersonVisual : DmgObjVisual
{

    #region consts

    protected const int maxEmployment = 10;

    #endregion //consts

    #region parametres

    protected int employment = maxEmployment;

    #endregion //parametres

    #region fields

    protected CharacterAnimator cAnim;//Объекты, которых можно с полной уверенностью назвать персонажами, обзавелись более сложной версией аниматора

    #endregion //fields

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        employment = maxEmployment;
    }

    #region AnimatedActionsInterface

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
    /// Анимировать быстрое передвижение по земле
    /// </summary>
    public virtual void FastGroundMove()
    {
    }

    /// <summary>
    /// Анимировать кувырок
    /// </summary>
    public virtual void Flip()
    { }

    /// <summary>
    /// Анимировать прижатие к стене
    /// </summary>
    public virtual void WallInteraction(float _time)
    {
    }

    /// <summary>
    /// Анимировать висячее состояние на краю
    /// </summary>
    public virtual void Hanging(float _time)
    {
    }

    /// <summary>
    /// Анимировать передвижение в воздухе
    /// </summary>
    public virtual void AirMove()
    {
    }

    /// <summary>
    /// Анимировать движение в приседе
    /// </summary>
    public virtual void CrouchMove()
    {

    }
       
    /// <summary>
    /// Анимировать процесс атаки
    /// </summary>
    public virtual void Attack(string attackName, float time)
    {
    }

    /// <summary>
    /// Вызвать процесс смерти
    /// </summary>
    public override void Death()
    {
         StartCoroutine(DeathProcess());
    }

    /// <summary>
    /// Процесс смерти
    /// </summary>
    protected override IEnumerator DeathProcess()
    {
        if (cAnim != null)
        {
            death = true;
            cAnim.Animate("Death");
            yield return new WaitForSeconds(deathTime);
            cAnim.Animate("Dead");
        }
    }

    protected IEnumerator VisualRoutine(int _employment, float _time)
    {
        employment -= _employment;
        yield return new WaitForSeconds(_time);
        employment += _employment;
    }

    #endregion //AnimatedActionsInterface

}
