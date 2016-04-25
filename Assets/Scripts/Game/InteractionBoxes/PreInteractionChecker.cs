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

    #endregion //consts

    protected PersonController person;
    public PersonController Person { set { person = value; } }

    protected Rigidbody rigid;
    public Rigidbody Rigid { set { rigid = value; } }

    protected List<PlatformClass> platforms = new List<PlatformClass>();

    protected Transform platformCheck;
    protected float zCoordinateOffset = -0.5f;
    protected float zCoordinate = 0f;

    public virtual void FixedUpdate()
    {
        if (platforms.Count > 0)
        {
            if ((platformCheck.position.y >= platforms[0].transform.position.y + platformOffsetY / 4) && (zCoordinate != platforms[0].zCoordinate))
            {
                zCoordinate = platforms[0].zCoordinate;
                person.ChangeColliderZCordinate(zCoordinate + zCoordinateOffset);
            }
            else if ((platformCheck.position.y + platformOffsetY < platforms[0].transform.position.y) && (zCoordinate == platforms[0].zCoordinate))
            {
                zCoordinate = 0f;
                person.ChangeColliderZCordinate(zCoordinate + zCoordinateOffset);
            }
        }
    }

    public virtual void Initialize()
    {
        SetZCoordinate();
        platformCheck = transform.parent.FindChild("PlatformCheck");
        if (person != null)
        {
            person.ChangeColliderZCordinate(zCoordinate + zCoordinateOffset);
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
                platforms.Add(platform);
            }
        }

        #endregion //PlatformInteractions

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
                person.ChangeColliderZCordinate(zCoordinate + zCoordinateOffset);
            }
        }

        #endregion //PlatformInteractions


    }

    public virtual List<InterObjController> GetInteractionList()
    {
        return null;
    }

    public virtual List<DropClass> GetDropList()
    {
        return null;
    }

    public virtual void SetZCoordinate()
    {
        zCoordinate = transform.position.z;
    }
}
