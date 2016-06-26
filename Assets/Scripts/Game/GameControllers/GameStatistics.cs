using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Данные о ходе игры. Здесь учитывается время, количество попыток пройти, текущее местоположение главного героя. Да и вообще, процесс игры во многом учитывается и управляется здесь
/// </summary>
public class GameStatistics : MonoBehaviour
{

    #region consts

    const int defMonth = 7;
    const int defDay = 15;
    const int defHour = 10;
    const int defMin = 0;

    #endregion //consts

    #region eventHandlers

    public EventHandler<MessageSentEventArgs> MessageSentEvent;

    public EventHandler<JournalEventArgs> GameStatsJournalEvent;

    #endregion //eventHandlers

    #region fields

    #endregion //fields

    #region parametres

    public CheckpointActions lastCheckPoint;
    public static AreaClass currentArea; //В какой комнате (пространстве) персонаж находится на данный момент
    public static IlluminationController lumControl;//Контроллер освещения
    public int deathNumber; //Сколько раз уже главный герой проигрывал.

    public static bool paused = false;

    #endregion //parametres

    public void Start()
    {
        LoadGameData();
        Initialize();
        OnGameBegin(new JournalEventArgs());
    }

    public void Update()
    {
        GameTime.TimeFlow();
        if (Input.GetButtonDown("Cancel"))
        {
            SpFunctions.Pause("menu");
        }
        if (Input.GetButtonDown("Journal"))
        {
            SpFunctions.ChangeWindow("journal");
        }
        else if (Input.GetButtonDown("Equipment"))
        {
            SpFunctions.ChangeWindow("equipment");
        }
        else if (Input.GetButtonDown("Map"))
        {
            SpFunctions.ChangeWindow("map");
        }
    }

    void Initialize()
    {
        currentArea = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<KeyboardActorController>().currentRoom;
        lumControl = GetComponent<IlluminationController>();
        paused = true;
        SpFunctions.Pause("menu");
        GameObject.FindGameObjectWithTag(Tags.cam).GetComponent<CameraController>().Initialize();
    }

