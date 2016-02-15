using UnityEngine;
using System.Collections;


/// <summary>
/// Скрипт, ответственный за смену суток, а точнее за передвижение солнца и связанное с этим измененние глобальной освещённости.
/// </summary>
public class SunController : MonoBehaviour {


    #region consts
    const float sunIntensity = 1.3f;
    const float moonIntensity = 0.7f;
    const float dayIntensity = 2f;
    const float nightIntensity = 0.25f;
    #endregion //consts

    private Light sun; //Само Солнце и сама Луна
    private float sunSpeed;//Коэффициент линейной зависимости относительного положение Солнца от игрового времени

	void Start ()
    {
        sun = gameObject.GetComponent<Light>();
        sunSpeed = 360f / GameTime.dayTime;
	}

	void FixedUpdate ()
    {
        if ((GameTime.timer > GameTime.dayTime / 4) && (GameTime.timer < GameTime.dayTime * 3 / 4))
        {
            RenderSettings.ambientIntensity = dayIntensity;
            sun.intensity = sunIntensity;
            transform.eulerAngles = new Vector3((GameTime.timer-GameTime.dayTime/4) * sunSpeed, 180f, 0f);
        }
        else
        {
            RenderSettings.ambientIntensity = nightIntensity;
            sun.intensity = moonIntensity;
            transform.eulerAngles = new Vector3(180-(GameTime.timer<GameTime.dayTime/4? 
                                                (GameTime.dayTime / 4 + GameTime.timer): 
                                                (GameTime.timer - GameTime.dayTime*3/ 4)) * sunSpeed, 180f, 0f);
        }
	}


}
