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

    #region fields

    protected PersonController person;
    public PersonController Person { set { person = value; } }

    protected EnvironmentStats envStats;

    protected Rigidbody rigid;
    public Rigidbody Rigid { set { rigid = value; } }

    protected List<PlatformClass> platforms = new List<PlatformClass>();

    protected List<PreInteractionChecker> characters = new List<PreInteractionChecker>();//Лист, используемый для обхода одним персонажа другим, и следовательно для симуляции 2D-толпы

    protected Transform platformCheck;

    #endregion //fields

    #region parametres

    protected float zCoordinate = 0f;//Какую эксклюзивную z-координату занимает коллайдер персонажа (эксклюзивная, значит, что персонаж не будет сталкиваться с другими персонажами)

    public float ZCoordinate
    {
        get { return zCoordinate; }
        set {
                zCoordinate = value;
                person.ChangeColliderZCoordinate(zCoordinate);
            }
    }

    protected bool onPlatform = false;//Находится ли персонаж на платформе и смещён ли его коллайдер соответственно

    #endregion //parametres

    public virtual void FixedUpdate()
    {
        if (platforms.Count > 0)
        {
            if ((platformCheck.position.y >= platforms[0].transform.position.y + platformOffsetY / 4) && (zCoordinate != platforms[0].zCoordinate) &&
                (envStats.interaction==interactionEnum.noInter))
            {
                person.ChangeColliderZCoordinate(zCoordinate + platforms[0].zCoordinate);
            }
            else if ((platformCheck.position.y + platformOffsetY < platforms[0].transform.position.y) && (zCoordinate == platforms[0].zCoordinate))
            {
                person.ChangeColliderZCoordinate(zCoordinate);
            }
        }
    }

    public virtual void Initialize()
    {
        platformCheck = transform.parent.FindChild("PlatformCheck");
        platforms = new List<PlatformClass>();
        characters = new List<PreInteractionChecker>();
        if (person != null)
        {
            person.ChangeColliderZCoordinate(zCoordinate);
            envStats = person.GetEnvStats();
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
                person.ChangeColliderZCoordinate(zCoordinate);
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

}
