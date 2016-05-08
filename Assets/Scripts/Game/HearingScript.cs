using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Скрипт, что ответственен за слух персонажей
/// </summary>
public class HearingScript : MonoBehaviour
{

    #region consts

    protected float minVelocity = 5f;

    #endregion //consts

    #region parametres

    protected float hearingRadius;
    public float HearingRadius{set { hearingRadius = value; }}

    #endregion //parametres

    #region fields

    protected SphereCollider hearingSphere;

    protected List<PersonController> whoIHear=new List<PersonController>();
    public List<PersonController> WhoIHear {get { return whoIHear; }}

    #endregion //fields

    public void Initialize()
    {
        hearingSphere = GetComponent<SphereCollider>();
        hearingSphere.radius = hearingRadius;
    }

    public void OnTriggerStay(Collider other)
    {
        PersonController person;
        Rigidbody rigid;
        if (((person = other.gameObject.GetComponent<PersonController>()) != null?person.GetEnvStats().groundness==groundnessEnum.grounded:false)&&
            ((rigid=other.gameObject.GetComponent<Rigidbody>())!=null?rigid.velocity.magnitude>minVelocity:false))
        {
            if (!whoIHear.Contains(person))
            {
                whoIHear.Add(person);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        PersonController person;
        if ((person = other.gameObject.GetComponent<PersonController>()) != null)
        {
            if (whoIHear.Contains(person))
            {
                whoIHear.Remove(person);
            }
        }
    }

}
