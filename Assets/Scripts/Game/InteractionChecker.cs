using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, что представляет собой объекты, которые детектируют объекты, готовые к взаимодействию.
/// </summary>
public class InteractionChecker : MonoBehaviour {

    #region consts

    protected const float platformOffsetY =0.5f;

    #endregion //consts

    private PersonController person;

    public List<InterObjController> interactions;

    public List<DropClass> dropList=new List<DropClass>();

    public List<PlatformClass> platforms=new List<PlatformClass>();
    private Transform platformCheck;
    private float zCoordinateOffset = -0.5f;
    private float zCoordinate = 0f;

    public void Awake()
    {
        SetZCoordinate();
        platforms = new List<PlatformClass>();
        platformCheck = transform.parent.FindChild("PlatformCheck");
        person = GetComponentInParent<PersonController>();
        person.ChangeColliderZCordinate(zCoordinate + zCoordinateOffset);
    }

    public void FixedUpdate()
    {
        if (platforms.Count>0)
        {
            if ((platformCheck.position.y >= platforms[0].transform.position.y+platformOffsetY) && (zCoordinate != platforms[0].zCoordinate))
            {
                zCoordinate = platforms[0].zCoordinate;
                person.ChangeColliderZCordinate(zCoordinate + zCoordinateOffset);
            }
            else if ((platformCheck.position.y+platformOffsetY < platforms[0].transform.position.y) && (zCoordinate == platforms[0].zCoordinate))
            {
                zCoordinate = 0f;
                person.ChangeColliderZCordinate(zCoordinate + zCoordinateOffset);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        #region GeneralInteractions

        if (string.Equals(other.gameObject.tag, Tags.interactive))
        {
            InterObjController interaction = other.gameObject.GetComponent<InterObjController>();
            if (!interactions.Contains(interaction))
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("ledge"))
                {
                    interactions.Insert(0, interaction);
                }
                else
                {
                    interactions.Add(interaction);
                }
            }
        }

        #endregion //GeneralInteractions

        #region DropInteractions

        else if (string.Equals(other.gameObject.tag, Tags.drop))
        {
            DropClass drop = other.gameObject.GetComponent<DropClass>();
            if (!dropList.Contains(drop))
            {
                if (drop.autoPick)
                {
                    dropList.Insert(0, drop);
                }
                else
                {
                    dropList.Add(drop);
                }
            }
        }

        #endregion //DropInteractions

        #region PlatformInteractions

        else if (string.Equals(other.gameObject.tag, Tags.platform))
        {
            PlatformClass platform = other.gameObject.GetComponent<PlatformClass>();
            if (!platforms.Contains(platform))
            {
                platforms.Add(platform);
            }
        }

        #endregion //PlatformInteractions

    }

    public void OnTriggerExit(Collider other)
    {

        #region GeneralInteractions

        if (string.Equals(other.gameObject.tag, Tags.interactive))
        {
            InterObjController interaction = other.gameObject.GetComponent<InterObjController>();
            if (interactions.Contains(interaction))
            {
                interactions.Remove(interaction);
            }
        }

        #endregion //GeneralInteractions

        #region DropInteractions

        else if (string.Equals(other.gameObject.tag, Tags.drop))
        {
            DropClass drop = other.gameObject.GetComponent<DropClass>();
            if (dropList.Contains(drop))
            {
                dropList.Remove(drop);
            }
        }

        #endregion //DropInteractions

        #region PlatformInteractions

        else if (string.Equals(other.gameObject.tag, Tags.platform))
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

    public void SetZCoordinate()
    {
        zCoordinate = transform.position.z;
    }

}
