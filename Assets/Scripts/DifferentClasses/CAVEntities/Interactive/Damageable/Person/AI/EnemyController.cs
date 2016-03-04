using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Шаблонный контроллер, который я создаю для того, чтобы определит, как вообще искусственный интеллект будет работать
/// Он может осуществлять некоторую деятельность в спокойствии, следовать за своей целью и атаковать её
/// </summary>
public class EnemyController : PersonController
{
    #region consts
    const float sightRadius = 30f;
    const float hearingRadius = 10f;
    protected const int maxEmployment = 10;//Персонажи могут совершать действия общей стоймостью в 10 ед.
    #endregion //consts

    #region fields
    public int k1 = 0;
    public GameObject target;//Объект, что привлёк внимание ИИ
    public float targetDistance;//Расстояние до цели
    public List<string> enemies;//Какие типы игровых объектов этот ИИ считает за врагов (пока что я отслеживаю враг ли это по тегу.)

    //public behaviourEnum behaviour;//Какую модель поведения применяет ИИ в данный момент (Calm,Agressive) 
    private Transform sight;//точка, что являутся глазами этого персонажа

    private EnemyActions actions;
    private EnemyVisual anim;
    public List<BehaviourClass> behaviours = new List<BehaviourClass>();//Какие модели поведения используются ИИ
    public BehaviourClass currentBehaviour;
    protected List<AIConditionID> conditionBase = new List<AIConditionID>();//База данных, которая по строке возвращает функцию проверки условия, что прописана в контроллере
    protected List<AIActionID> actionBase = new List<AIActionID>();//База данных, которая по строке возвращает элементарную функцию действия
    #endregion //fields

    #region parametres
    protected int employment = maxEmployment;
    #endregion //parametres

    public override void Awake()
    {
        base.Awake();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (currentBehaviour != null)
        {
            ImplementBehaviour(currentBehaviour);
        }
        AnalyzeSituation();
    }

    /// <summary>
    /// Сформировать список моделей поведения
    /// </summary>
    public virtual void FormBehaviourList()
    {
        conditionBase = new List<AIConditionID>();
        conditionBase.Add(new AIConditionID("distance to target", CheckTargetDistance));
        conditionBase.Add(new AIConditionID("target exists?", CheckTarget));

        actionBase = new List<AIActionID>();
        actionBase.Add(new AIActionID("change behaviour", ChangeBehaviour));
        actionBase.Add(new AIActionID("perception", Perception));
        actionBase.Add(new AIActionID("pursue", Pursue));
        
        for (int i=0;i<behaviours.Count;i++)
        {
            behaviours[i] = new BehaviourClass(behaviours[i]);
        }

        for (int i = 0; i < behaviours.Count; i++)
        {
            for (int j=0; j < behaviours[i].activities.Count; j++)
            {
                for (int k = 0; k < behaviours[i].activities[j].whyToDo.Count; k++)
                {
                    for (int l = 0; l < conditionBase.Count; l++)
                    {
                        if (string.Equals(conditionBase[l].conditionName, behaviours[i].activities[j].whyToDo[k].conditionString))
                        {
                            behaviours[i].activities[j].whyToDo[k].aiCondition = conditionBase[l].aiCondition.Invoke;
                        }
                    }
                }
                for (int k = 0; k < behaviours[i].activities[j].whatToDo.Count; k++)
                {
                    for (int l = 0; l < actionBase.Count; l++)
                    {
                        if (string.Equals(actionBase[l].actionName, behaviours[i].activities[j].whatToDo[k].actionName))
                        {
                            behaviours[i].activities[j].whatToDo[k].aiAction = actionBase[l].aiAction.Invoke;
                        }
                    }
                }
            }
        }
    }

    public override void Initialize()
    {
        groundCheck = transform.FindChild("Indicators").FindChild("GroundCheck");
        sight = transform.FindChild("Indicators").FindChild("Sight");
        FormBehaviourList();
        if (behaviours.Count > 0)
        {
            currentBehaviour = behaviours[0];
        }
        if (stats == null)
        {
            stats = new Stats();
        }
        target = null;
        actions = GetComponent<EnemyActions>();
        if (actions != null)
        {
            actions.SetStats(stats);
        }
        anim = GetComponentInChildren<EnemyVisual>();
        if (anim != null)
        {
            anim.SetStats(stats);
        }
        GetComponentInChildren<CharacterAnimator>().SetStats(stats);
        employment = maxEmployment;
    }

