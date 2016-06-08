using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, представляющий собой точки, которые можно поджечь
/// </summary>
[System.Serializable]
public class FlamePoint: MonoBehaviour
{
    public float x;//x-координата точки горения
    public bool burned;//Горит ли эта точка

    private OilTraceActions oilTrace;//Какой дороге из масла принадлежит этот флэймпоинт
    public OilTraceActions OilTrace { get { return oilTrace; } set { oilTrace = value; } }

    public void SetPoint(float _x)
    {
        x = _x;
        burned = false;
    }

    /// <summary>
    /// Заставить флэймпоинт гореть
    /// </summary>
    public void Burn(float spreadTime, float burnTime)
    {
        StartCoroutine(SpreadProcess(spreadTime));
        StartCoroutine(BurningProcess(burnTime));
    }

    /// <summary>
    /// Процесс распространения огня
    /// </summary>
    private IEnumerator SpreadProcess(float spreadTime)
    {
        yield return new WaitForSeconds(spreadTime);
        OilTrace.SpreadFire(this);
    }

    /// <summary>
    /// Процесс распространения огня
    /// </summary>
    private IEnumerator BurningProcess(float burningTime)
    {
        burned = true;
        yield return new WaitForSeconds(burningTime);
        OilTrace.OilIsBurnt(this);
    }

}