using UnityEngine;
using System.Collections;


/// <summary>
/// Данные о ходе игры. Здесь учитывается время, количество попыток пройти, текущее местоположение главного героя
/// </summary>
public class GameStatisics : MonoBehaviour
{
    public float timer; // Сколько уже прошло игрового времени с начала.
    public AreaClass currentArea; //В какой комнате (пространстве) персонаж находится на данный момент
    public int deathNumber; //Сколько раз уже главный герой проигрывал.


    public void Start()
    {
        //Инициализация
        if (PlayerPrefs.HasKey("Timer"))
        {
            timer = PlayerPrefs.GetFloat("Timer");
        }
        else
        {
            timer = 0f;
            PlayerPrefs.SetFloat("Timer", 0f);
        }
        deathNumber = 0;
    }

    public void Update()
    {
        timer += Time.deltaTime;
    }


}
