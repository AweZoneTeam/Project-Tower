using UnityEngine;
using System.Collections;

public class CheckpointActions : InterObjActions
{

    #region parameters
    public AreaClass currentRoom;
    #endregion

    /// <summary>
    /// Произвести взаимодействие
    /// </summary>
    public override void Interact()
    {
        if (interactor != null)
        {
            if (interactor is KeyboardActorController)
            {
                KeyboardActorController actor = (KeyboardActorController)interactor;
                SpFunctions.SendMessage(new MessageSentEventArgs("Игра сохранена", 1,2f));
            }
        }
    }
}
