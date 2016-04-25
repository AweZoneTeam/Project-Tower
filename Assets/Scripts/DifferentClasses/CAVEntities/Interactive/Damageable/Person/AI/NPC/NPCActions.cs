using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, что исполняет действия ИИ, характеризующие NPC. 
/// </summary>
public class NPCActions : AIActions
{
    /// <summary>
    /// Произвести взаимодействие - начать общение с игроком
    /// </summary>
    public override void Interact()
    {
        if (interactor != null)
        {
            SpFunctions.SendMessage(new MessageSentEventArgs("Приветствую тебя путник!",1,2f));

        }
    }
}
