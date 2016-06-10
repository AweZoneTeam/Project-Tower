using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, что исполняет действия ИИ, характеризующие NPC. 
/// </summary>
public class NPCActions : AIActions
{
    public List<NPCSpeech> speeches;

    public string defaultSpeech;//Что скажет NPC, если canTalk = false
    public bool canTalk = true;//Если true, то откроется окно диалога, иначе этот персонаж просто вслух скажет defaultSpeech

    /// <summary>
    /// Произвести взаимодействие - начать общение с игроком
    /// </summary>
    public override void Interact()
    {
        if (interactor != null && speeches!=null)
        {
            if (canTalk)
            {
                SpFunctions.BeginDialog(this);
            }
            else
            {
                SpFunctions.SendMessage(new MessageSentEventArgs(defaultSpeech, 1, 3f));
            }
        }
    }
}
