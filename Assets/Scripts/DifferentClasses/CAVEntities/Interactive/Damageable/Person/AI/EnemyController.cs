using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Шаблонный контроллер, который я создаю для того, чтобы определит, как вообще искусственный интеллект будет работать
/// Он может осуществлять некоторую деятельность в спокойствии, следовать за своей целью и атаковать её
/// </summary>
public class EnemyController : PersonController, IPersonWatching
{
    #region consts
    protected const float sightRadius = 60f;
    protected const float hearingRadius = 10f;

    protected const float g = 100f;
    
    #endregion //consts

    #region delegates

    public delegate void AIAction(string id, int argument);
    public delegate bool AICondition(string id, int argument);

    #endregion //delegates

    #region eventHandlers

    public EventHandler<JournalEventArgs> EnemyDieJournalEvent;

    #endregion //eventHandlers

    #region fields
    public int k1 = 0;
    private TargetWithCondition mainTarget;//Объект, что является истинной целью ИИ. Выполнение действия с данной целью является корнем поведения ИИ
    private List<TargetWithCondition> waypoints=new List<TargetWithCondition>();//Очередь из последовательности объектов, что являются точками интереса ИИ. ИИ последовательно выполняет действия с этими объектами.
                                                                              //Таким образом я хочу сделать что-то вроде памяти ИИ. Ну и ещё это часть вспомогательного механизма, двигающего ИИ
    private TargetWithCondition currentTarget;//Чем интересуется ИИ в данный момент
    private float targetDistance;//Расстояние до текущей цели
    public List<string> enemies;//Какие типы игровых объектов этот ИИ считает за врагов (пока что я отслеживаю враг ли это по тегу.)

    //public behaviourEnum behaviour;//Какую модель поведения применяет ИИ в данный момент (Calm,Agressive) 

    private EnemyVisual anim;
    public string behaviourPath;//Путь, в котором находятся модели поведения данного персонажа
    public List<SBehaviourClass> behaviours = new List<SBehaviourClass>();//Какие модели поведения используются ИИ
    public BehaviourClass currentBehaviour;
    protected Dictionary<string, AICondition> conditionBase = new Dictionary<string, AICondition>();//База данных, которая по строке возвращает функцию проверки условия, что прописана в контроллере
    protected Dictionary<string,AIAction> actionBase = new Dictionary<string, AIAction>();//База данных, которая по строке возвращает элементарную функцию действия
    #endregion //fields

    #region parametres

    protected float height = 15f;
    public LayerMask whatToSight;

    #endregion //parametres

    public override void Awake()
    {
        base.Awake();
    }

    public void FixedUpdate()
    {
        if ((stats.hitted > 0f) && (stats.health > 0f))
        {
            Hitted();
        }

        if (stats.health <= 0f)
        {
            Death();
        }

        if (!death)
        {
            if (currentBehaviour != null)
            {
                ImplementBehaviour(currentBehaviour);
            }
            AnalyzeSituation();
        }
    }