    /// <summary>
    /// Здесь будет происходить загрузка
    /// </summary>
    public void LoadGameData()
    {
        if (PlayerPrefs.HasKey("SaveDatapath"))
        {
            Map map = GetComponent<Map>();
            string savePath = PlayerPrefs.GetString("SaveDatapath");
            GameData loadData = Serializator.DeXml(savePath);
            KeyboardActorController _player = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<KeyboardActorController>();
            EquipmentClass equip = (EquipmentClass)_player.GetEquipment();
            Transform allWindows = GameObject.FindGameObjectWithTag(Tags.interfaceWindows).transform;
            EquipmentWindow equipWindow = allWindows.GetComponentInChildren<EquipmentWindow>();
            JournalWindow journalWindow = allWindows.GetComponentInChildren<JournalWindow>();
            GameHistory gHistory = GetComponent<GameHistory>();
            MapWindow mapWindow = allWindows.GetComponentInChildren<MapWindow>();

            if (loadData != null)
            {

                #region gameStatistics

                GameStatsData loadSData = loadData.gameStats;
                SerializedGameTime gTime = loadSData.gameTime;
                GameTime.timer = gTime.timer;
                GameTime.SetTime(gTime.monthNumb, gTime.day, gTime.hour, gTime.min);
                lastCheckPoint = map.rooms.Find(x => string.Equals(x.id.areaName, loadSData.lastCheckpoint)).GetComponentInChildren<CheckpointActions>();
                lastCheckPoint.Activated = true;
                currentArea = map.rooms.Find(x => string.Equals(x.id.areaName, loadSData.currentRoom));
                deathNumber = loadSData.deathTimes;

                #endregion //gameStatistics

                #region mainCharacter

                BuffDatabase bBase = GetComponentInChildren<BuffDatabase>();

                PersonData charData = loadData.mainCharData;
                _player.transform.position = charData.position;
                _player.SetRoomPosition(map.rooms.Find(x => string.Equals(x.id.areaName, charData.roomPosition)));
                Vector3 scale = _player.transform.localScale;
                _player.transform.localScale = new Vector3(Mathf.Sign(scale.x * charData.orientation) * scale.x, scale.y, scale.z);
                OrganismStats orgStats = _player.GetOrgStats();
                orgStats.maxHealth = charData.maxHealth;
                orgStats.health = charData.health;
                if (bBase != null)
                {
                    foreach (string buff in charData.buffs)
                    {
                        if (bBase.BuffDict.ContainsKey(buff))
                        {
                            _player.buffList.AddBuff(new BuffClass(bBase.BuffDict[buff],_player.buffList));
                        }
                    }
                }

                #endregion //mainCharacter

                #region journal

                JournalInfo jInfo = loadData.jInfo;
                JournalDatabase jBase = GetComponentInChildren<JournalDatabase>();

                foreach (string activeQuestName in jInfo.activeQuests)
                {
                    if (jBase.QuestDict.ContainsKey(activeQuestName))
                    {
                        journalWindow.ActiveQuests.Add(jBase.QuestDict[activeQuestName]);
                    }
                }

                foreach (string completedQuestName in jInfo.completedQuests)
                {
                    if (jBase.QuestDict.ContainsKey(completedQuestName))
                    {
                        journalWindow.CompletedQuests.Add(jBase.QuestDict[completedQuestName]);
                    }
                }

                foreach (string failedQuestName in jInfo.failedQuests)
                {
                    if (jBase.QuestDict.ContainsKey(failedQuestName))
                    {
                        journalWindow.FailedQuests.Add(jBase.QuestDict[failedQuestName]);
                    }
                }

                foreach (string characterName in jInfo.characters)
                {
                    if (jBase.CharDict.ContainsKey(characterName))
                    {
                        journalWindow.CharactersList.Add(jBase.CharDict[characterName]);
                    }
                }

                foreach (string beastName in jInfo.beasts)
                {
                    if (jBase.BeastDict.ContainsKey(beastName))
                    {
                        journalWindow.BeastsList.Add(jBase.BeastDict[beastName]);
                    }
                }

                foreach (string locationName in jInfo.locations)
                {
                    if (jBase.LocationDict.ContainsKey(locationName))
                    {
                        journalWindow.LocationsList.Add(jBase.LocationDict[locationName]);
                    }
                }

                #endregion //journal

                #region equipment

                EquipmentInfo eInfo = loadData.eInfo;
                EquipmentDatabase eBase = GetComponentInChildren<EquipmentDatabase>();

                #region armor

                ArmorSet armor = equip.armor;

                if (!string.Equals(eInfo.helmet, string.Empty))
                {
                    if (eBase.ArmorDict.ContainsKey(eInfo.helmet))
                    {
                        armor.helmet = eBase.ArmorDict[eInfo.helmet];
                    }
                }
                else
                {
                    armor.helmet = null;
                }

                if (!string.Equals(eInfo.cuirass, string.Empty))
                {
                    if (eBase.ArmorDict.ContainsKey(eInfo.cuirass))
                    {
                        armor.cuirass = eBase.ArmorDict[eInfo.cuirass];
                    }
                }
                else
                {
                    armor.cuirass = null;
                }

                if (!string.Equals(eInfo.pants, string.Empty))
                {
                    if (eBase.ArmorDict.ContainsKey(eInfo.pants))
                    {
                        armor.pants = eBase.ArmorDict[eInfo.pants];
                    }
                }
                else
                {
                    armor.pants = null;
                }

                if (!string.Equals(eInfo.gloves, string.Empty))
                {
                    if (eBase.ArmorDict.ContainsKey(eInfo.gloves))
                    {
                        armor.gloves = eBase.ArmorDict[eInfo.gloves];
                    }
                }
                else
                {
                    armor.gloves = null;
                }

                if (!string.Equals(eInfo.boots, string.Empty))
                {
                    if (eBase.ArmorDict.ContainsKey(eInfo.boots))
                    {
                        armor.boots = eBase.ArmorDict[eInfo.boots];
                    }
                }
                else
                {
                    armor.boots = null;
                }

                #endregion //armor

                #region weapons

                if (!string.Equals(eInfo.rightWeapon1, string.Empty))
                {
                    if (eBase.WeaponDict.ContainsKey(eInfo.rightWeapon1))
                    {
                        equip.rightWeapon = eBase.WeaponDict[eInfo.rightWeapon1];
                    }
                }
                else
                {
                    equip.rightWeapon = null;
                }

                if (!string.Equals(eInfo.leftWeapon1, string.Empty))
                {
                    if (eBase.WeaponDict.ContainsKey(eInfo.leftWeapon1))
                    {
                        equip.leftWeapon = eBase.WeaponDict[eInfo.leftWeapon1];
                    }
                }
                else
                {
                    equip.leftWeapon = null;
                }

                if (!string.Equals(eInfo.rightWeapon2, string.Empty))
                {
                    if (eBase.WeaponDict.ContainsKey(eInfo.rightWeapon2))
                    {
                        equip.altRightWeapon = eBase.WeaponDict[eInfo.rightWeapon2];
                    }
                }
                else
                {
                    equip.altRightWeapon = null;
                }

                if (!string.Equals(eInfo.leftWeapon2, string.Empty))
                {
                    if (eBase.WeaponDict.ContainsKey(eInfo.leftWeapon2))
                    {
                        equip.altLeftWeapon = eBase.WeaponDict[eInfo.leftWeapon2];
                    }
                }
                else
                {
                    equip.altLeftWeapon = null;
                }

                #endregion //weapons

                #region useItems

                if (!string.Equals(eInfo.useItem1.item, string.Empty))
                {
                    if (eBase.UseItemDict.ContainsKey(eInfo.useItem1.item))
                    {
                        equip.useItems[0] = new ItemBunch(eBase.UseItemDict[eInfo.useItem1.item],eInfo.useItem1.quantity);
                    }
                }
                else
                {
                    equip.useItems[0] = null;
                }

                if (!string.Equals(eInfo.useItem2.item, string.Empty))
                {
                    if (eBase.UseItemDict.ContainsKey(eInfo.useItem2.item))
                    {
                        equip.useItems[1] = new ItemBunch(eBase.UseItemDict[eInfo.useItem2.item], eInfo.useItem2.quantity);
                    }
                }
                else
                {
                    equip.useItems[1] = null;
                }

                if (!string.Equals(eInfo.useItem3.item, string.Empty))
                {
                    if (eBase.UseItemDict.ContainsKey(eInfo.useItem3.item))
                    {
                        equip.useItems[2] = new ItemBunch(eBase.UseItemDict[eInfo.useItem3.item], eInfo.useItem3.quantity);
                    }
                }
                else
                {
                    equip.useItems[2] = null;
                }

                if (!string.Equals(eInfo.useItem4.item, string.Empty))
                {
                    if (eBase.UseItemDict.ContainsKey(eInfo.useItem4.item))
                    {
                        equip.useItems[3] = new ItemBunch(eBase.UseItemDict[eInfo.useItem4.item], eInfo.useItem4.quantity);
                    }
                }
                else
                {
                    equip.useItems[3] = null;
                }

                equipWindow.InitializeBag(false);

                #endregion //useItems

                #region bag

                foreach (BagSlotInfo bagSlot in eInfo.bagInfo)
                {
                    if (!string.Equals(bagSlot.item, string.Empty))
                    {
                        if (eBase.ItemDict.ContainsKey(bagSlot.item))
                        {
                            equipWindow.AddItemInBag(new ItemBunch(eBase.ItemDict[bagSlot.item], bagSlot.quantity), bagSlot.index);
                        }
                    }
                    else
                    {
                        equipWindow.AddItemInBag(null, bagSlot.index);
                    }
                }

                #endregion //bag

                #region resources

                equip.gold = eInfo.gold;
                equip.keys[0] = eInfo.ironKeys;
                equip.keys[1] = eInfo.silverKeys;
                equip.keys[2] = eInfo.goldKeys;

                #endregion //resources

                equipWindow.InitializeBag(false);

                #endregion //equipment

                #region gameEvents

                GameEventInfo gInfo = loadData.gInfo;
                GameEventsDatabase gBase = GetComponentInChildren<GameEventsDatabase>();

                gHistory.journalEvents.journalList = new List<JournalEventScript>();
                foreach (string jEvent in gInfo.jEvents)
                {
                    if (gBase.JEventDict.ContainsKey(jEvent))
                    {
                        gHistory.journalEvents.AddJournalScript(gBase.JEventDict[jEvent]);
                    }
                }

                #endregion //gameEvents

                #region location

                LocationInfo lInfo = loadData.lInfo;

                #region formAllNewObjects

                Dictionary<string, InterObjController> savedObjsDict=new Dictionary<string, InterObjController>();
                SpawnObjectsDatabase sBase = GetComponentInChildren<SpawnObjectsDatabase>();

                AreaClass room = null;

                //Создадим те объекты, что были созданы во время сохранённой игры
                foreach (RoomData rInfo in lInfo.roomsData)
                {

                    room = map.rooms.Find(x => string.Equals(x.id.areaName, rInfo.roomName));
                    room.MaxRegistrationNumber = rInfo.maxRegistrationNumber;
                    for (int i=room.drops.Count-1;i>=0;i--)
                    {
                        Destroy(room.drops[i].gameObject);
                    }  
                   foreach (InterObjData intInfo in rInfo.objsData)
                   {
                       InterObjController intObj=null;
                       if (!map.StartObjsDict.ContainsKey(intInfo.objId))
                       {
                           if (sBase.SpawnDict.ContainsKey(intInfo.spawnId))
                           {
                               GameObject obj = Instantiate(sBase.SpawnDict[intInfo.spawnId]) as GameObject;
                               intObj = obj.GetComponent<InterObjController>();
                               if (intObj != null)
                               {
                                   intObj.ObjId = intInfo.objId;
                               }
                           }
                       }
                       else
                       {
                          intObj = map.StartObjsDict[intInfo.objId];
                       }
                       if (intObj != null)
                       {
                           savedObjsDict.Add(intInfo.objId, intObj);
                       }
                   }
               }

               //Удалим те объекты, что были удалены в сохранённой игре
               for (int i = 0; i < map.startObjList.Count; i++)
               {
                   if (!savedObjsDict.ContainsKey(map.startObjList[i].ObjId))
                   {
                       map.startObjList[i].DestroyInterObj();
                        map.startObjList.RemoveAt(i);
                        i--;
                   }
                }

                #endregion //formAllObjects

                #region setParametres

                foreach (RoomData rInfo in lInfo.roomsData)
                {
                    room = map.rooms.Find(x => string.Equals(x.id.areaName, rInfo.roomName));

                    foreach (InterObjData intInfo in rInfo.objsData)
                    {
                        if (savedObjsDict.ContainsKey(intInfo.objId))
                        {
                            savedObjsDict[intInfo.objId].AfterInitialize(intInfo, map, savedObjsDict);
                        }
                    }
                    foreach (DropInfo dropInfo in rInfo.dropsInfo)
                    {
                        GameObject drop = Instantiate(sBase.dropPrefab) as GameObject;
                        drop.GetComponent<DropClass>().SetDrop(dropInfo, eBase);
                        room.drops.Add(drop.GetComponent<DropClass>());
                    }
                    foreach (DoorData doorInfo in rInfo.doorsInfo)
                    {
                        room.neigbAreas[doorInfo.index].door.GetComponent<DoorClass>().locker.opened = doorInfo.opened;
                    }
                }

                #endregion //setParametres

                #endregion //location

                #region map

                MapData mInfo = loadData.mInfo;

                if (mInfo!=null)
                {
                    mapWindow.AfterInitialize(mInfo);
                }

                #endregion //map


            }
            else
            {
                GameTime.timer = 75;
                GameTime.SetTime(defMonth, defDay, defHour, defMin);
                deathNumber = 0;
                equipWindow.InitializeBag(true);
                gHistory.journalEvents.Initialize();
            }

            equipWindow.InitializeCharacterDoll();
            journalWindow.InitializeJournalData();
            _player.InitializeEquipment();
        }
    }

