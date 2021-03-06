﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Набор простейших действи, которые может совершать объект, имеющий здоровье
/// </summary>
public class DmgObjActions : InterObjActions
{

    #region consts
    const float dropX1 = -1500f;
    const float dropX2 = 1500f;
    const float dropY1 = 1000f;
    const float dropY2 = 4000f;
    #endregion //consts

    #region fields

    protected OrganismStats orgStats;
    protected DmgObjVisual dmgAnim;

    #endregion //fields

    public List<GameObject> dropList = new List<GameObject>();//Какие предметы выпадают из персонажа после его смерти

    public override void Initialize()
    {
        dmgAnim = GetComponentInChildren<DmgObjVisual>();
    }

    public void SetOrgStats(OrganismStats _orgStats)
    {
        orgStats = _orgStats;
    }

    #region interface

    /// <summary>
    /// Как персонаж реагирует на удар
    /// </summary>
    public virtual void Hitted()
    {
    }

    /// <summary>
    /// Процесс смерти персонажа
    /// </summary>
    public virtual void Death()
    {
        Drop();
    }

    /// <summary>
    /// Инициировать процесс выброса предметов
    /// </summary>
    public virtual void Drop()
    {
        GameObject obj;
        for (int i = 0; i < dropList.Count; i++)
        {
            obj = Instantiate(dropList[i], transform.position, transform.rotation) as GameObject;
            obj.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(dropX1, dropX2), Random.Range(dropY1, dropY2), 0f));
        }
    }

    /// <summary>
    /// Произвести взаимодействие
    /// </summary>
    public override void Interact()
    {
        base.Interact();
    }
    #endregion //interface

}
