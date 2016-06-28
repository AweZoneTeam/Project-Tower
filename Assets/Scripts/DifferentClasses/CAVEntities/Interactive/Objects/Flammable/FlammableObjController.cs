using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Контроллер поджигаемых объектов.
/// </summary>
public class FlammableObjController : InterObjController
{

    #region fields

    protected bool burned = false;//Подожжён ли объект

    #endregion //fields

    #region parametres

    public float minFlameHit;//Какой должен быть минимальный огонь, чтобы запустился процесс горения. Если огонь по значению меньше, то вызовется другое действие 
                            //(например, динамит можно поджечь, но если огонь слишком силён, то динамит сразу взорвётся)

    #endregion //parametres

    /// <summary>
    /// Функция, ответственная за поджог
    /// </summary>
    public virtual void OnTriggerEnter(Collider other)
    {
        if (!burned)
        {
            HitController hitControl;
            if ((hitControl = other.GetComponent<HitController>()) != null)
            {
                if (hitControl.hitData != null)
                {
                    if (hitControl.hitData.fDamage > minFlameHit)
                    {
                        MakeItBurn(other.transform.position);
                    }
                    else
                    {
                        MakeItBurnSmall(other.transform.position);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Заставить объект гореть
    /// </summary>
    public virtual void MakeItBurn()
    {
        burned = true;
        if ((FlammableActions)intActions != null)
        {
            ((FlammableActions)intActions).BurnAction();
        }
    }

    /// <summary>
    /// Заставить объект гореть
    /// </summary>
    public virtual void MakeItBurn(Vector3 flamePosition)
    {
        burned = true;
        if ((FlammableActions)intActions != null)
        {
            ((FlammableActions)intActions).BurnAction(flamePosition);
        }
    }

    /// <summary>
    /// Действия, что происходят при слабом огне
    /// </summary>
    public virtual void MakeItBurnSmall()
    {
        if ((FlammableActions)intActions != null)
        {
            ((FlammableActions)intActions).SmallBurnAction();
        }
    }

    /// <summary>
    /// Действия, что происходят при слабом огне
    /// </summary>
    public virtual void MakeItBurnSmall(Vector3 flamePosition)
    {
        if ((FlammableActions)intActions != null)
        {
            ((FlammableActions)intActions).SmallBurnAction();
        }
    }

    /// <summary>
    /// Проинициализировать обхект, учитывая сохранённые данные
    /// </summary>
    public override void AfterInitialize(InterObjData intInfo, Map map, Dictionary<string, InterObjController> savedIntObjs)
    {
        base.AfterInitialize(intInfo, map, savedIntObjs);
        FlameObjData flInfo = (FlameObjData)intInfo;
        if (flInfo != null)
        {
            burned = flInfo.burned;
            if (burned)
            {
                MakeItBurn();
            }
        }
    }

    /// <summary>
    /// Получить информацию об объекте
    /// </summary>
    public override InterObjData GetInfo()
    {
        FlameObjData intInfo = new FlameObjData();
        intInfo.objId = objId;
        if (spawnId != null ? !string.Equals(spawnId, string.Empty) : false)
        {
            intInfo.spawnId = spawnId;
        }
        else
        {
            intInfo.spawnId = string.Empty;
        }
        intInfo.position = transform.position;
        intInfo.roomPosition = currentRoom.id.areaName;
        intInfo.orientation = (int)direction.dir;
        intInfo.burned = burned;
        return intInfo;
    }

}