    /// <summary>
    /// Функция, что возвращает всю необходимую информацию об игре в данный момент (чтобы сохранить её)
    /// </summary>
    public GameData GetGameData()
    {
        Transform allWindows = GameObject.FindGameObjectWithTag(Tags.interfaceWindows).transform;
        KeyboardActorController _player = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<KeyboardActorController>();
        return new GameData(this,
                            _player,
                            allWindows.GetComponentInChildren<JournalWindow>(),
                            (EquipmentClass)_player.GetEquipment(),
                            allWindows.GetComponentInChildren<EquipmentWindow>().BagSlots,
                            GetComponent<GameHistory>().journalEvents.journalList,
                            GetComponent<Map>(),
                            allWindows.GetComponentInChildren<MapWindow>());
    }

    /// <summary>
    /// Функция, что возвращает время начала игры строчкой
    /// </summary>
    /// <returns></returns>
    public static string GetDefaultTime()
    {
        return GameTime.TimeString(defMonth,defDay,defHour,defMin);
    }

    #region events

    /// <summary>
    /// Событие о том, что кто-то что-то сказал или высветилось важное сообщение
    /// </summary>
    public void OnMessageSent(MessageSentEventArgs e)
    {
        EventHandler<MessageSentEventArgs> handler = MessageSentEvent;

        if (handler != null)
        {
            handler(this, e);
        }
    }

