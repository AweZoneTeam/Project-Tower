using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Скрипт, координирующий световые эффекты в игре
/// </summary>
public class IlluminationController : MonoBehaviour
{
    #region consts

    const float sunIntensity = 1f;
    const float moonIntensity = 0.5f;
    const float defNightIntensity = 0.2f;
    const float defDayIntensity = 1f;

    #endregion //consts

    #region parametres

    protected float dayIntensity=defDayIntensity, nightIntensity=defNightIntensity;

    private float sunSpeed;//Коэффициент линейной зависимости относительного положение Солнца от игрового времени
    public float ambBrightness;//Насколько ярок свет окружения  

    #endregion //parametres

    #region fields

    public Light sun, moon; //Солнце и сама Луна собственной персоной

    public float phase = 0f;
    public Material ambientLight;   

    private Camera cam;

    #endregion //fields

    void Start()
    {
        SetIntensity(defDayIntensity, defNightIntensity);
        sunSpeed = 360f / GameTime.dayTime;
        cam = GameObject.FindGameObjectWithTag(Tags.cam).GetComponent<Camera>();
        sun.intensity = sunIntensity;
        moon.intensity = moonIntensity;
    }

    void FixedUpdate()
    {
        phase = GameTime.timer / GameTime.dayTime;
        if (phase >7f/ 8f)
        {
            RenderSettings.skybox.SetFloat("_Blend",  phase*8f-7f);
        }
        else if (phase<3f/8f)
        {
            RenderSettings.skybox.SetFloat("_Blend", 1f - GameTime.timer/ GameTime.dayTime * 8/3);
        }
        else
        {
            RenderSettings.skybox.SetFloat("_Blend", 0f);
        }
        sun.gameObject.transform.eulerAngles = new Vector3((GameTime.timer - GameTime.dayTime * 3 / 4) * -1f * sunSpeed, 180f, 0f);
        moon.gameObject.transform.eulerAngles = new Vector3((GameTime.timer - GameTime.dayTime / 4) * -1f * sunSpeed, 180f, 0f);
        ambBrightness = phase < 0.5f ? dayIntensity * (phase * 2) + nightIntensity * (1f - phase * 2) :
                                                                dayIntensity * (2f - (phase * 2)) + nightIntensity * (phase * 2-1f);
        ambientLight.SetFloat("_Scale", ambBrightness);
    }

    /// <summary>
    /// Задать интенсивности дневного и ночного света
    /// </summary>
    public void SetIntensity(float _dayIntensity, float _nightIntensity)
    {
        dayIntensity = _dayIntensity;
        nightIntensity = _nightIntensity;
    }
    
}

