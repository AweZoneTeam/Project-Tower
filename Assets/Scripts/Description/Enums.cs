using UnityEngine;
using System.Collections;

/// <summary>
/// Ориентация сущности
/// </summary>
public enum orientationEnum
{
    left=-1,
    right=1,
    max=10
}

/// <summary>
/// Положение относительно земли
/// </summary>
public enum groundnessEnum { grounded = 1, crouch, preGround, inAir };

/// <summary>
/// состояние из которого производится атака
/// </summary>
public enum attackState { stay, walk, run, crouch, up, jump, jumpDawn, NO };

/// <summary>
/// В какую модель поведения использует данный представитель искусственного интеллекта
/// </summary>
public enum behaviourEnum {calm, agression, search};

/// <summary>
/// Как сильно повлияла на персонажа совершённая атака
/// </summary>
public enum hittedEnum {noHit, microStun, pushedForward, pushedUp, pushedDown, macroStun};

/// <summary>
/// Какой (самый "интересный") тип препятствия окружает персонажа 
/// </summary>
public enum obstaclenessEnum {noObstcl, lowObstcl, highObstcl, wall};

/// <summary>
/// Номера приоритетности у взаимодействующих с персонажем объектов. Нужно ли это, покажет время
/// </summary>
public enum interactionEnum {noInter, stair, rope, thicket, ledge, platform, edge, lowEdge, interactive};

public enum doorEnum { left, right, back, forward, down, up, everywhere };//За какой тип двери отвечает каждый номер типа
