using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Контроллер паукообразных существ
/// </summary>
public class SpiderController : AIController
{

    #region consts
    #endregion //consts

    #region indicators

    protected GroundChecker wallCheck;

    #endregion //indicators

    #region fields
    
    [SerializeField]protected GroundOrientation grOrientation = new GroundOrientation();//На какой поверхности стоит паук 

    #endregion //fields

    public override void FixedUpdate()
    {
        if ((orgStats.hitted > 0f) && (orgStats.health > 0f))
        {
            Hitted();
        }

        if (orgStats.health <= 0f)
        {
            Death();
        }

        if (!death)
        {
            if ((envStats.obstacleness != obstaclenessEnum.lowObstcl) && (envStats.interaction == interactionEnum.noInter))
            {
                if (currentBehaviour != null)
                {
                    ImplementBehaviour(currentBehaviour);
                }
            }

            else
            {
                if (pActions != null)
                {
                    pActions.AvoidLowObstacle(0f);
                }
            }
        }
        AnalyzeSituation();
    }

    public override void Initialize()
    {
        base.Initialize();
        wallCheck = transform.FindChild("Indicators").FindChild("WallCheck").GetComponent<GroundChecker>();
    }

    /// <summary>
    /// Сформировать словари действий и проверок
    /// </summary>
    protected override void FormActionDictionaries()
    {
        //Использовать весь функционал обычного ИИ
        base.FormActionDictionaries();

        //И добавить к нему такие функции:
        conditionBase.Add("ground orientation", CheckGroundOrientation);

    }

    protected override void SetAction()
    {
        base.SetAction();
        if (pActions is SpiderActions)
        {
            SpiderActions spActions = (SpiderActions)pActions;
            spActions.SetGroundOrientation(grOrientation);
        }
    }

    #region interface

    #region actions
    #endregion //actions

    #region conditions

    /// <summary>
    /// Проверить, на какой поверхности находится персонаж в данный момент
    /// </summary>
    protected virtual bool CheckGroundOrientation(string id, int argument)
    {
        if (grOrientation!=null)
        {
            return SpFunctions.ComprFunctionality((int)grOrientation.grOrientation, id, argument);
        }
        else return false;
    }

    #endregion //conditions

    #endregion interface

    /// <summary>
    /// Произвести обработку вэйпоинтов
    /// </summary>
    protected override List<TargetWithCondition> PrepareWaypoints(List<TargetWithCondition> _waypoints)
    {
        GameObject target = null;
        for (int i = 0; i < _waypoints.Count; i++)
        {
            if (string.Equals(_waypoints[i].targetType, "stairs"))
            {
                target = _waypoints[i].target;
                bool up = target.transform.position.y < _waypoints[i + 1].target.transform.position.y;
                List<GameObject> stairTurns = new List<GameObject>();
                for (int j = 0; j < transform.childCount; j++)
                {
                    if (transform.GetChild(j).gameObject.name.Contains("StairTurnPoint"))
                    {
                        stairTurns.Add(transform.GetChild(j).gameObject);
                    }
                }
                string stairType = "Forward";
                GameObject turn1 = null, turn2 = null;
                while (stairTurns.Count > 0)
                {
                    float value;
                    if (up)
                    {
                        value = -100000;
                        foreach (GameObject obj in stairTurns)
                        {
                            if (obj.transform.position.y > value)
                            {
                                turn1 = obj;
                                value = obj.transform.position.y;
                            }
                            else if (obj.transform.position.y == value)
                            {
                                turn2 = obj;
                            }
                        }
                    }
                    else
                    {
                        value = 100000;
                        foreach (GameObject obj in stairTurns)
                        {
                            if (obj.transform.position.y < value)
                            {
                                turn1 = obj;
                                value = obj.transform.position.y;
                            }
                            else if (obj.transform.position.y == value)
                            {
                                turn2 = obj;
                            }
                        }
                    }
                    _waypoints.Insert(i + 1, new TargetWithCondition((turn1.name.Contains(stairType) ? turn1 : turn2), "enter"));
                    stairType = (string.Equals(stairType, "Backward") ? "Forward" : "Backward");
                    stairTurns.Remove(turn1);
                    stairTurns.Remove(turn2);
                }
                _waypoints.RemoveAt(i);
            }
        }
        return _waypoints;
    }

    /// <summary>
    /// Проинициализировать объект, используя сохраненные данные
    /// </summary>
    public override void AfterInitialize(InterObjData intInfo, Map map, Dictionary<string, InterObjController> savedIntObjs)
    {
        base.AfterInitialize(intInfo, map, savedIntObjs);

        SpiderData spInfo = (SpiderData)intInfo;
        if (spInfo != null)
        {
            grOrientation.grOrientation = (groundOrientationEnum)spInfo.grOrientation;
        }

    }

    /// <summary>
    /// Получить информацию о персонаже
    /// </summary>
    public override InterObjData GetInfo()
    {
        SpiderData intInfo = new SpiderData();
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
        intInfo.maxHealth = orgStats.maxHealth;
        intInfo.health = orgStats.health;

        intInfo.buffs = new List<string>();
        foreach (BuffClass buff in buffList)
        {
            if (!buff.armorSetBuff)
            {
                intInfo.buffs.Add(buff.buffName);
            }
        }

        intInfo.behaviour = behaviours.Find(x => string.Equals(x.behaviour.behaviourName, currentBehaviour.behaviourName)).path;
        intInfo.mainTarget = new TargetData(mainTarget);
        intInfo.currentTarget = new TargetData(currentTarget);
        intInfo.enemies = enemies;

        NPCActions npc = null;
        if ((npc=GetComponent<NPCActions>())!=null)
        {
            intInfo.canTalk = npc.canTalk;
            if (npc.speeches.Count > 0)
            {
                intInfo.startSpeech = npc.speeches[0].speechName;
            }
            else
            {
                intInfo.startSpeech = string.Empty;
            }
        }

        intInfo.grOrientation = (int)grOrientation.grOrientation;

        return intInfo;
    }

    #region Analyze

    /// <summary>
    /// Анализировать окружающую персонажа информацию
    /// </summary>
    protected override void AnalyzeSituation()
    {
        base.AnalyzeSituation();
    }

    /// <summary>
    /// Оценить препятствие перед персонажем
    /// </summary>
    protected override void DefinePrecipice()
    {
        if (pActions.PrecipiceIsForward = (!(Physics.OverlapSphere(precipiceCheck.position, precipiceRadius, whatIsGround).Length > 0) && (envStats.groundness == groundnessEnum.grounded)))
        {
            if (!(pActions.JumpIsPossible = (DefineJumpPosibility(pActions.RunSpeed/1.2f * 1f, pActions.jumpForce * 1f, orgStats.health))
                &&(grOrientation.grOrientation==groundOrientationEnum.down)
                &&(currentTarget!=null?(currentTarget.target.transform.position.y>transform.position.y):true)))
            {
                envStats.interaction = interactionEnum.edge;
            }
            else
            {
                envStats.interaction = interactionEnum.noInter;
            }
        }
    }

    /// <summary>
    /// Определить, находится ли на пути персонажа стена
    /// </summary>
    protected override void CheckObstacles()
    {
        if (wallCheck!=null?wallCheck.GetCount() > 0:false)
        {
            envStats.obstacleness = obstaclenessEnum.lowObstcl;
        }
        else if (frontWallCheck.GetCount()>0)
        {
            envStats.obstacleness = obstaclenessEnum.wall;
        }
        else
        {
            envStats.obstacleness = obstaclenessEnum.noObstcl;
        }
    }

    #endregion //Analyze

}

