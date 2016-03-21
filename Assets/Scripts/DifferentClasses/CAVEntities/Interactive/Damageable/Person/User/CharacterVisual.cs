using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterVisual : PersonVisual
{

    #region consts

    private const float beginRunTime = 0.2f;//Сколько времени длится анимация "Начало бега"?
    private const float stopDragTime = 0.5f;//Сколько времени длится анимация "Конец скольжения"?
    private const float crouchTime = 0.5f;//Сколько времени длится анимация "присесть"?
    private const float flipTime = 0.8f;//Сколько времени персонаж кувыркается?
    private const float hangTime = 0.4f;//Сколько времени длится анимация "Захватиться за уступ?"

    private const float dragSpeed = 5f;//до какой скорости считается, что персонаж ещё тормозится (когда его скорость уменьшается)?
    private const float crouchSpeed = 5f;

    #endregion

    #region enums

    // public enum groundness { grounded = 1, crouch, preGround, inAir };

    private enum speedY { fastUp = 30, medUp = 10, slowUp = 7, slowDown = -1, medDown = -6, fastDown = -10 };//Скорости, при которых меняется анимация прыжка

    #endregion //enums

    #region timers

    public float beginRunTimer, stopDragTimer, crouchTimer;

    #endregion //timers

    #region parametres

    private bool attack=false;

    #endregion //parametres

    #region fields

    private Stats stats;
    private Rigidbody rigid;

    public int k1 = 0;

    private Dictionary<string,Timer> timers = new Dictionary<string, Timer>();
    private List<string> timerNames = new List<string>();

    #endregion //fields

    public override void Awake()
    {
        base.Awake();
    }

        public override void Initialize()
    {
        base.Initialize();
        cAnim = GetComponent<CharacterAnimator>();
        rigid = gameObject.GetComponentInParent<Rigidbody>();
        FormTimers();
    }

    /// <summary>
    /// Сформировать список таймеров
    /// </summary>
    public virtual void FormTimers()
    {
        timerNames.Add("beginRunTimer");
        timerNames.Add("stopDragTimer");
        timerNames.Add("crouchTimer");
        timerNames.Add("hangTimer");

        timers.Add("beginRunTimer",new Timer(beginRunTime));
        timers.Add("stopDragTimer", new Timer(stopDragTime));
        timers.Add("crouchTimer", new Timer(crouchTime));
        timers.Add("hangTimer", new Timer(hangTime));
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

    #region AnimatedActions
    /// <summary>
    /// Анимировать отсутствие активности
    /// </summary>
    public override void GroundStand()
    {
        if ((cAnim != null)&&(!attack)&&!(employment<=4))
        {
            Timer timer = timers["stopDragTimer"];
            ResetAllTimersExcept("stopDragTimer");
            if (Mathf.Abs(rigid.velocity.x) > dragSpeed)
            {
                cAnim.Animate("DragBegin");
            }
            else
            {
                if ((cAnim.CompareAnimation("DragBegin")) || (cAnim.CompareAnimation("DragStop")))
                {
                    if (timer == -1f)
                    {
                        timer.TimeStart();
                    }
                    if (stopDragTimer > 0f)
                    {
                        cAnim.Animate("DragStop");
                        timer.value -= Time.deltaTime;
                        stopDragTimer = timer.value;
                    }
                    else
                    {
                        cAnim.Animate("Idle");
                        timer.TimeReset();
                    }
                }
                else
                {
                    cAnim.Animate("Idle");
                    timer.TimeReset();
                }
            }
        }
    }

    /// <summary>
    /// Анимировать кувырок
    /// </summary>
    public override void Flip()
    {
        if ((cAnim != null) && (!attack) && !(employment <= 4))
        {
            cAnim.Animate("FlipForward");
            StartCoroutine(VisualRoutine(10,flipTime));
        }
    }

    /// <summary>
    /// Анимировать передвижение по земле
    /// </summary>
    public override void GroundMove()
    {
        if ((cAnim != null)&&(!attack)&&!(employment<=4))
        {
            Timer timer = timers["beginRunTimer"];
            ResetAllTimersExcept("beginRunTimer");
            if (!cAnim.CompareAnimation("Run")&& !cAnim.CompareAnimation("FastRun"))
            {
                if (timer == -1f)
                {
                    timer.TimeStart();
                }
                if (timer > 0f)
                {
                    beginRunTimer = timer.value;
                    cAnim.Animate("RunBegin");
                    timer.value-= Time.deltaTime;
                }
                else
                {
                    cAnim.Animate("Run");
                    timer.TimeReset();
                }
            }
        }
    }

    /// <summary>
    /// Анимировать движение при быстром беге
    /// </summary>
    public override void FastGroundMove()
    {
        if ((cAnim != null) && (!attack) && !(employment <= 4))
        {
            Timer timer = timers["beginRunTimer"];
            ResetAllTimersExcept("beginRunTimer");
            if (!cAnim.CompareAnimation("Run") && !cAnim.CompareAnimation("FastRun"))
            {
                if (timer == -1f)
                {
                    timer.TimeStart();
                }
                if (timer > 0f)
                {
                    cAnim.Animate("FastRunBegin");
                    timer.value -= Time.deltaTime;
                }
                else
                {
                    cAnim.Animate("FastRun");
                    timer.TimeReset();
                }
            }
        }
    }

    /// <summary>
    /// Анимировать движение в приседе
    /// </summary>
    public override void CrouchMove()
    {
        if ((cAnim != null) && (!attack) && !(employment <= 4))
        {
            Timer timer = timers["crouchTimer"];
            ResetAllTimersExcept("crouchTimer");
            if (cAnim != null)
            {
                if (Mathf.Abs(rigid.velocity.x) > crouchSpeed)
                {
                    timer.TimeReset();
                    cAnim.Animate("CrouchMove");
                }
                else if (!cAnim.CompareAnimation("CrouchMove") && !cAnim.CompareAnimation("CrouchContinue") && !cAnim.CompareAnimation("FlipForward"))
                {
                    k1++;
                    crouchTimer = timer.maxValue;
                    if (timer == -1f)
                    {
                        timer.TimeStart();
                    }
                    if (crouchTimer > 0f)
                    {
                        cAnim.Animate("Crouch");
                        timer.value -= Time.deltaTime;
                    }
                    else
                    {
                        cAnim.Animate("CrouchContinue");
                        timer.TimeReset();
                    }
                }
                else
                {
                    cAnim.Animate("CrouchContinue");

                }
                
            }
        }
    }

    /// <summary>
    /// Анимировать движения, происходящие в воздухе
    /// </summary>
    public override void AirMove()
    {
        if ((cAnim != null)&&(!attack)&&(employment>4))
        {
            if (stats.groundness == groundnessEnum.preGround) { cAnim.Animate("Fallen"); }
            else if (rigid.velocity.y <= 1f * (int)speedY.fastDown) { cAnim.Animate("FallEnd"); }
            else if (rigid.velocity.y <= 1f * (int)speedY.medDown) { cAnim.Animate("FallContinue"); }
            else if (rigid.velocity.y <= 1f * (int)speedY.slowDown) { cAnim.Animate("FallBegin"); }
            else if (rigid.velocity.y <= 1f * (int)speedY.slowUp) { cAnim.Animate("JumpEnd"); }
            else if (rigid.velocity.y <= 1f * (int)speedY.medUp) { cAnim.Animate("JumpContinue"); }
            else if (rigid.velocity.y <= 1f * (int)speedY.fastUp) { cAnim.Animate("JumpBegin"); }
            else { cAnim.Animate("StartJump"); }
            ResetAllTimersExcept("");
        }
    }

    /// <summary>
    /// Анимировать взаимодейтсвие с препятствиями
    /// </summary>
    public override void WallInteraction(float _time)
    {
        if ((cAnim != null) && (!attack)&& (employment > 3))
        {
            if (_time > 0f)
            {
                cAnim.Animate("SmallClimb");
                StartCoroutine(VisualRoutine(10, _time));
            }
            else
            {
                if (stats.groundness == groundnessEnum.grounded)
                {
                    cAnim.Animate("WallInFront");
                }
                else if (stats.groundness == groundnessEnum.crouch)
                {
                    cAnim.Animate("CrouchWall");
                }
                else
                {
                    cAnim.Animate("JumpWall");
                }
                StartCoroutine(VisualRoutine(6, Time.deltaTime));
            }
        }
    }

    /// <summary>
    /// Анимировать состояние захвата за уступ
    /// </summary>
    public override void Hanging(float _time)
    {
        if ((cAnim != null) && (!attack) && (employment > 3))
        {
            if (_time > 0f)
            {
                cAnim.Animate("Climb");
                StartCoroutine(VisualRoutine(10, _time));
            }
            else
            {
                Timer timer = timers["hangTimer"];
                ResetAllTimersExcept("hangTimer");
                if (!cAnim.CompareAnimation("Hanging"))
                {
                    if (timer == -1f)
                    {
                        timer.TimeStart();
                    }
                    if (timer > 0f)
                    {
                        cAnim.Animate("HangUp");
                        timer.value -= Time.deltaTime;
                    }
                    else
                    {
                        cAnim.Animate("Hanging");
                        timer.TimeReset();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Анимировать движение по зарослям
    /// </summary>
    public override void ThicketMove()
    {
        if (cAnim != null)
        {
            if (!cAnim.CompareAnimation("ThicketMove"))
            {
                cAnim.Animate("Thicket");
            }

            if (rigid.velocity.magnitude > 5f)
            {
                cAnim.Animate("ThicketMove");
            }
            else if (cAnim.CompareAnimation("ThicketMove"))
            {
                cAnim.Pause();
            }
        }
    }

    /// <summary>
    /// Анимировать движение на лестнице
    /// </summary>
    public override void StairMove()
    {
        if (cAnim != null)
        {
            if (!cAnim.CompareAnimation("StairUp")&& !cAnim.CompareAnimation("StairDown"))
            {
                cAnim.Animate("Stair");
            }

            if (rigid.velocity.y > 5f)
            {
                cAnim.Animate("StairUp");
            }
            else if (rigid.velocity.y < -5f)
            {
                cAnim.Animate("StairDown");
            }
            else if (cAnim.CompareAnimation("StairUp") || cAnim.CompareAnimation("StairDown"))
            {
                cAnim.Pause();
            }
        }
    }

    /// <summary>
    /// Анимировать движение на верёвке
    /// </summary>
    public override void RopeMove()
    {
        if (cAnim != null)
        {
            if (!cAnim.CompareAnimation("RopeUp") && !cAnim.CompareAnimation("RopeDown"))
            {
                cAnim.Animate("Rope");
            }

            if (rigid.velocity.y > 5f)
            {
                cAnim.Animate("RopeUp");
            }
            else if (rigid.velocity.y < -5f)
            {
                cAnim.Animate("RopeDown");
            }
            else if (cAnim.CompareAnimation("RopeUp") || cAnim.CompareAnimation("RopeDown"))
            {
                cAnim.Pause();
            }
        }
    }

    /// <summary>
    /// Анимировать движение на выступе
    /// </summary>
    public override void LedgeMove(float _time)
    {
        if (_time <= 0f)
        {
            if (cAnim != null)
            {
                if (rigid.velocity.x > 5f)
                {
                    cAnim.Animate("EdgeMoveRight");
                }
                else if (rigid.velocity.x <-5f)
                {
                    cAnim.Animate("EdgeMoveLeft");
                }
                else
                {
                    cAnim.Animate("Edge");
                }
            }
        }
        else
        {
            if (cAnim != null)
            {
                cAnim.Animate("EdgeUp");
            }
        }
    }

    /// <summary>
    /// Анимировать заданную атаку
    /// </summary>
    public override void Attack(string attackName, float time)
    {
        if (cAnim != null)
        {
            cAnim.Animate(attackName);
            StartCoroutine(AttackProcess(time));
        }
    }

    IEnumerator AttackProcess(float time)//Процесс атаки
    {
        attack = true;
        yield return new WaitForSeconds(time);
        attack = false; 
    }

    #endregion //AnimatedActions

    public void SetStats(Stats _stats)
    {
        stats = _stats;
    }
}

//Класс таймеров
public class Timer
{
    public float value;
    public float maxValue;

    public Timer(float _value)
    {
        value =-1f;
        maxValue = _value;
    }

    public Timer(float _value, float _maxValue)
    {
        value = _value;
        maxValue = _maxValue;
    }

    public static Timer operator -(Timer timer, float _value)
    {
        return new Timer(timer.value - _value, timer.maxValue);
    }

    public static bool operator ==(Timer timer, float _value)
    {
        return (timer.value==_value);
    }

    public static bool operator !=(Timer timer, float _value)
    {
        return (timer.value != _value);
    }

    public static bool operator >(Timer timer, float _value)
    {
        return (timer.value > _value);
    }

    public static bool operator <(Timer timer, float _value)
    {
        return (timer.value < _value);
    }

    public static bool operator >=(Timer timer, float _value)
    {
        return (timer.value >= _value);
    }

    public static bool operator <=(Timer timer, float _value)
    {
        return (timer.value <= _value);
    }

    public void TimeStart()
    {
        value = maxValue;
    }

    public void TimeReset()
    {
        value = -1f;
    }
}