    /// <summary>
    /// Реализовать данную модель поведения
    /// </summary>
    public virtual void ImplementBehaviour(BehaviourClass _behaviour)
    {
        List<AIActionData> canIEmployIt = new List<AIActionData>();
        List<AIActionData> whatToEmploy = new List<AIActionData>();
        while (employment > 0)
        {
            int posibilitySum = 0;
            foreach (AIActionData activity in currentBehaviour.activities)
            {
                if ((!activity.unavailable)&&(!whatToEmploy.Contains(activity)))
                {
                    if ((activity.CheckCondition())&&(activity.employment<=employment))
                    {
                        canIEmployIt.Add(activity);
                        posibilitySum += activity.posibility;
                    }
                }
            }
            if (canIEmployIt.Count == 0)
            {
                break;
            }
            else
            {
                int j = 0;
                int r = Random.Range(1, posibilitySum);
                while (r > canIEmployIt[j].posibility)
                {
                    j++;
                    r -= canIEmployIt[j].posibility;
                }
                whatToEmploy.Add(canIEmployIt[j]);
                employment -= whatToEmploy[j].employment;
                canIEmployIt.Clear();
            }
        }
        for (int i = 0; i < whatToEmploy.Count; i++)
        {
            StartCoroutine(ImplementActivity(whatToEmploy[i]));
        }
    }

    #region interface

    /// <summary>
    /// Сменить модель поведения
    /// </summary>
    public virtual void ChangeBehaviour(string id, int argument)
    {
        for (int i = 0; i < behaviours.Count; i++)
        {
            if (string.Equals(id, behaviours[i].behaviourName))
            {
                currentBehaviour = behaviours[i];
                break;
            }
        }
    }

    /// <summary>
    /// Функция, что отвечает за нахождение цели
    /// </summary> 
    public virtual void Perception(string id, int argument)
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(new Ray(sight.position, (int)stats.direction * sight.right), out hit, sightRadius))
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (string.Equals(hit.collider.gameObject.tag, enemies[i]))
                {
                    target = hit.collider.gameObject;
                    actions.target = target.transform;
                    break;
                }
            }           
        }
    }

    /// <summary>
    /// Функция совершения любого вида атаки на противнике
    /// </summary>
    public virtual void Attack (string id, int argument)
    {

    }
    
    /// <summary>
    /// Функция пресследования цели
    /// </summary>
    public virtual void Pursue(string id, int argument)
    {
        if (target != null)
        {
            actions.Pursue();
        }
    }

    /// <summary>
    /// Реализовать данную деятельность
    /// </summary>
    protected virtual IEnumerator ImplementActivity(AIActionData activity)
    {
        activity.unavailable = true;
        activity.DoAction();
        yield return new WaitForSeconds(activity.actionTime);
        employment += activity.employment;
        yield return new WaitForSeconds(activity.cooldown);
        activity.unavailable = false;
    }

    #endregion //interface

    #region condition

    /// <summary>
    /// Проверка на наличие у данного персонажа активной цели
    /// </summary>
    public virtual bool CheckTarget(string id, int argument)
    {
        if (string.Equals(id, "no"))
        {
            return (target == null);
        }
        else if (string.Equals(id, "yes"))
        {
            return (target != null);
        }
        else return false;
    }

    /// <summary>
    /// Оценка расстояния до активной цели
    /// </summary>
    public virtual bool CheckTargetDistance(string id, int argument)
    {
        float targetDistance;
        if (target != null)
        {
            targetDistance = Vector3.Distance(transform.position, target.transform.position);
            return SpFunctions.ComprFunctionality(targetDistance, id,argument * 1f);
        }
        else return false;
    }

    #endregion //condition
}

/// <summary>
/// Редактор контроллера искусственного интеллекта
/// </summary>
[CustomEditor(typeof(EnemyController))]
public class EnemyEditor : Editor
{
    private Stats stats;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EnemyController obj = (EnemyController)target;
        stats = (Stats)obj.GetStats();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.IntField("direction", (int)stats.direction);
        stats.maxHealth = EditorGUILayout.FloatField("Max Health", stats.maxHealth);
        EditorGUILayout.FloatField("Health", stats.health);
    }
}
