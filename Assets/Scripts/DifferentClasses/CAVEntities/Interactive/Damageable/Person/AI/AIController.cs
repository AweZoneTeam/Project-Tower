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
public class AIController : PersonController, IPersonWatching
{
    #region consts

    protected const float sightRadius = 60f;
    protected const float hearingRadius = 20f;

    protected const float g = 100f;

    #endregion //consts

    #region dictionaries

    protected Dictionary<string, jDataActionDelegate> jActionBase = new Dictionary<string, jDataActionDelegate> ();

    public virtual Dictionary<string, jDataActionDelegate> GetJActionBase()
    {
        return jActionBase;
    }

    #endregion //dictionaries

    #region delegates

    public delegate void AIAction(string id, int argument);
    public delegate bool AICondition(string id, int argument);
    public delegate void jDataActionDelegate(JournalEventAction _action);//Делегат функции, что вызываются скриптовыми событиями

    #endregion //delegates

    #region eventHandlers

    public EventHandler<JournalEventArgs> EnemyDieJournalEvent;

    #endregion //eventHandlers

    #region fields

    public int k1 = 0;

    protected HearingScript hearing;//Слух персонажа

    protected Vector3 startPosition;//Начальная позиция персонажа
    protected AreaClass startRoom;//Начальная комната пероснажа
    protected TargetWithCondition mainTarget;//Объект, что является истинной целью ИИ. Выполнение действия с данной целью является корнем поведения ИИ
    protected List<TargetWithCondition> waypoints=new List<TargetWithCondition>();//Очередь из последовательности объектов, что являются точками интереса ИИ. ИИ последовательно выполняет действия с этими объектами.                      
                                                                                 //Таким образом я хочу сделать что-то вроде памяти ИИ. Ну и ещё это часть вспомогательного механизма, двигающего ИИ
    protected TargetWithCondition currentTarget;//Чем интересуется ИИ в данный момент
    protected TargetWithCondition whoAttacksMe;//Кто атаковал персонажа
    public TargetWithCondition WhoAttacksMe {set { whoAttacksMe = value; } }
    protected float targetDistance;//Расстояние до текущей цели
    public List<string> enemies;//Какие типы игровых объектов этот ИИ считает за врагов (пока что я отслеживаю враг ли это по тегу.)

    //public behaviourEnum behaviour;//Какую модель поведения применяет ИИ в данный момент (Calm,Agressive) 

    public List<SBehaviourClass> behaviours = new List<SBehaviourClass>();//Какие модели поведения используются ИИ
    public BehaviourClass currentBehaviour;
    protected Dictionary<string, AICondition> conditionBase = new Dictionary<string, AICondition>();//База данных, которая по строке возвращает функцию проверки условия, что прописана в контроллере
    protected Dictionary<string,AIAction> actionBase = new Dictionary<string, AIAction>();//База данных, которая по строке возвращает элементарную функцию действия

    #endregion //fields

    #region parametres

    protected float height = 15f;
    [SerializeField]protected LayerMask whatToSight;

    #endregion //parametres

    public override void Awake()
    {
        base.Awake();
    }

