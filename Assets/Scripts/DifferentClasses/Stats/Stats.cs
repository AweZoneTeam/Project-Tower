using UnityEngine;
using System.Collections;

//Статы - это параметры персонажа, или то, как персонаж себя чувствует)

/// <summary>
///Простейший тип статов у которого есть только один параметр - в какую сторону направлен игровой объек.
/// </summary>
[System.Serializable]
public class Prestats
{
	public orientationEnum direction;

    public Prestats()
    {
        direction = orientationEnum.right;
    }
}

/// <summary>
/// Второй по сложности тип статов, который есть у тех игровых объектов, которым можно нанести урон
/// </summary>
[System.Serializable]
public class Organism: Prestats 
{

    #region timers
    public float stunTimer;
    #endregion //timers 

    public float health;//Здоровье
    public float maxHealth;//Максимальное кол-во здоровья, которое может быть у персонажа
    public int pDefence, fDefence, dDefence, aDefence;//Постоянная надбавка к защите от физ, огн, тен и яд урона
    public int addPDefence, addFDefence, addDDefence, addADefence;//Временная надбавка к защите от тех же видов урона
	public int stability;//Устойчивость, способность персонажа не сбиваться от атак
    public hittedEnum hitted;//Насколько "сбит" персонаж с ног влетевшей атакой. 
                      //0-атака не сбила его либо её вообще не было, 1-атака привела его в микростан, 2-атака сильно сбила его
    public float microStun, macroStun;//Как долго персонаж бездействует при hitted=1 и hitted>1 соотвественно

    public Organism()
    {
        maxHealth = 100f;
        health = 100f;
        stability = 1;
    }

    public IEnumerator Stunned(float time)//Процесс стана
    {
        float _time = time;
        while (stunTimer > 0f)
        {
            yield return new WaitForSeconds(_time);
            stunTimer -= _time;
            _time = stunTimer;
        }
        hitted = hittedEnum.noHit;
    }

}

/// <summary>
/// А это такой тип статов, который может, как мне кажется, охарактеризовать любое состояние "соображающего" персонажа
/// </summary>
[System.Serializable]
public class Stats : Organism
{
    public Vector2 currentSpeed;//Какова текущая скорость 
    public Vector2 targetSpeed;//Какую скорость персонаж стремится достичь
    public int employment;//Насколько персонаж "занят" - этим параметром можно 
                          //охарактеризовать, сколько ещё действий персонаж способен совершить
    public groundnessEnum groundness;//Положение персонажа относительно земли: 0 - стоит, 1-почти приземлился, 2-присел, 3-прыгнул
    public obstaclenessEnum obstacleness;//Какие твёрдые препятствия окружают персонажа?
    public interactionEnum interaction;//Взаимодействует ли персонаж с чем-либо в данный момент
    public interactionEnum maxInteraction;//Какой максимальный номер приоритетности среди объектов, 
                              //с которыми персонаж может взаимодействовать

    public Stats()
    {
        maxHealth = 100f;
        health = 100f;
        stability = 1;
    }
}
