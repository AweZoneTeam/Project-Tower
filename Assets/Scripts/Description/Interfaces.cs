using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public interface ILeverActivated : IEventSystemHandler
{
    void LeverActivation();
}