    public virtual void FixedUpdate()
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
            if (currentBehaviour != null)
            {
                ImplementBehaviour(currentBehaviour);
            }
            AnalyzeSituation();
        }
    }

    /// <summary>
    /// Сформировать словари действий и проверок
    /// </summary>
    protected virtual void FormActionDictionaries()
    {
        conditionBase = new Dictionary<string, AICondition>();
        conditionBase.Add("distance to target", CheckTargetDistance);
        conditionBase.Add("distance x to target", CheckTargetX);
        conditionBase.Add("distance y to target", CheckTargetY);
        conditionBase.Add("distance abs x to target", CheckTargetAbsX);
        conditionBase.Add("distance abs y to target", CheckTargetAbsY);
        conditionBase.Add("target room",CheckTargetRoomPosition);
        conditionBase.Add("target exists?", CheckTarget);
        conditionBase.Add("main target exists?", CheckMainTarget);
        conditionBase.Add("waypoints exist?", CheckWaypoints);
        conditionBase.Add("attacker exists?", CheckAttacker);
        conditionBase.Add("main target is current", MainTargetIsCurrent);
        conditionBase.Add("target on sight?", CheckTargetIsOnSight);
        conditionBase.Add("target type", CheckTargetType);
        conditionBase.Add("main target type", CheckMainTargetType);
        conditionBase.Add("health", CheckHeatlh);

        actionBase = new Dictionary<string, AIAction>();
        actionBase.Add("change behaviour", ChangeBehaviour);
        actionBase.Add("watch the target", WatchTheTarget);
        actionBase.Add("choose waypoint", ConcentrateOnWaypoint);
        actionBase.Add("clear waypoints", ClearWaypoints);
        actionBase.Add("choose attacker", ConcentrateOnAttacker);
        actionBase.Add("choose main target", ConcentrateOnMainTarget);
        actionBase.Add("use door", DoorInteraction);
        actionBase.Add("use enter", EnterInteraction);
        actionBase.Add("use waypoint", WaypointInteraction);
        actionBase.Add("use target", UnknownWaypointInteraction);
        actionBase.Add("lose target", LoseTarget);
        actionBase.Add("perception", Perception);
        actionBase.Add("pursue", Pursue);
        actionBase.Add("escape", Escape);
        actionBase.Add("stay", Stay);
        actionBase.Add("attack", Attack);
        actionBase.Add("say", SaySomething);
        actionBase.Add("tell about target", SayEveryOneAboutTarget);
        actionBase.Add("test", TestFunction);
        actionBase.Add("turn", Turn);
        actionBase.Add("turnMove", TurnMoveDirection);
        actionBase.Add("jump", Jump);
        actionBase.Add("go home", GoHome);

        jActionBase.Add("makeAnEnemy", MakeAnEnemy);
    }

    /// <summary>
    /// Сформировать список моделей поведения
    /// </summary>
    protected virtual void FormBehaviourList()
    {
        FormActionDictionaries();

        BehaviourClass _behaviour;
        for (int i=0;i<behaviours.Count;i++)
        {
            behaviours[i].behaviour = new BehaviourClass(behaviours[i].behaviour);
            _behaviour = behaviours[i].behaviour;
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
        hearing = GetComponentInChildren<HearingScript>();
        if (hearing!=null)
        {
            hearing.HearingRadius = hearingRadius;
            hearing.Initialize();
        }
        if (pActions != null)
        {
            pActions.SetSpeeds(orgStats.velocity, orgStats.defVelocity);
        }
        FormBehaviourList();
        if (behaviours.Count > 0)
        {
            currentBehaviour = behaviours[0].behaviour;
        }
        currentTarget = null;
        mainTarget = null;
        waypoints = new List<TargetWithCondition>();
        GetComponentInChildren<CharacterAnimator>().SetStats(envStats);
        employment = maxEmployment;
        startPosition = transform.position;
        startRoom = currentRoom;
        if (hitBox != null)
        {
            hitBox.SetEnemies(enemies);
        }
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
        waypoints = new List<TargetWithCondition>();
        waypoints.Add(new TargetWithCondition(currentRoom.GetDoor(e.Room), "door"));
    }

    /// <summary>
    /// Реализовать данную модель поведения
    /// </summary>
    protected virtual void ImplementBehaviour(BehaviourClass _behaviour)
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

    #region actions

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
        //Переосмотреть цели
        whoAttacksMe = null;
        currentTarget = null;
        waypoints = null;
    }

    /// <summary>
    /// Сделать активной целью один из вэйпоинтов
    /// </summary>
    protected virtual void ConcentrateOnWaypoint(string id, int argument)
    {
        if (waypoints!=null?waypoints.Count>0:false)
        {
            if (string.Equals(id, "next"))
            {
                int index = 0;
                if (currentTarget != null)
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
                TargetWithCondition prevWaypoint = currentTarget;
                currentTarget = waypoints[index];
                if (currentTarget == prevWaypoint)
                {
                    currentTarget = null;
                }
                if (waypoints.Contains(prevWaypoint))
                {
                    waypoints.Remove(prevWaypoint);
                }
            }
            else if (waypoints.Count > argument)
            {
                currentTarget = waypoints[argument];
            }
            else
            {
                currentTarget = null;
            }
            if (currentTarget != null)
            {
                pActions.target = currentTarget.target.transform;
            }
        }
    }

    /// <summary>
    /// Сконцентрироваться на атакующем
    /// </summary>
    protected virtual void ConcentrateOnAttacker(string id, int argument)
    {
        if (whoAttacksMe != null?(whoAttacksMe.target!=null):false)
        {
            currentTarget = whoAttacksMe;
            mainTarget = whoAttacksMe;
            if (pActions != null)
            {
                pActions.target = currentTarget.target.transform;
            }
        }
    }

    /// <summary>
    /// Сконцентрироваться на главной цели
    /// </summary>
    protected virtual void ConcentrateOnMainTarget(string id, int argument)
    {
        if (mainTarget != null ? (mainTarget.target != null) : false)
        {
            currentTarget = mainTarget;
            if (pActions!=null)
            {
                pActions.target = currentTarget.target.transform;
            }
        }
    }

    /// <summary>
    /// Сбросить список вэйпоинтов
    /// </summary>
    protected virtual void ClearWaypoints(string id, int argument)
    {
        waypoints.Clear();
    }

    /// <summary>
    /// Игнорировать текущую цель
    /// </summary>
    protected virtual void LoseTarget(string id, int argument)
    {
        currentTarget = null;
    }

    /// <summary>
    /// Возвращение в начальную позицию
    /// </summary>
    protected virtual void GoHome(string id, int argument)
    {
        GameObject startPoint = new GameObject("StartPoint");
        startPoint.transform.position = startPosition;
        mainTarget = new TargetWithCondition(startPoint, "waypoint");
        waypoints = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Map>().GetWay(transform.position,currentRoom,mainTarget,startRoom);
    }

    /// <summary>
    /// Функция, что отвечает за нахождение цели
    /// </summary> 
    protected virtual void Perception(string id, int argument)
    {
        RaycastHit hit = new RaycastHit();
        if (sight != null)
        {
            if (Physics.Raycast(new Ray(sight.position, (int)direction.dir * sight.right), out hit, sightRadius, whatToSight))
            {
                if (enemies.Contains(hit.collider.gameObject.tag))
                {
                    currentTarget = new TargetWithCondition(hit.collider.gameObject, "enemy");
                    mainTarget = currentTarget;
                    pActions.target = currentTarget.target.transform;
                }
            }
        }
        if (hearing != null)
        {
            if (hearing.WhoIHear.Count > 0)
            {
                foreach(PersonController person in hearing.WhoIHear)
                {
                    if (enemies.Contains(person.gameObject.tag))
                    {
                        currentTarget = new TargetWithCondition(person.gameObject, "enemy");
                        mainTarget = currentTarget;
                        pActions.target = currentTarget.target.transform;
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Начать пристально наблюдать за противником
    /// </summary>
    protected virtual void WatchTheTarget(string id, int argument)
    {
        PersonController person=null;
        if (currentTarget != null? (currentTarget.target!=null):false)
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
    protected virtual void Attack (string id, int argument)
    {
        k1++;
        pActions.SetHitData(id);
        pActions.Attack();
    }
    
    /// <summary>
    /// Функция пресследования цели
    /// </summary>
    protected virtual void Pursue(string id, int argument)
    {
        if (currentTarget!=null?(currentTarget.target!=null):false)
        {
            if (pActions != null)
            {
                pActions.Pursue();
            }
        }
    }

    /// <summary>
    /// Функция избегания цели
    /// </summary>
    protected virtual void Escape(string id, int argument)
    {
        if (currentTarget != null ? (currentTarget.target != null) : false)
        {
            if (pActions != null)
            {
                pActions.Escape();
            }
        }
    }

    /// <summary>
    /// Функция стояния на месте
    /// </summary>
    protected virtual void Stay(string id, int argument)
    {
        if (pActions != null)
        {
            pActions.StopWalking();
        }
    }

    /// <summary>
    /// Повернуться на противоположную сторону
    /// </summary>
    protected virtual void Turn(string id, int argument)
    {
        if (pActions!=null)
        {
            if (argument == 1)
            {
                pActions.Turn((orientationEnum)1);
            }
            else if (argument == -1)
            {
                pActions.Turn((orientationEnum)(-1));
            }
            else
            {
                pActions.Turn((orientationEnum)(-1 * (int)direction.dir));
            }
        }
    }

    /// <summary>
    /// Поменять направление движения на противоположное
    /// </summary>
    protected virtual void TurnMoveDirection(string id, int argument)
    {
        if (pActions != null)
        {
            pActions.movingDirection = (orientationEnum)(-1 * (int)pActions.movingDirection);
        }
    }

    /// <summary>
    /// Совершить прыжок
    /// </summary>
    protected virtual void Jump(string id, int argument)
    {
        if (pActions!=null)
        {
            pActions.Jump();
        }
    }

    /// <summary>
    /// Функция открывания двери
    /// </summary>
    protected virtual void DoorInteraction(string id, int argument)
    {
        Transform trans = gameObject.transform;
        DoorClass door=currentTarget.target.GetComponent<DoorClass>();
        if (door!=null)
        {
            if (door.locker.opened)
            {
                ChangeRoom(door.roomPath);
                pActions.GoThroughTheDoor(door);
            }
        }
        if (waypoints!=null?waypoints.Contains(currentTarget):false)
        {
            ConcentrateOnWaypoint("next", 0);
        }
        base.DoorInteraction();
    }

    /// <summary>
    /// Как персонаж ведёт себя с вэйпоинтом обыкновенным
    /// </summary>
    protected virtual void WaypointInteraction(string id, int argument)
    {
        if (currentTarget != null ? (currentTarget.target != null) : false)
        {
            Destroy(currentTarget.target);
            ConcentrateOnWaypoint("next", 0);
        }
    }

    /// <summary>
    /// Общая функция, объединяющая в себе все типы взаимодействий с вэйпоинтами
    /// </summary>
    protected virtual void UnknownWaypointInteraction(string id, int argument)
    {
        if (currentTarget != null ? (currentTarget.target != null) : false)
        {
            if (string.Equals(currentTarget.targetType, "door"))
            {
                DoorInteraction("", 0);
            }
            else if (string.Equals(currentTarget.targetType, "enter"))
            {
                EnterInteraction("", 0);
            }
            else if (string.Equals(currentTarget.targetType, "waypoint"))
            {
                WaypointInteraction("", 0);
            }
        }
    }

    /// <summary>
    /// Как персонаж ведёт себя с объектами класса EnterClass
    /// </summary>
    protected virtual void EnterInteraction(string id, int argument)
    {
        if (currentTarget != null ? (currentTarget.target != null) : false)
        {
            EnterClass enter;
            if ((enter = currentTarget.target.GetComponent<EnterClass>()) != null)
            {
                ChangeRoom(enter.nextRoom);
                ConcentrateOnWaypoint("next", 0);
            }
        }
    }

    /// <summary>
    /// Сказать что-то для того, чтобы все услышали.
    /// </summary>
    protected virtual void SaySomething(string id, int argument)
    {
        SpFunctions.SendMessage(new MessageSentEventArgs(id, argument, id.Length * 0.04f));   
    }

    /// <summary>
    /// Функция для детектирования срабатывания тех или иных моделей поведения
    /// </summary>
    protected virtual void TestFunction(string id, int argument)
    {
        k1++;
    }

    #endregion //actions

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
        pActions.Hitted();
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
            pActions.Death();
            OnEnemyDie(new JournalEventArgs());
        }
    }

    #endregion //interface

    #region conditions

    /// <summary>
    /// Проверка на наличие у данного персонажа активной цели
    /// </summary>
    protected virtual bool CheckTarget(string id, int argument)
    {
        if (string.Equals(id, "no"))
        {
            return (currentTarget == null? true: (currentTarget.target == null));
        }
        else if (string.Equals(id, "yes"))
        {
            return (currentTarget != null ? (currentTarget.target != null) : false);
        }
        else return false;
    }

    /// <summary>
    /// Проверка на наличие у данного персонажа каких-либо вэйпоинтов
    /// </summary>
    protected virtual bool CheckWaypoints(string id, int argument)
    {
        if (string.Equals(id, "no"))
        {
            return (waypoints==null?true: (waypoints.Count == 0));
        }
        else if (string.Equals(id, "yes"))
        {
            return (waypoints!=null?(waypoints.Count>0):false);
        }
        else return false;
    }

    /// <summary>
    /// Проверка на наличие у данного персонажа главной цели
    /// </summary>
    protected virtual bool CheckMainTarget(string id, int argument)
    {
        if (string.Equals(id, "no"))
        {
            return (mainTarget == null?true: (mainTarget.target == null));
        }
        else if (string.Equals(id, "yes"))
        {
            return (mainTarget != null?(mainTarget.target!=null):false);
        }
        else return false;
    }

    /// <summary>
    /// Проверка на наличие того, кто атаковал персонажа в последний момент
    /// </summary>
    protected virtual bool CheckAttacker(string id, int argument)
    {
        if (string.Equals(id, "no"))
        {
            return (whoAttacksMe == null?true: (whoAttacksMe.target == null));
        }
        else if (string.Equals(id, "yes"))
        {
            return (whoAttacksMe != null?(whoAttacksMe.target!=null):false);
        }
        else return false;
    }

    /// <summary>
    /// Все незанятые персонажи одного типа в той же комнате узнают о новой цели
    /// </summary>
    protected virtual void SayEveryOneAboutTarget(string id, int argument)
    {
        foreach (PersonController person in currentRoom.container)
        {
            if (string.Equals(person.tag, gameObject.tag))
            {
                AIController ai = (AIController)person;
                if (ai.currentTarget == null? true: (currentTarget.target == null))
                {
                    ai.mainTarget=mainTarget;
                    ai.currentTarget = mainTarget;
                }
            }
        }
    }

    /// <summary>
    /// Проверить, действительно ли главная цель персонажа является главной
    /// </summary
    protected virtual bool MainTargetIsCurrent(string id, int argument)
    {
        if (string.Equals(id, "no"))
        {
            return (mainTarget != currentTarget);
        }
        else if (string.Equals(id, "yes"))
        {
            return (mainTarget == currentTarget);
        }
        else return false;
    }

    /// <summary>
    /// Оценка расстояния до активной цели
    /// </summary>
    protected virtual bool CheckTargetDistance(string id, int argument)
    {
        float targetDistance;
        if (currentTarget != null ? (currentTarget.target != null) : false)
        {
            targetDistance = Vector3.Distance(transform.position, currentTarget.target.transform.position);
            return SpFunctions.ComprFunctionality(targetDistance, id,argument * 1f);
        }
        else return false;
    }

    /// <summary>
    /// Оценить относительную x-координату цели
    /// </summary>
    protected virtual bool CheckTargetX(string id, int argument)
    {
        if (currentTarget != null ? (currentTarget.target != null) : false)
        {
            return SpFunctions.ComprFunctionality((currentTarget.target.transform.position - transform.position).x, id, argument);
        }
        else return false;
    }

    /// <summary>
    /// Оценить расстояние по оси x до цели
    /// </summary>
    protected virtual bool CheckTargetAbsX(string id, int argument)
    {
        if (currentTarget != null ? (currentTarget.target != null) : false)
        {
            return SpFunctions.ComprFunctionality(Mathf.Abs((currentTarget.target.transform.position - transform.position).x), id, argument);
        }
        else return false;
    }

    /// <summary>
    /// Оценить относительную y-координату цели
    /// </summary>
    protected virtual bool CheckTargetY(string id, int argument)
    {
        if (currentTarget != null ? (currentTarget.target != null) : false)
        {
            return SpFunctions.ComprFunctionality((currentTarget.target.transform.position - transform.position).y, id, argument);
        }
        else return false;
    }

    /// <summary>
    /// Оценить расстояние по оси y до цели
    /// </summary>
    protected virtual bool CheckTargetAbsY(string id, int argument)
    {
        if (currentTarget != null ? (currentTarget.target != null) : false)
        {
            return SpFunctions.ComprFunctionality(Mathf.Abs((currentTarget.target.transform.position - transform.position).y), id, argument);
        }
        else return false;
    }

    /// <summary>
    /// Проверка на нахождение текущей цели в поле зрения
    /// </summary>
    protected virtual bool CheckTargetIsOnSight (string id, int argument)
    {
        if (currentTarget != null ? (currentTarget.target != null) : false)
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(new Ray(sight.position, (int)direction.dir * sight.right), out hit, sightRadius))
            {
                if (hit.transform.gameObject==currentTarget.target)
                {
                    if (string.Equals(id, "yes"))
                    {
                        return true;
                    }
                    else return false;
                }
                else if (string.Equals(id, "no"))
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
    protected virtual bool CheckTargetType(string id, int argument)
    {
        if (currentTarget != null ? (currentTarget.target != null) : false)
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

    /// <summary>
    /// Проверить тип главной цели персонажа
    /// </summary>
    protected virtual bool CheckMainTargetType(string id, int argument)
    {
        if (mainTarget != null?(mainTarget.target!=null):false)
        {
            if (argument == -1)
            {
                return (!string.Equals(id, mainTarget.targetType));
            }
            else
            {
                return (string.Equals(id, mainTarget.targetType));
            }
        }
        if (argument == -1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Проверить, сколько здововья осталось у персонажа
    /// </summary>
    protected virtual bool CheckHeatlh(string id, int argument)
    {
        return SpFunctions.ComprFunctionality(orgStats.health, id, argument * 1f);
    }

    /// <summary>
    /// Узнать, находится ли нынешняя цель в той же комнате, что и персонаж
    /// </summary>
    protected virtual bool CheckTargetRoomPosition(string id, int argument)
    {
        PersonController person;
        if ((person = currentTarget.target.GetComponent<PersonController>()) != null)
        {
            if (string.Equals(id, "same"))
            {
                return (currentRoom == person.currentRoom);
            }
            else if (string.Equals(id, "other"))
            {
                return (currentRoom != person.currentRoom);
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    #endregion //condition

    #region scriptActions

    /// <summary>
    /// Добавить в свой список врагов нового
    /// </summary>
    protected virtual void MakeAnEnemy(JournalEventAction _action)
    {
        string newEnemy = _action.id;
        if (!enemies.Contains(newEnemy))
        {
            enemies.Add(newEnemy);
        }
    }

    #endregion //scriptActions

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
        if (pActions.PrecipiceIsForward = (!(Physics.OverlapSphere(precipiceCheck.position, precipiceRadius, whatIsGround).Length > 0) && (envStats.groundness == groundnessEnum.grounded)))
        {
            pActions.JumpIsPossible = DefineJumpPosibility(pActions.RunSpeed * 1f, pActions.jumpForce * 1f, orgStats.health);
        }
    }

    /// <summary>
    /// Определить, можно ли прыгать
    /// Персонаж учитывает свою скорость при прыжке и сколько здоровья он потеряет, если совершит прыжок
    /// </summary>
    protected virtual bool DefineJumpPosibility(float v0x, float v0y, float hp)
    {
        Vector3 precPos = precipiceCheck.position;
        int dir = (int)direction.dir;
        float hpCost = 0f;
        float delX = 1f;
        float x = precPos.x+dir*delX;
        float x1 = precPos.x + dir * delX;
        float vy = v0y;
        float y = 0f;
        Vector2 extremum = new Vector2(precPos.x + dir * v0x * v0y / g, precPos.y + v0y * v0y / g);
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
            x += dir * delX;
            if (!(Physics.OverlapSphere(new Vector3(x1, y + height, precPos.z), precipiceRadius, whatIsGround).Length > 0))
            {
                x1 += dir * delX;
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
[CustomEditor(typeof(AIController))]
public class EnemyEditor : Editor
{
    private Direction direction;
    private OrganismStats orgStats;
    private EnvironmentStats envStats;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AIController obj = (AIController)target;
        EditorGUILayout.ObjectField(obj.GetTarget(), typeof(GameObject), true);
        direction = obj.GetDirection();
        orgStats = obj.GetOrgStats();
        envStats = obj.GetEnvStats();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Parametres");
        EditorGUILayout.EnumMaskField("direction", direction.dir);
        //orgStats.maxHealth = EditorGUILayout.FloatField("Max Health", orgStats.maxHealth);
        //orgStats.velocity = EditorGUILayout.FloatField("Velocity", orgStats.velocity);
        //EditorGUILayout.FloatField("Health", orgStats.health);
    }
}
#endif