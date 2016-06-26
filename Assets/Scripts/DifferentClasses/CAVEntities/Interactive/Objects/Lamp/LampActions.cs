using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, представляющий лампы и другие источники света
/// </summary>
public class LampActions : InterObjActions
{

    #region fields

    public GameObject lightSource;// Источник света

    #endregion //fields

    /// <summary>
    /// Произвести взаимодействие
    /// </summary>
    public override void Interact()
    {
        if (interactor != null)
        {
            lightSource.SetActive(!lightSource.activeSelf);
        }
    }

}
