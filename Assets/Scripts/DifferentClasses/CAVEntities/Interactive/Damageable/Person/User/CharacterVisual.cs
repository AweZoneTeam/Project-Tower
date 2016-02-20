﻿using UnityEngine;
using System.Collections;

public class CharacterVisual : PersonVisual
{

    #region consts

    private const float beginRunTime = 0.2f;//Сколько времени длится анимация "Начало бега"?
    private const float stopDragTime = 0.5f;//Сколько времени длится анимация "Конец скольжения"?
    private const float dragSpeed = 5f;//до какой скорости считается, что персонаж ещё тормозится (когда его скорость уменьшается)?

    #endregion

    #region enums

    // public enum groundness { grounded = 1, crouch, preGround, inAir };

    private enum speedY { fastUp = 30, medUp = 10, slowUp = 7, slowDown = -1, medDown = -6, fastDown = -10 };//Скорости, при которых меняется анимация прыжка

    #endregion //enums

    #region timers

    public float beginRunTimer, stopDragTimer;

    #endregion //timers

    #region fields
    private CharacterAnimator cAnim;
    private Stats stats;
    private Rigidbody rigid;
    #endregion //fields


    public override void Initialize()
    {
        cAnim = GetComponent<CharacterAnimator>();
        rigid = gameObject.GetComponentInParent<Rigidbody>();
    }

    public override void Awake()
    {
        Initialize();
    }

    #region AnimatedActions
    /// <summary>
    /// Анимировать отсутствие активности
    /// </summary>
    public void GroundStand()
    {
        if (cAnim != null)
        {
            beginRunTimer = -1f;
            if (Mathf.Abs(rigid.velocity.x) > dragSpeed)
            {
                cAnim.Animate("DragBegin");
            }
            else
            {
                if (!cAnim.CompareAnimation("Idle"))
                {
                    if (stopDragTimer == -1f)
                    {
                        stopDragTimer = stopDragTime;
                    }
                    if (stopDragTimer > 0f)
                    {
                        cAnim.Animate("DragStop");
                        stopDragTimer -= Time.deltaTime;
                    }
                    else
                    {
                        cAnim.Animate("Idle");
                        stopDragTimer = -1f;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Анимировать передвижение по земле
    /// </summary>
    public void GroundMove()
    {
        if (cAnim != null)
        {
            stopDragTimer = -1f;
            if (!cAnim.CompareAnimation("Run"))
            {
                if (beginRunTimer == -1f)
                {
                    beginRunTimer = beginRunTime;
                }
                if (beginRunTimer > 0f)
                {
                    cAnim.Animate("RunBegin");
                    beginRunTimer -= Time.deltaTime;
                }
                else
                {
                    cAnim.Animate("Run");
                    beginRunTimer = -1f;
                }
            }
        }
    }

    /// <summary>
    /// Анимировать движения, происходящие в воздухе
    /// </summary>
    public void AirMove()
    {
        if (cAnim != null)
        {
            if (stats.groundness == (int)groundness.preGround) { cAnim.Animate("Fallen"); }
            else if (rigid.velocity.y <= 1f * (int)speedY.fastDown) { cAnim.Animate("FallEnd"); }
            else if (rigid.velocity.y <= 1f * (int)speedY.medDown) { cAnim.Animate("FallContinue"); }
            else if (rigid.velocity.y <= 1f * (int)speedY.slowDown) { cAnim.Animate("FallBegin"); }
            else if (rigid.velocity.y <= 1f * (int)speedY.slowUp) { cAnim.Animate("JumpEnd"); }
            else if (rigid.velocity.y <= 1f * (int)speedY.medUp) { cAnim.Animate("JumpContinue"); }
            else if (rigid.velocity.y <= 1f * (int)speedY.fastUp) { cAnim.Animate("JumpBegin"); }
            else { cAnim.Animate("StartJump"); }
            beginRunTimer = -1f;
        }
    }
    #endregion //AnimatedActions

    public void SetStats(Stats _stats)
    {
        stats = _stats;
    }

}
