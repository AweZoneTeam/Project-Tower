using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Список действий, совершаемых ловушкой с шипами
/// </summary>
public class SpikesTrapActions : InterObjActions
{

    #region consts

    protected const float preAttackTime = .3f;//Сколько времени должно пройти, чтобы ловушка атаковала персонажа, ставшего на неё
    protected const float attackTime=.2f;//Сколько времени происходит сама атака

    #endregion //consts

    #region parametres

    protected bool activated = false;

    #endregion parametres

    #region fields

    [SerializeField] HitClass hit=new HitClass();//Урон, что наносит ловушка
    protected HitController hitBox;
    [SerializeField]protected List<string> enemies = new List<string>();

    #endregion //fields

    public override void Initialize()
    {
        base.Initialize();
        hitBox = GetComponentInChildren<HitController>();
        hitBox.SetEnemies(enemies);
    }

    /// <summary>
    /// Что происходит, когда все сошли с ловушки
    /// </summary>
    public override void OnPushDown()
    {
        Attack();
    }

    /// <summary>
    /// Что происходит, когда кто-то стал на ловушку
    /// </summary>
    public override void OnPushUp()
    {
        if (!activated)
        {
            anim.Activate(false);
        }
        else
        {
            StartCoroutine(DeactivationProcess());
        }
    }

    /// <summary>
    /// Ловушка атакует
    /// </summary>
    void Attack()
    {
        StartCoroutine(AttackProcess());
    }

    /// <summary>
    /// Произвести атаку
    /// </summary>
    IEnumerator AttackProcess()
    {
        activated = true;
        yield return new WaitForSeconds(preAttackTime);
        anim.Activate(true);
        hitBox.SetHitBox(attackTime,hit);
        activated = false;
    }

    /// <summary>
    /// Запустить процес деактивации, если персонаж успел сойти с ловушки во время нажатия
    /// </summary>
    IEnumerator DeactivationProcess()
    {
        yield return new WaitForSeconds(preAttackTime);
        if (!activated)
        {
            anim.Activate(false);
        }
    }

}
