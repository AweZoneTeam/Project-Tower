using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GAF.Core;


/// <summary>
/// Скрипт, управляющий бомбой
/// </summary>
public class BombScript : MonoBehaviour
{

    #region consts

    private const float explosionTime = .2f;
    private const int preExplosionFPS = 20;
    private const int explosionFPS = 30;

    #endregion //consts

    #region fields

    private GAFMovieClip mov;//Анимация бомбы
    private HitController hitBox;
    private bool explode = false;

    public HitClass hitData;//Данные об атаке
    public float preExplosionTime;//Сколько времени должно пройти перед взрывом бомбы

    #endregion //fields

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        mov = GetComponent<GAFMovieClip>();
        hitBox = GetComponentInChildren<HitController>();
        explode = false;
        hitBox.SetEnemies(null);
        StartCoroutine(PreExplosionProcess(preExplosionTime));
    }

    void FixedUpdate()
    {
        if (explode)
        {
            StartCoroutine(ExplosionProcess());
            explode = false;
        }
    }

    IEnumerator PreExplosionProcess(float _time)
    {
        mov.setSequence("PreExplosion", true);
        mov.settings.targetFPS = preExplosionFPS;
        yield return new WaitForSeconds(_time);
        explode = true;
    }

    IEnumerator ExplosionProcess()
    { 
        mov.setSequence("Explosion", true);
        mov.settings.targetFPS = explosionFPS;
        GameObject hBox = hitBox.gameObject;
        hitBox.SetHitBox(explosionTime, hitData);
        yield return new WaitForSeconds(explosionTime);
        Destroy(gameObject);
    }


}
