using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// таблица, в которой находятся все совершаемые ИИ действия, причины и вероятность  их совершения
/// </summary>
[System.Serializable]
public class BehaviourClass : ScriptableObject
{
    public string behaviourName;
    public List<AIActionData> activities=new List<AIActionData>();

    public BehaviourClass(BehaviourClass original)
    {
        name = original.name;
        behaviourName = original.behaviourName;
        activities = new List<AIActionData>();
        for (int i = 0; i < original.activities.Count; i++)
        {
            activities.Add(new AIActionData(original.activities[i]));
        }
    }
}

/// <summary>
/// Класс, в котором содержится вся информация о совершаемом ИИ одном действии
/// </summary>
[System.Serializable]
public class AIActionData
{
    public string actionName;//Название действия
    public List<AIConditionSign> whyToDo=new List<AIConditionSign>();//Условия совершения действия
    public int posibility;// Вероятность, что действие будет совершено - это обычное число, что используется для определения, какое именно действие из возможных будет совершено
    public List<AIActionSign> whatToDo = new List<AIActionSign>();//Какие элементарные функции вызываются.
    public bool unavailable=false;//Если действие уже активированно или на нём висит кулдаун, то его нельзя активировать. Для этого нужна эта переменная
    public float actionTime;//Сколько времени длится действие
    public float cooldown;//Сколько времени длится кулдаун 
    public int employment;//стоимость совершения данной деятельности.

    /// <summary>
    /// Метод, что проверяет, действительно ли условие деятельности выполняется
    /// </summary>
    public bool CheckCondition()
    {
        AIConditionSign condSign;
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
        AIActionSign actSign;
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
public class AIActionSign
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
public class AIConditionSign
{
    public string conditionString;
    public delegate bool AICondition(string id, int argument);
    public AICondition aiCondition;//Ссылка на функцию
    public string id;//Строковый аргумент
    public int argument;//Численный аргумент
}
