using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Базовый класс коммуникаторов - скриптов, ответственных за взаимодействие с внешним миром
/// </summary>
public class PreInteractionChecker : MonoBehaviour
{
    #region consts

    protected const float platformOffsetY = 4f;

    protected float zCoordinateOffset = 0.5f;//величина обхода других персонажей

    #endregion //consts

    #region fields

    protected PersonController person;
    public PersonController Person { set { person = value; } }

    protected Rigidbody rigid;
    public Rigidbody Rigid { set { rigid = value; } }

    protected LayerMask whatIsCharacter = LayerMask.GetMask("character");//Какие объекты считать за персонажей

    protected List<PlatformClass> platforms = new List<PlatformClass>();

    protected List<PreInteractionChecker> characters = new List<PreInteractionChecker>();//Лист, используемый для обхода одним персонажа другим, и следовательно для симуляции 2D-толпы

    protected Transform platformCheck;

    #endregion //fields

    #region parametres

    protected float zCoordinate = 0f;

    public float ZCoordinate
    {
        get { return zCoordinate; }
        set { zCoordinate = 0f; }
    }

    protected traceEnum trace=traceEnum.forward;//на какой дорожке находится коллайдер персонажа? на передней или задней?

    public traceEnum Trace {get { return trace; }}

    #endregion //parametres

    public virtual void FixedUpdate()
    {
        if (platforms.Count > 0)
        {
            if ((platformCheck.position.y >= platforms[0].transform.position.y + platformOffsetY / 4) && (zCoordinate != platforms[0].zCoordinate))
            {
                zCoordinate = platforms[0].zCoordinate;
                person.ChangeColliderZCoordinate(zCoordinate + (int)trace*zCoordinateOffset);
            }
            else if ((platformCheck.position.y + platformOffsetY < platforms[0].transform.position.y) && (zCoordinate == platforms[0].zCoordinate))
            {
                zCoordinate = 0f;
                person.ChangeColliderZCoordinate(zCoordinate + (int)trace*zCoordinateOffset);
            }
        }
    }

    public virtual void Initialize()
    {
        trace = traceEnum.forward;
        zCoordinate = 0f;
        platformCheck = transform.parent.FindChild("PlatformCheck");
        platforms = new List<PlatformClass>();
        characters = new List<PreInteractionChecker>();
        if (person != null)
        {
            person.ChangeColliderZCoordinate(zCoordinate + (int)trace * zCoordinateOffset);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {

        #region PlatformInteractions

        if (string.Equals(other.gameObject.tag, Tags.platform))
        {
            PlatformClass platform = other.gameObject.GetComponent<PlatformClass>();
            if (!platforms.Contains(platform))
            {
                platforms.Add(platform)
                    ;
            }
        }

        #endregion //PlatformInteractions

        #region characterInteractions

        if (other.gameObject.layer==LayerMask.NameToLayer("characterInteraction"))
        {
            PreInteractionChecker intChecker = other.gameObject.GetComponent<PreInteractionChecker>();
            if (intChecker?!characters.Contains(other.gameObject.GetComponent<PreInteractionChecker>()):false)
            {
                bool forwardTrace = true, backwardTrace = true;
                characters.Add(intChecker);
                foreach (PreInteractionChecker checker in characters)
                {
                    if (checker.Trace == traceEnum.backward)
                    {
                        backwardTrace = false;
                    }
                    else if (checker.Trace == traceEnum.forward)
                    {
                        forwardTrace = false;
                    }
                    trace = forwardTrace ? traceEnum.forward : (backwardTrace ? traceEnum.backward : trace);
                    person.ChangeColliderZCoordinate(zCoordinate + (int)trace * zCoordinateOffset);
                }    
            }
        }

        #endregion //characterInteractions

    }

    public virtual void OnTriggerExit(Collider other)
    {

        #region PlatformInteractions

        if (string.Equals(other.gameObject.tag, Tags.platform))
        {
            PlatformClass platform = other.gameObject.GetComponent<PlatformClass>();
            if (platforms.Contains(platform))
            {
                platforms.Remove(platform);
                zCoordinate = 0f;
                person.ChangeColliderZCoordinate(zCoordinate + (int)trace * zCoordinateOffset);
            }
        }

        #endregion //PlatformInteractions

        #region characterInteractions

        if (other.gameObject.layer == LayerMask.NameToLayer("characterInteraction"))
        {
            PreInteractionChecker intChecker = other.gameObject.GetComponent<PreInteractionChecker>();
            if (intChecker ? characters.Contains(other.gameObject.GetComponent<PreInteractionChecker>()) : false)
            {
                bool forwardTrace = true, backwardTrace = true;
                characters.Remove(intChecker);
                foreach (PreInteractionChecker checker in characters)
                {
                    if (checker.Trace == traceEnum.backward)
                    {
                        backwardTrace = false;
                    }
                    else if (checker.Trace == traceEnum.forward)
                    {
                        forwardTrace = false;
                    }
                    trace = forwardTrace ? traceEnum.forward : (backwardTrace ? traceEnum.backward : trace);
                    person.ChangeColliderZCoordinate(zCoordinate + (int)trace * zCoordinateOffset);
                }
            }
        }

        #endregion //characterInteractions

    }

    public virtual List<InterObjController> GetInteractionList()
    {
        return null;
    }

    public virtual List<DropClass> GetDropList()
    {
        return null;
    }

}
