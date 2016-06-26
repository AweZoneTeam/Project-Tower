using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, представляющий собой набор действий для дырки, сквозь которую можно смотреть
/// </summary>
public class HoleActions : LampActions
{

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<InterObjController>() != null)
        {
            if (interactor == other.gameObject.GetComponent<InterObjController>())
            {
                lightSource.SetActive(false);
            }
        }
    }

}
