using UnityEngine;
using System.Collections;

//Статы - это параметры персонажа, или то, как персонаж себя чувствует)

/// <summary>
///Простейший тип статов у которого есть только один параметр - в какую сторону направлен игровой объек.
/// </summary>
[System.Serializable]
public class Prestats
{
	public int direction;


}

/// <summary>
/// Второй по сложности тип статов, который есть у тех игровых объектов, которым можно нанести урон
/// </summary>
[System.Serializable]
public class Organism: Prestats 
{
    public float health;//Здоровье
    public float maxHealth;//Максимальное кол-во здоровья, которое может быть у персонажа
    public int pDefence, fDefence, dDefence, aDefence;//Постоянная надбавка к защите от физ, огн, тен и яд урона
    public int addPDefence, addFDefence, addDDefence, addADefence;//Временная надбавка к защите от тех же видов урона
	public int stability;//Устойчивость, способность персонажа не сбиваться от атак

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
    public int groundness;//Положение персонажа относительно земли: 0 - стоит, 1-почти приземлился, 2-присел, 3-прыгнул
    public int obstacleness;//Какие твёрдые препятствия окружают персонажа?
    public int interaction;//Взаимодействует ли персонаж с чем-либо в данный момент
    public int maxInteraction;//Какой максимальный номер приоритетности среди объектов, 
                              //с которыми персонаж может взаимодействовать
    public int hitted;//Насколько "сбит" персонаж с ног влетевшей атакой. 
                         //0-атака не сбила его либо её вообще не было, 1-атака привела его в микростан, 2-атака сильно сбила его
}
