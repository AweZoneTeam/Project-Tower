using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Данные о ходе игры. Здесь учитывается время, количество попыток пройти, текущее местоположение главного героя
/// </summary>
public class GameStatisics : MonoBehaviour
{

    #region consts
    const int defMonth=7;
    const int defDay = 15;
    const int defHour = 9;
    const int defMin = 0;
    #endregion //consts

    #region variables
    public AreaClass currentArea; //В какой комнате (пространстве) персонаж находится на данный момент
    public int deathNumber; //Сколько раз уже главный герой проигрывал.
    #endregion //variables

    public void Start()
    {
        //Инициализация
        PlayerPrefs.DeleteKey("Timer");
        if (PlayerPrefs.HasKey("Timer"))
        {
            GameTime.timer = PlayerPrefs.GetFloat("Timer");
        }
        else
        {
            GameTime.timer = 135f;
            PlayerPrefs.SetFloat("Timer", 135f);
            GameTime.SetTime(defMonth, defDay, defHour, defMin);
        }
        deathNumber = 0;
    }

    public void Update()
    {
        GameTime.TimeFlow(); 
    }


}

/// <summary>
/// Игровое время. Сколько месяцев, дней, часов прошло с начала. 
/// </summary>
public static class GameTime
{
    public const float dayTime = 36f;//Сколько секунд реального времени длится день?

    public static float timer;
    public static List<Month> months = new List<Month>(new Month[]{ new Month("January", 31), new Month("February", 28),
                                                                    new Month("March",31), new Month("April",30),
                                                                    new Month("May",31), new Month("June",30),
                                                                    new Month("July",31),new Month("August",31),
                                                                    new Month("Septmber",30), new Month("October",31),
                                                                    new Month("November",30), new Month("December",31) });
    public static int monthNumb,day, hour, min;

    /// <summary>
    /// Метод, отвечающий за течение игрового времени
    /// </summary>
    public static void TimeFlow()
    {
        int sec;
        timer += Time.deltaTime;
        if (timer > dayTime)
        {
            day++;
            if (day > months[monthNumb].days)
            {
                day = 1;
                monthNumb++;
                if (monthNumb > 11)
                {
                    monthNumb = 1;
                }
            }
            timer -= dayTime;
        }
        sec = Mathf.RoundToInt(timer/dayTime*3600 * 24);
        hour = sec / 3600;
        min = (sec % 3600) / 60;
    }

    /// <summary>
    /// С помощью этого метода в новой игре выставляется заданное дефолтное время.
    /// </summary>
    /// <param name="_month"></param>
    /// <param name="_day"></param>
    /// <param name="_hour"></param>
    /// <param name="_min"></param>
    public static void SetTime(int _month, int _day, int _hour, int _min)
    {
        monthNumb = _month; day = _day; hour = _hour; min = _min; 
    }

    /// <summary>
    /// Метод, использующийся при выводе игрового времени
    /// </summary>
    /// <returns></returns>
    public static string TimeString()
    {
        return months[monthNumb].name + " " + day.ToString() + " " + hour.ToString() + ":" + min.ToString();
    }
}
/// <summary>
/// Класс, пердставляющий собой название месяца и кол-во дней в месяце
/// </summary>
public class Month
{
    public string name;
    public int days;

    public Month(string _name, int _days)
    {
        name = _name; days = _days;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof (GameStatisics))]
public class GameStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.LabelField("Time");
            EditorGUILayout.LabelField(GameTime.TimeString());
        }
        
    }
}
#endif 