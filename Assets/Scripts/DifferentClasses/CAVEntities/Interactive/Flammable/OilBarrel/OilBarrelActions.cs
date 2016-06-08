using UnityEngine;
using System.Collections;
using GAF.Core;

/// <summary>
/// Действия, выполняемые бочкой с маслом, которая может взорваться
/// </summary>
public class OilBarrelActions : FlammableActions
{

    #region fields

    private HitController hitBox;
    private bool explode = false;

    public HitClass hitData;//Данные об атаке

    #endregion //fields

    #region parametres

    public float explosionTime = .6f;//Сколько времени длится сам взрыв
    public float preExplosionTime;//Сколько времени должно пройти перед взрывом бомбы

    #endregion //parametres

    void FixedUpdate()
    {
        if (explode)
        {
            StartCoroutine(ExplosionProcess());
            explode = false;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        hitBox = GetComponentInChildren<HitController>();
        explode = false;
        //hitBox.SetEnemies(null);
    }

    /// <summary>
    /// Действия, вызываемые при поджоге
    /// </summary>
    public override void BurnAction()
    {
        StartCoroutine(PreExplosionProcess(preExplosionTime));
    }

    /// <summary>
    /// Действия, вызываемые при поджоге
    /// </summary>
    public override void BurnAction(Vector3 flamePosition)
    {
        StartCoroutine(PreExplosionProcess(preExplosionTime));
    }

    IEnumerator PreExplosionProcess(float _time)
    {
        yield return new WaitForSeconds(_time);
        explode = true;
    }

    IEnumerator ExplosionProcess()
    {
        if ((OilBarrelVisual)anim != null)
        {
            ((OilBarrelVisual)anim).Explode();
        }
        GameObject hBox = hitBox.gameObject;
        hBox.GetComponent<BoxCollider>().size = hitData.hitSize;
        this.hitBox.SetHitBox(explosionTime, hitData);
        yield return new WaitForSeconds(explosionTime);
        Destroy(gameObject);
    }
}
