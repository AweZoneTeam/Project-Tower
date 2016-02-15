using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, представляющий собой источник 2D света. Этот скрипт контролирует
/// </summary>
public class LightSource : MonoBehaviour
{
    public float lightIntensity;//Собственная интенсивность источника
    public float maxSumIntensity;//Максимальная суммарная интенсивность источника и окружающего света.

    private IlluminationController lControl;
    private float ambBrightness;
    private Material lightMat;

    public void Awake()
    {
        lControl = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<IlluminationController>();
        lightMat = GetComponent<Renderer>().material;
    }

    public void FixedUpdate()
    {
        ambBrightness = lControl.ambBrightness;
        if (lightIntensity + ambBrightness > maxSumIntensity)
        {
            lightMat.SetFloat("_Scale", maxSumIntensity-ambBrightness);
        }
        else
        {
            lightMat.SetFloat("_Scale",lightIntensity);
        }
    }
}
