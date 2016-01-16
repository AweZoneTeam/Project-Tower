using UnityEngine;
using System.Collections;

//Статы - это параметры персонажа, или то, как персонаж себя чувствует)

/// <summary>
///Простейший тип статов у которого есть только один параметр - в какую сторону направлен игровой объек.
/// </summary>
public class Prestats
{
	protected int direction;

	public virtual int GetDirection()
	{
		return direction;
	}
}

/// <summary>
/// Второй по сложности тип статов, который есть у тех игровых объектов, которым можно нанести урон
/// </summary>
public class Organism: Prestats 
{
	protected float health;//Здоровье
	protected float maxHealth;//Максимальное кол-во здоровья, которое может быть у персонажа
	protected int pDefence, fDefence, dDefence, aDefence;//Постоянная надбавка к защите от физ, огн, тен и яд урона
	protected int addPDefence, addFDefence, addDDefence, addADefence;//Временная надбавка к защите от тех же видов урона
	protected int stability;//Устойчивость, способность персонажа не сбиваться от атак

	public void SetHealth(float _health)
	{
		health=_health;
	}

}

/// <summary>
/// А это такой тип статов, который может, как мне кажется, охарактеризовать любое состояние "соображающего" персонажа
/// </summary>
public class Stats : Organism
{
	protected Vector2 currentSpeed;//Какова текущая скорость 
	protected Vector2 targetSpeed;//Какую скорость персонаж стремится достичь
	protected int employment;//Насколько персонаж "занят" - этим параметром можно 
							//охарактеризовать, сколько ещё действий персонаж способен совершить
	protected int groundness;//Положение персонажа относительно земли: 0 - стоит, 1-почти приземлился, 2-присел, 3-прыгнул
	protected int obstacleness;//Какие твёрдые препятствия окружают персонажа?
	protected int interaction;//Взаимодействует ли персонаж с чем-либо в данный момент
	protected int maxInteraction;//Какой максимальный номер приоритетности среди объектов, 
								//с которыми персонаж может взаимодействовать
	protected int hitted;//Насколько "сбит" персонаж с ног влетевшей атакой. 
                         //0-атака не сбила его либо её вообще не было, 1-атака привела его в микростан, 2-атака сильно сбила его

    public void SetTargetSpeed(Vector2 speed)
    {
        targetSpeed = speed;
    }

    public void SetCurrentSpeed(Vector2 speed)
    {
        currentSpeed = speed;
    }

    public Vector2 GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
