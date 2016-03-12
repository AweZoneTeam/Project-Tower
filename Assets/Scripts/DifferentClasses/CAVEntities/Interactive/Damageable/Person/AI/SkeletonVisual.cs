using UnityEngine;
using System.Collections;

public class SkeletonVisual : EnemyVisual
{

    #region consts

    private const float beginRunTime = 0.2f;//Сколько времени длится анимация "Начало бега"?

    #endregion

    #region timers

    public float beginRunTimer;

    #endregion //timers

    #region parametres
    private bool attack = false;//Во время атаки у персонажа будут совсем другие анимации, поэтому вижуалка должна знать, когда этот момент атаки наступил
    #endregion //parametres

    #region fields
    private Stats stats;
    //private Rigidbody rigid;
    #endregion //fields

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        cAnim = GetComponent<CharacterAnimator>();
        //rigid = gameObject.GetComponentInParent<Rigidbody>();
    }

    #region AnimatedActions

    /// <summary>
    /// Анимировать отсутствие активности
    /// </summary>
    public override void GroundStand()
    {
        if ((cAnim != null) && (!attack))
        {
            cAnim.Animate("Idle");
        }
    }

    /// <summary>
    /// Анимировать передвижение по земле
    /// </summary>
    public override void GroundMove()
    {
        if ((cAnim != null) && (!attack))
        {
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
    public override void AirMove()
    {
        if ((cAnim != null) && (!attack))
        {
            cAnim.Animate("Jump");
        }
    }

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

    /*public void SetStats(Stats _stats)
    {
        stats = _stats;
    }*/

}