    /// <summary>
    /// Сформировать список моделей поведения
    /// </summary>
    public virtual void FormBehaviourList()
    {
        conditionBase = new Dictionary<string, AICondition>();
        conditionBase.Add("distance to target", CheckTargetDistance);
        conditionBase.Add("target exists?", CheckTarget);
        conditionBase.Add("main target exists?", CheckMainTarget);
        conditionBase.Add("waypoints exist?", CheckWaypoints);
        conditionBase.Add("target on sight?", CheckTargetIsOnSight);
        conditionBase.Add("target type", CheckTargetType);

        actionBase = new Dictionary<string, AIAction>();
        actionBase.Add("change behaviour", ChangeBehaviour);
        actionBase.Add("watch the target", WatchTheTarget);
        actionBase.Add("choose waypoint", ConcentrateOnWaypoint);
        actionBase.Add("use door", DoorInteraction);
        actionBase.Add("perception", Perception);
        actionBase.Add("pursue", Pursue);
        actionBase.Add("stay", Stay);
        actionBase.Add("attack", Attack);
        actionBase.Add("say", SaySomething);
        actionBase.Add("test", TestFunction);

        BehaviourClass _behaviour;
        for (int i=0;i<behaviours.Count;i++)
        {
            _behaviour = behaviours[i].behaviour;
            if (_behaviour == null)
            {
#if UNITY_EDITOR
                _behaviour = AssetDatabase.LoadAssetAtPath(behaviourPath + behaviours[i].path + ".asset", typeof(BehaviourClass)) as BehaviourClass;
#endif
            }
            behaviours[i].behaviour = new BehaviourClass(_behaviour);
        }

        string s;
        for (int i = 0; i < behaviours.Count; i++)
        {
            _behaviour = behaviours[i].behaviour;
            for (int j=0; j < _behaviour.activities.Count; j++)
            {
                for (int k = 0; k < _behaviour.activities[j].whyToDo.Count; k++)
                {
                    if (conditionBase.ContainsKey(_behaviour.activities[j].whyToDo[k].conditionString))
                    {
                        s = _behaviour.activities[j].whyToDo[k].conditionString;
                        _behaviour.activities[j].whyToDo[k].aiCondition = conditionBase[s].Invoke;
                    }
                }
                for (int k = 0; k < _behaviour.activities[j].whatToDo.Count; k++)
                {
                    if (actionBase.ContainsKey(_behaviour.activities[j].whatToDo[k].actionName))
                    {
                        s = _behaviour.activities[j].whatToDo[k].actionName;
                        _behaviour.activities[j].whatToDo[k].aiAction = actionBase[s].Invoke;
                    }
                }
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        sight = transform.FindChild("Indicators").FindChild("Sight");
        FormBehaviourList();
        if (behaviours.Count > 0)
        {
            currentBehaviour = behaviours[0].behaviour;
        }
        if (stats == null)
        {
            stats = new Stats();
        }
        rigid = GetComponent<Rigidbody>();
        currentTarget = null;
        mainTarget = null;
        waypoints = new List<TargetWithCondition>();
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
        buffList.OrgStats = stats;
    }

    /// <summary>
    /// Функция, что реализует тот факт, что какой-то персонаж стал наблюдать за действиями данного
    /// Организуется подписка на заданные события
    /// </summary>
    public override void WatchYou(PersonController person)
    {
        person.RoomChangedEvent += TargetChangeRoom;
    }

    /// <summary>
    /// Функция, что реализует тот факт, что какой-то персонаж перестал наблюдать за действиями данного
    /// </summary>
    public override void StopWatchingYou(PersonController person)
    {
        person.RoomChangedEvent -= TargetChangeRoom;
    }

    /// <summary>
    /// Если главная цель выйдет в соседнюю комнату, то ИИ это заметит и начнёт искать цель в следующей комнате
    /// </summary>
    public override void TargetMakeAnAction(string actionName)
    {
        if (string.Equals(actionName, "DoorInteraction"))
        {
            currentTarget = null;
            ChangeBehaviour("Search", 0);
            PersonController person= mainTarget.target.GetComponent<PersonController>();
            if (person != null)
            {
                waypoints.Add(new TargetWithCondition(currentRoom.GetDoor(person.GetRoomPosition()), "door"));
            }
        }
    }

    /// <summary>
    /// Что произойдёт, если цель уйдёт в другую комнату
    /// </summary>
    protected override void TargetChangeRoom(object sender, RoomChangedEventArgs e)
    {
        currentTarget = null;
        ChangeBehaviour("Search", 0);
        waypoints.Add(new TargetWithCondition(currentRoom.GetDoor(e.Room), "door"));
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
                    if (activity.CheckCondition())
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
                int r = UnityEngine.Random.Range(1, posibilitySum);
                while (r > canIEmployIt[j].posibility)
                {
                    j++;
                    r -= canIEmployIt[j].posibility;
                }
                whatToEmploy.Add(canIEmployIt[j]);
                employment -= canIEmployIt[j].employment;
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
        BehaviourClass _behaviour;
        for (int i = 0; i < behaviours.Count; i++)
        {
            _behaviour = behaviours[i].behaviour;
            if (string.Equals(id, _behaviour.behaviourName))
            {
                currentBehaviour = _behaviour;
                break;
            }
        }
    }

    /// <summary>
    /// Сделать активной целью один из вэйпоинтов
    /// </summary>
    public virtual void ConcentrateOnWaypoint(string id, int argument)
    {
        if (waypoints.Count>0)
        {
            if (string.Equals(id, "next"))
            {
                int index = 0;
                if (currentTarget!=null)
                {
                    if (waypoints.Contains(currentTarget))
                    {
                        index = waypoints.IndexOf(currentTarget);
                        index++;
                    }
                    if (index >= waypoints.Count)
                    {
                        index = 0;
                    }
                }
                currentTarget = waypoints[index];
            }
            else if (waypoints.Count > argument)
            {
                currentTarget = waypoints[argument];
            }
            if (currentTarget != null)
            {
                actions.target = currentTarget.target.transform;
            }
        }
    }

    /// <summary>
    /// Функция, что отвечает за нахождение цели
    /// </summary> 
    public virtual void Perception(string id, int argument)
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(new Ray(sight.position, (int)stats.direction * sight.right), out hit, sightRadius,whatToSight))
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (string.Equals(hit.collider.gameObject.tag, enemies[i]))
                {
                    currentTarget = new TargetWithCondition(hit.collider.gameObject,"enemy");
                    mainTarget = currentTarget;
                    actions.target = currentTarget.target.transform;
                    break;
                }
            }           
        }
    }

    /// <summary>
    /// Начать пристально наблюдать за противником
    /// </summary>
    public virtual void WatchTheTarget(string id, int argument)
    {
        PersonController person=null;
        if (currentTarget != null)
        {
            person = currentTarget.target.GetComponent<PersonController>();
            if (person != null)
            {
                if (string.Equals(id, "yes"))
                {
                    WatchYou(person);
                }
                else if (string.Equals(id, "no"))
                {
                    StopWatchingYou(person);
                } 
            }
        }
    }

    /// <summary>
    /// Функция совершения любого вида атаки
    /// </summary>
    public virtual void Attack (string id, int argument)
    {
        k1++;
        actions.SetHitData(id);
        actions.Attack();
    }
    
    /// <summary>
    /// Функция пресследования цели
    /// </summary>
    public virtual void Pursue(string id, int argument)
    {
        if (currentTarget!=null)
        {
            if (actions != null)
            {
                actions.Pursue();
            }
        }
    }

    /// <summary>
    /// Функция стояния на месте
    /// </summary>
    public virtual void Stay(string id, int argument)
    {
        if (actions != null)
        {
            actions.StopWalking();
        }
    }

    /// <summary>
    /// Функция открывания двери
    /// </summary>
    public virtual void DoorInteraction(string id, int argument)
    {
        Transform trans = gameObject.transform;
        DoorClass door=currentTarget.target.GetComponent<DoorClass>();
        if (door!=null)
        {
            if (door.locker.opened)
            {
                currentRoom = door.roomPath;
                actions.GoThroughTheDoor(door);
            }
        }
        if (waypoints.Contains(currentTarget))
        {
            waypoints.Remove(currentTarget);
        }
        currentTarget = null;
        base.DoorInteraction();
    }

    /// <summary>
    /// Сказать что-то для того, чтобы все услышали.
    /// </summary>
    public virtual void SaySomething(string id, int argument)
    {
        SpFunctions.SendMessage(new MessageSentEventArgs(id, argument, id.Length * 0.04f));   
    }

    /// <summary>
    /// Функция для детектирования срабатывания тех или иных моделей поведения
    /// </summary>
    public virtual void TestFunction(string id, int argument)
    {
        k1++;
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

    /// <summary>
    /// Эта функция вызывается при нанесении урона
    /// </summary>
    public override void Hitted()
    {
        actions.Hitted();
    }

    /// <summary>
    /// Эта функция вызывается при смерти персонажа
    /// </summary>
    public override void Death()
    {
        if (!death)
        {
            death = true;
            WatchTheTarget("no", 0);
            actions.Death();
            OnEnemyDie(new JournalEventArgs());
        }
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
            return (currentTarget == null);
        }
        else if (string.Equals(id, "yes"))
        {
            return (currentTarget != null);
        }
        else return false;
    }

    /// <summary>
    /// Проверка на наличие у данного персонажа каких-либо вэйпоинтов
    /// </summary>
    public virtual bool CheckWaypoints(string id, int argument)
    {
        if (string.Equals(id, "no"))
        {
            return (waypoints.Count == 0);
        }
        else if (string.Equals(id, "yes"))
        {
            return (waypoints.Count>0);
        }
        else return false;
    }

    /// <summary>
    /// Проверка на наличие у данного персонажа главной цели
    /// </summary>
    public virtual bool CheckMainTarget(string id, int argument)
    {
        if (string.Equals(id, "no"))
        {
            return (mainTarget == null);
        }
        else if (string.Equals(id, "yes"))
        {
            return (mainTarget != null);
        }
        else return false;
    }

    /// <summary>
    /// Оценка расстояния до активной цели
    /// </summary>
    public virtual bool CheckTargetDistance(string id, int argument)
    {
        float targetDistance;
        if (currentTarget != null)
        {
            targetDistance = Vector3.Distance(transform.position, currentTarget.target.transform.position);
            return SpFunctions.ComprFunctionality(targetDistance, id,argument * 1f);
        }
        else return false;
    }

    /// <summary>
    /// Проверка на нахождение текущей цели в поле зрения
    /// </summary>
    public virtual bool CheckTargetIsOnSight (string id, int argument)
    {
        if (currentTarget != null)
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(new Ray(sight.position, (int)stats.direction * sight.right), out hit, sightRadius))
            {
                if (hit.transform.gameObject==currentTarget.target)
                {
                    if (string.Equals(id, "yes"))
                    {
                        return true;
                    }
                    else return false;
                }
                if (string.Equals(id, "no"))
                {
                    return true;
                }
                return false;
            }
        }
        if (string.Equals(id, "no"))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Проверка на соответствие типа текущей цели заданному
    /// </summary>
    public virtual bool CheckTargetType(string id, int argument)
    {
        if (currentTarget != null)
        {
            if (argument == -1)
            {
                return (!string.Equals(id, currentTarget.targetType));
            }
            else
            {
                return (string.Equals(id, currentTarget.targetType));
            }
        }
        if (argument==-1)
        {
            return true;
        }
        return false;
    }

    #endregion //condition

    #region Analyze

    protected override void AnalyzeSituation()
    {
        base.AnalyzeSituation();
    }

    /// <summary>
    /// Оценить пропасть, что находится перед персонажем
    /// </summary>
    protected override void DefinePrecipice()
    {
        if (actions.PrecipiceIsForward = (!(Physics.OverlapSphere(precipiceCheck.position, precipiceRadius, whatIsGround).Length > 0) && (stats.groundness == groundnessEnum.grounded)))
        {
            actions.JumpIsPossible = DefineJumpPosibility(actions.maxSpeed * 1f, actions.jumpForce * 1f, stats.health);
        }
    }

    /// <summary>
    /// Определить, можно ли прыгать
    /// Персонаж учитывает свою скорость при прыжке и сколько здоровья он потеряет, если совершит прыжок
    /// </summary>
    protected virtual bool DefineJumpPosibility(float v0x, float v0y, float hp)
    {
        Vector3 precPos = precipiceCheck.position;
        float hpCost = 0f;
        float delX = 1f;
        float x = precPos.x+(int)stats.direction*delX;
        float x1 = precPos.x + (int)stats.direction * delX;
        float vy = v0y;
        float y = 0f;
        Vector2 extremum = new Vector2(precPos.x + (int)stats.direction * v0x * v0y / g, precPos.y + v0y * v0y / g);
        while (hpCost < hp)
        {
            y = -1 * g / v0x / v0x * (x - extremum.x)* (x - extremum.x) + extremum.y;
            vy = v0y - g * Mathf.Abs(x - precPos.x) / v0x;
            if (Physics.OverlapSphere(new Vector3(x1, y, precPos.z), precipiceRadius, whatIsGround).Length > 0)
            {
                if (!(Physics.OverlapSphere(new Vector3(x1, y + height, precPos.z), precipiceRadius, whatIsGround).Length > 0))
                {
                    return true;
                }
            }
            x += (int)stats.direction * delX;
            if (!(Physics.OverlapSphere(new Vector3(x1, y + height, precPos.z), precipiceRadius, whatIsGround).Length > 0))
            {
                x1 += (int)stats.direction * delX;
            }
            if (vy < -1f * minDmgFallSpeed)
            {
                hpCost = dmgPerSpeed * Mathf.Abs(rigid.velocity.y + minDmgFallSpeed);
            }
        }
        return false;
    }

    #endregion //Analyze

    #region getters

    public virtual GameObject GetTarget()
    {
        if (currentTarget != null)
        {
            return currentTarget.target;
        }
        else return null;
    }

    #endregion //getters

    #region events

    public void OnEnemyDie(JournalEventArgs e)
    {
        EventHandler<JournalEventArgs> handler = EnemyDieJournalEvent;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    #endregion //events

}

#if UNITY_EDITOR
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
        EditorGUILayout.ObjectField(obj.GetTarget(), typeof(GameObject), true);
        stats = (Stats)obj.GetStats();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.IntField("direction", (int)stats.direction);
        stats.maxHealth = EditorGUILayout.FloatField("Max Health", stats.maxHealth);
        EditorGUILayout.FloatField("Health", stats.health);
    }
}
#endif