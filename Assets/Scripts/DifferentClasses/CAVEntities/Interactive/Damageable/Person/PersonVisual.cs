using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    protected Dictionary<string, Timer> timers = new Dictionary<string, Timer>();
    protected List<string> timerNames = new List<string>();

    protected EnvironmentStats envStats;

    #endregion //fields

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        employment = maxEmployment;
    }

    public void SetEnvStats(EnvironmentStats _envStats)
    {
        envStats = _envStats;
    }

    /// <summary>
    /// Сбросить все таймеры, кроме обозначенного
    /// </summary>
    public virtual void ResetAllTimersExcept(string _name)
    {
        for (int i = 0; i < timerNames.Count; i++)
        {
            if (!string.Equals(timerNames[i], _name))
            {
                if (timers.ContainsKey(timerNames[i]))
                {
                    timers[timerNames[i]].TimeReset();
                }
            }
        }
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
    /// Анимировать взгляд в заданную сторону
    /// </summary>
    public virtual void Look(Vector2 direction)
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
    /// Анимировать передвижение по лестнице
    /// </summary>
    public virtual void StairMove()
    {
    }

    /// <summary>
    /// Анимировать передвижение по верёвке
    /// </summary>
    public virtual void RopeMove()
    {
    }

    /// <summary>
    /// Анимировать передвижение по выступу
    /// </summary>
    public virtual void LedgeMove(float _time)
    {
    }

    /// <summary>
    /// Анимировать движение по зарослям
    /// </summary>
    public virtual void ThicketMove()
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

    /// <summary>
    /// Функция, которая используется для 2-x стадийных анимаций, первая стадия которых есть переход между анимациями
    /// </summary>
    protected virtual void AnimationTransition(float _time, string firstAnim, string secondAnim, params string[] abandonedAnims)
    {
        string timerName = firstAnim + "Timer";
        if (!timers.ContainsKey(timerName))
        {
            timerNames.Add(timerName);
            timers.Add(timerName, new Timer(_time));
        }
        ResetAllTimersExcept(timerName);
        Timer timer = timers[timerName];
        if (NotAbandonedAnim(abandonedAnims) && NotAbandonedAnim(secondAnim))
        {
            if (timer == -1f)
            {
                timer.TimeStart();
            }
            if (timer > 0f)
            {
                cAnim.Animate(firstAnim);
                timer.value -= Time.deltaTime;
            }
            else
            {
                cAnim.Animate(secondAnim);
                timer.TimeReset();
            }
        }
        else
        {
            cAnim.Animate(secondAnim);
            timer.TimeReset();
        }
    }

    /// <summary>
    /// Проверка, не является ли запрашиваемая анимация запрещённой
    /// </summary>
    protected bool NotAbandonedAnim(params string[] abandonedAnims)
    {
        bool k = true;
        foreach (string anim in abandonedAnims)
        {
            if (cAnim.CompareAnimation(anim))
            {
                k = false;
                break;
            }
        }
        return k;
    }


    #endregion //AnimatedActionsInterface

}
