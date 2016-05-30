using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


/// <summary>
/// Контроллер объектов, которые производят действия, когда на них станут.
/// </summary>
public class PushablePlateController : InterObjController, ILeverActivated
{

    #region parametres

    protected bool pushed=false;//Нажат ли объект
    protected List<string> whatCanPushMe = new List<string> { "ground", "character" };
    protected List<int> layers = new List<int>();

    #endregion //parametres

    #region fields

    [SerializeField]protected List<GameObject> whatPushesMe=new List<GameObject>();//Какие объекты в данный момент нажимают на кнопку.  
    protected BoxCollider trapZone;//Активная зона ловушки, что воспринимает нажатие.

    #endregion //fields

    public void FixedUpdate()
    {
        if (pushed)
        {
            intActions.OnPush();
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        foreach (string layer in whatCanPushMe)
        {
            layers.Add(LayerMask.NameToLayer(layer));
        }
        trapZone = GetComponent<BoxCollider>();
        foreach (string layer in whatCanPushMe)
        {
            layers.Add(LayerMask.NameToLayer(layer));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (layers.Contains(other.gameObject.layer))
        {
            if (!whatPushesMe.Contains(other.gameObject))
            {
                whatPushesMe.Add(other.gameObject);
                if (whatPushesMe.Count == 1)
                {
                    pushed = true;
                    intActions.OnPushDown();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (layers.Contains(other.gameObject.layer))
        {
            if (whatPushesMe.Contains(other.gameObject))
            {
                whatPushesMe.Remove(other.gameObject);
                if (whatPushesMe.Count == 0)
                {
                    intActions.OnPushUp();
                    pushed = false;
                }
            }
        }
    }

    /// <summary>
    /// Функция, которую вызовет нажатие на тот рычаг, к которому подключен сундук
    /// </summary>
    public void LeverActivation()
    {
        if (trapZone.enabled)
        {
            trapZone.enabled = false;
        }
        else
        {
            trapZone.enabled = true;
        }
    }


}
