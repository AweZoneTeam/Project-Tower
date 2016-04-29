using UnityEngine;
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
        whyToDo = new List<ConditionSign>();
        for (int i = 0; i < original.whyToDo.Count; i++)
        {
            whyToDo.Add(new ConditionSign(original.whyToDo[i]));
        }
        posibility=original.posibility;
        whatToDo = new List<ActionSign>();
        for (int i = 0; i < original.whatToDo.Count; i++)
        {
            whatToDo.Add(new ActionSign(original.whatToDo[i]));
        }
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

    public ActionSign(ActionSign original)
    {
        actionName = original.actionName;
        aiAction = null;
        id = original.id;
        argument = original.argument;
    }
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

    public ConditionSign(ConditionSign original)
    {
        conditionString = original.conditionString;
        aiCondition = null;
        id = original.id;
        argument = original.argument;
    }

}

/// <summary>
/// Класс, что представляет собой ссылку на вызываемую функцию и аргументы этой функции 
/// </summary>
[System.Serializable]
public class ReactionSign
{
    public string actionName;//Имя вызываемой функции
    public delegate void AIAction(string id, int argument);
    public AIAction aiAction;//ссылка на функцию
    public AIAction reAction;//Ссылка на контрфункцию
    public string id;//Строковый аргумент
    public int argument;//Численный аргумент

    public ReactionSign(ActionSign original)
    {
        actionName = original.actionName;
        aiAction = null;
        reAction = null;
        id = original.id;
        argument = original.argument;
    }
}

/// <summary>
/// Класс, что представяет собой данные, используемые при вызове функций, при которых происходит некоторое активное действие персонажа
/// </summary>
[System.Serializable]
public class ActionData
{
    public string actionName;//Имя вызываемой функции
    public string id;//Строковый аргумент
    public int argument;//Числовой аргумент
    public float actionTime = 1f;//Как долго совершается действие
    public int employment;//Насколько занятым станет персонаж при совершении этого действия?

}

/// <summary>
/// Класс, представляющий собой данные для вызова чарджевых действий
/// </summary>
[System.Serializable]
public class ChargeData : ActionData
{
    public float deadZone = 1f;//значение, при котором считается, что чардж ещё не начался
    public float maxValue = 2f;//Максимальное значение которого можно достичь при накапливании.
    public bool autoInteruption;//Если true, то автоматически прервётся чардж при достижении максимального значения.

    public ChargeData()
    {
        deadZone = 1f;
        maxValue = 2f;
        autoInteruption = false;
    }
}

/// <summary>
/// Класс, представляющий собой данные для вызовачарджевых действий предметов
/// </summary>
[System.Serializable]
public class ItemChargeData : ChargeData
{
    public ItemActionData chargedAction, unchargedAction;//Действия, что вызывается при завершённой и незавершённой заряддках предета соответственно
}

/// <summary>
/// Класс, что представляет собой аргументы функций, вызываемых при использовании предметов
/// </summary>
[System.Serializable]
public class ItemActionData: ActionData
{
    public GameObject itemObject;//Игровой объект, что ассоциируется с этим действием (бомба, что бросится при "Броске бомбы")
    public bool consumed=true;//Если true, то при использовании предмета, он будет расходоваться

    public ItemActionData()
    {
    }


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