using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Контроллер механизмов
/// </summary>
public class MechanismController : InterObjController
{

    #region fields

    protected bool activated;//Был ли объект активирован?
    public bool Activated {get { return activated; }set { activated = value; }}

    #endregion //fields

    /// <summary>
    /// Проинициализировать объект, используя сохранённые данные
    /// </summary>
    public override void AfterInitialize(InterObjData intInfo, Map map, Dictionary<string, InterObjController> savedIntObjs)
    {
        base.AfterInitialize(intInfo, map, savedIntObjs);
        MechData mInfo = (MechData)intInfo;
        if (mInfo != null)
        {
            activated = mInfo.activated;
            MechanismActions mActions;
            if (intActions != null)
            {
                if ((mActions = (MechanismActions)intActions) != null)
                {
                    if (mActions.activated != activated)
                    {
                        mActions.Interact();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Получить информацию об объекте
    /// </summary>
    public override InterObjData GetInfo()
    {
        MechData intInfo = new MechData();
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
        intInfo.activated = activated;
        return intInfo;
    }

}
