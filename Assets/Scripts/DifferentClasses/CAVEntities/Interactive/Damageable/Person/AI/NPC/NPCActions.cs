using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, что исполняет действия ИИ, характеризующие NPC. 
/// </summary>
public class NPCActions : AIActions
{
    public List<NPCSpeech> speeches;
    public DialogWindow dialogWindow;
    /// <summary>
    /// Произвести взаимодействие - начать общение с игроком
    /// </summary>
    public override void Interact()
    {
        if (interactor != null && speeches!=null)
        {
            dialogWindow.Init(this);
        }
    }
}
