using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, предоставляющий информацию о том, какое взаимодействие  персонаж произведёт в данный момент.
/// </summary>
[System.Serializable]
public class InteractionInfo
{
    public interactionInfoEnum interactionType;//Тип возможного взаимодействия
    public string interactionMessage;//Какое сообщение выведется на экран при появлении возможности взаимодействия
    public GameObject interObj;//Объект, с которым можно провзаимодействовать

    public InteractionInfo(interactionInfoEnum _interactionType, string _interactionMessage, GameObject _interObj)
    {
        interactionType = _interactionType;
        interactionMessage = _interactionMessage;
        interObj = _interObj;
    }
}