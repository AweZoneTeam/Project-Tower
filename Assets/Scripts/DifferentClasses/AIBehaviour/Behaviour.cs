﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



/// <summary>
/// Класс, в котором содержится вся информация о совершаемом ИИ одном действии
/// </summary>
[System.Serializable]
public class AIActionData
{
    public string actionName;//Название действия
    public List<ConditionSign> whyToDo=new List<ConditionSign>();//Условия совершения действия
    public int posibility;// Вероятность, что действие будет совершено - это обычное число, что используется для определения, какое именно действие из возможных будет совершено
    public List<ActionSign> whatToDo = new List<ActionSign>();//Какие элементарные функции вызываются.
    public bool unavailable=false;//Если действие уже активированно или на нём висит кулдаун, то его нельзя активировать. Для этого нужна эта переменная
    public float actionTime;//Сколько времени длится действие
    public float cooldown;//Сколько времени длится кулдаун 
    public int employment;//стоимость совершения данной деятельности.

    /// <summary>
    /// Метод, что проверяет, действительно ли условие деятельности выполняется
    /// </summary>
    public bool CheckCondition()
    {
        ConditionSign condSign;
        bool k = true;
        for (int i = 0; i < whyToDo.Count; i++)
        {
            condSign = whyToDo[i];
            if (condSign.aiCondition != null)
            {
                k = k && condSign.aiCondition(condSign.id, condSign.argument);
            }
            if (!k)
            {
                break;
            }
        }
        return k;
    }

    /// <summary>
    /// Метод, что вызывает все элементарные функции, составляющие деятельность
    /// </summary>
    public void DoAction()
    {
        ActionSign actSign;
        bool k = true;
        for (int i = 0; i < whatToDo.Count; i++)
        {
            actSign = whatToDo[i];
            if (actSign.aiAction != null)
            {
                actSign.aiAction(actSign.id, actSign.argument);
            }
        }
    }

    public AIActionData(AIActionData original)
    {
        actionName=original.actionName;
        whyToDo = original.whyToDo;
        posibility=original.posibility;
        whatToDo = original.whatToDo;
        unavailable = false;
        actionTime=original.actionTime;
        cooldown=original.cooldown; 
        employment=original.employment;
    }
}

/// <summary>
/// Класс, что представляет собой ссылку на вызываемую функцию и аргументы этой функции 
/// </summary>
[System.Serializable]
public class ActionSign
{
    public string actionName;//Имя вызываемой функции
    public delegate void AIAction(string id, int argument);
    public AIAction aiAction;//ссылка на функцию
    public string id;//Строковый аргумент
    public int argument;//Численный аргумент
}

/// <summary>
/// Класс, что представляет собой все данные, необходимые для вызова функции условия 
/// </summary>
[System.Serializable]
public class ConditionSign
{
    public string conditionString;
    public delegate bool AICondition(string id, int argument);
    public AICondition aiCondition;//Ссылка на функцию
    public string id;//Строковый аргумент
    public int argument;//Численный аргумент
}

/// <summary>
/// Класс, что используется при загрузке моделей поведения в персонажа с ИИ
/// </summary>
[System.Serializable]
public class SBehaviourClass
{
    public string path;
    public BehaviourClass behaviour;
}