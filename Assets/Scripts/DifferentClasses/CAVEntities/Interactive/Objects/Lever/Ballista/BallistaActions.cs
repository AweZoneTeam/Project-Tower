using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallistaActions: MechanismActions
{


    #region consts

    protected float chargeTime = 1f;

    #endregion //consts

    #region fields

    [SerializeField] protected bool charged=false;//Заряжена ли баллиста
    [SerializeField] protected BowClass arrowData;//Параметры атаки, используемые баллистой

    protected List<string> enemies = new List<string>();

    #endregion //fields 

    /// <summary>
    /// Функция взаимодействия с баллистой
    /// </summary>
    public override void Interact()
    {
        if (!activated)
        {
            activated = true;
            if (!charged)
            {
                StartCoroutine(ChargeProcess());
            }
            else
            {
                if ((PersonController)interactor != null)
                {
                    enemies = ((PersonController)interactor).enemies;//баллиста не стреляет по своим
                }
                Attack();
            }
            if (anim != null)
            {
                anim.Activate(true);
            }
        }
    }

    /// <summary>
    /// Атака
    /// </summary>
    protected void Attack()
    {
        StartCoroutine(AttackProcess());
    }

    /// <summary>
    /// Процесс атаки баллисты
    /// </summary>
    protected IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(arrowData.hitData.hitTime-arrowData.hitData.beginTime);
        GameObject arrow = Instantiate(arrowData.arrow, transform.position, transform.rotation) as GameObject;
        HitController hit = arrow.GetComponent<HitController>();
        hit.hitData = arrowData.hitData;
        hit.SetEnemies(enemies);
        arrow.GetComponent<Arrow>().SetEnemies(enemies);
        arrow.GetComponent<Rigidbody>().AddForce(new Vector3(arrowData.shotForce*SpFunctions.RealSign(transform.localScale.x),0f,0f));
        activated = false;
        charged = false;
    }

    /// <summary>
    /// Процесс зарядки баллисты
    /// </summary>
    protected IEnumerator ChargeProcess()
    {
        yield return new WaitForSeconds(chargeTime);
        activated = false;
        charged = true;
    }

}
