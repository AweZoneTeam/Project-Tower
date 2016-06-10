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
/// Енам, содержащий основные 6 направлений
/// </summary>
public enum directionEnum {left, right, down, up, back, forward, anywhere}

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
public enum obstaclenessEnum {noObstcl, lowObstcl, medObstcl, highObstcl, wall};

/// <summary>
/// Номера приоритетности у взаимодействующих с персонажем объектов. Нужно ли это, покажет время
/// </summary>
public enum interactionEnum {noInter, ladder, rope, thicket, ledge, platform, edge, lowEdge, mount, interactive};

/// <summary>
/// Возможные типы взаимодействий, назначенные на баттон interact
/// </summary>
public enum interactionInfoEnum {interupt, door, intObj, drop };

/// <summary>
/// За какой тип двери отвечает каждый номер типа
/// </summary>
public enum doorEnum { left, right, back, forward, down, up, everywhere };

/// <summary>
/// Ориентация поверхности, на которой стоит персонаж
/// </summary>
public enum groundOrientationEnum {down, right, up, left};

public enum doorConnectionEnum {ground, air, ladder, stairs, zero, obstacle };