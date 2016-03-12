using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public interface ILeverActivated : IEventSystemHandler
{
    void LeverActivation();
}

/// <summary>
/// Интерфейс, которым я хочу реализовать способность персонажей наблюдать за действиями других персонажей
/// </summary>
public interface IPersonWatching : IEventSystemHandler
{
    void TargetMakeAnAction(string actionType);//Так например, если скелет следует за игроком и он в поле его зрения, то скелет заметит, что игрок открывает дверь.
}