    /// <summary>
    /// Событие о том, что игра началась
    /// </summary>
    public void OnGameBegin(JournalEventArgs e)
    {
        EventHandler<JournalEventArgs> handler = GameStatsJournalEvent;

        if (handler != null)
        {
            handler(this, e);
        }
    }

    #endregion //events

}

/// <summary>
/// Игровое время. Сколько месяцев, дней, часов прошло с начала. 
/// </summary>
public static class GameTime
{
    public const float dayTime = 180f;//Сколько секунд реального времени длится день?

    public static float timer;
    public static List<Month> months = new List<Month>(new Month[]{ new Month("January", 31), new Month("February", 28),
                                                                    new Month("March",31), new Month("April",30),
                                                                    new Month("May",31), new Month("June",30),
                                                                    new Month("July",31),new Month("August",31),
                                                                    new Month("Septmber",30), new Month("October",31),
                                                                    new Month("November",30), new Month("December",31) });
    public static int monthNumb, day, hour, min;

    /// <summary>
    /// Метод, отвечающий за течение игрового времени
    /// </summary>
    public static void TimeFlow()
    {
        int sec;
        timer += Time.deltaTime;
        if (timer > dayTime)
        {
            day++;
            if (day > months[monthNumb].days)
            {
                day = 1;
                monthNumb++;
                if (monthNumb > 11)
                {
                    monthNumb = 1;
                }
            }
            timer -= dayTime;
        }
        sec = Mathf.RoundToInt(timer / dayTime * 3600 * 24);
        hour = sec / 3600;
        min = (sec % 3600) / 60;
    }

