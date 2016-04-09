using UnityEngine;
using System.Collections;

/// <summary>
/// Тип статов, который ответсвенен за окружающую персонажа обстановку
/// </summary>
[System.Serializable]
public class EnvironmentStats
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

    public EnvironmentStats()
    {
    }
}
