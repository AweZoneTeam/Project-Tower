using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Скрипт, управляющий пламенем
/// </summary>
public class FireScript : MonoBehaviour
{

    #region fields

    protected HitController hitBox;

    public HitClass attackData;//Какой урон наносит пламя
    public List<string> enemies = new List<string>();

    #endregion //fields

    #region parametres

    public float lifeTime = 10f;//Сколько времени горит пламя

    private bool attack = false;

    #endregion //parametres

    void Start()
    {
        Initialize();
        StartCoroutine(BurnProcess(lifeTime));
    }

    void FixedUpdate()
    {
        if (!attack)
        {
            StartCoroutine(AttackProcess(attackData.beginTime - attackData.endTime));
        }
    }

    void Initialize()
    {
        hitBox = transform.GetChild(0).GetComponentInChildren<HitController>();
        hitBox.SetEnemies(enemies);
    }

    /// <summary>
    /// Процесс аттаки
    /// </summary>
    IEnumerator AttackProcess(float attackTime)
    {
        attack = true;
        hitBox.SetHitBox(attackTime, attackData);
        yield return new WaitForSeconds(attackTime);
        attack = false;
    }

    /// <summary>
    /// Процесс сгорания
    /// </summary>
    IEnumerator BurnProcess(float burnTime)
    {
        yield return new WaitForSeconds(burnTime);
        Destroy(gameObject);
    }

}
