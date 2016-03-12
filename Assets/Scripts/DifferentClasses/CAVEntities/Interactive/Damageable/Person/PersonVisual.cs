﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Скрипт, отвечающий за визуальную часть сложных персонажей
/// </summary>
public class PersonVisual : DmgObjVisual
{
    #region fields

    protected CharacterAnimator cAnim;//Объекты, которых можно с полной уверенностью назвать персонажами, обзавелись более сложной версией аниматора

    #endregion //fields

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        base.Initialize();
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
    /// Анимировать передвижение в воздухе
    /// </summary>
    public virtual void AirMove()
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

    #endregion //AnimatedActionsInterface

}