    /// <summary>
    /// С помощью этого метода в новой игре выставляется заданное дефолтное время.
    /// </summary>
    /// <param name="_month"></param>
    /// <param name="_day"></param>
    /// <param name="_hour"></param>
    /// <param name="_min"></param>
    public static void SetTime(int _month, int _day, int _hour, int _min)
    {
        monthNumb = _month; day = _day; hour = _hour; min = _min;
    }

    /// <summary>
    /// Метод, использующийся при выводе игрового времени
    /// </summary>
    public static string TimeString()
    {
        return months[monthNumb].name + " " + day.ToString() + " " + hour.ToString() + ":" + min.ToString();
    }

    /// <summary>
    /// Метод, использующийся при выводе игрового времени
    /// </summary>
    public static string TimeString(int _month, int _day, int _hour, int _min)
    {
        return months[_month].name + " " + _day.ToString() + " " + _hour.ToString() + ":" + _min.ToString();
    }

}
/// <summary>
/// Класс, пердставляющий собой название месяца и кол-во дней в месяце
/// </summary>
public class Month
{
    public string name;
    public int days;

    public Month(string _name, int _days)
    {
        name = _name; days = _days;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameStatistics))]
public class GameStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.LabelField("Time");
            EditorGUILayout.LabelField(GameTime.TimeString());
        }

    }
}
#endif