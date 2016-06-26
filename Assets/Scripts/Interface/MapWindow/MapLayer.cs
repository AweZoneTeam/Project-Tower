using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, в котором содержится информация о солое карты
/// </summary>
public class MapLayer : MonoBehaviour
{

    #region fields

    private Transform roomsPanel, enemiesPanel, charactersPanel, itemsPanel, questsPanel;
    private GameObject enemyIcon, characterIcon, bossIcon, checkpointIcon, chestIcon, itemIcon, questIcon;//Иконки, используемые при составлении карты
    private List<RoomMap> roomMaps = new List<RoomMap>();//Комнаты, что находятся в этом слое карты
    public List<RoomMap> RoomMaps {get { return roomMaps; }}

    public string layer;

    #endregion //fields

    public void Initialize()
    {
        roomsPanel = transform.FindChild("Rooms");
        RoomMap roomMap = null;
        for (int i = 0; i < roomsPanel.childCount; i++)
        {
            roomMap = roomsPanel.GetChild(i).GetComponent<RoomMap>();
            roomMap.Initialize();
            roomMaps.Add(roomMap);
        }

        enemiesPanel = transform.FindChild("Enemies");
        charactersPanel = transform.FindChild("Characters");
        itemsPanel = transform.FindChild("Items");
        questsPanel = transform.FindChild("Quests");

        MapWindow mapWindow = transform.parent.parent.GetComponent<MapWindow>();
        enemyIcon = mapWindow.icons.Find(x => string.Equals(x.name, "EnemyIcon"));
        characterIcon = mapWindow.icons.Find(x => string.Equals(x.name, "CharacterIcon"));
        bossIcon = mapWindow.icons.Find(x => string.Equals(x.name, "BossIcon"));
        checkpointIcon = mapWindow.icons.Find(x => string.Equals(x.name, "CheckpointIcon"));
        chestIcon = mapWindow.icons.Find(x => string.Equals(x.name, "ChestIcon"));
        itemIcon = mapWindow.icons.Find(x => string.Equals(x.name, "ItemIcon"));
        questIcon = mapWindow.icons.Find(x => string.Equals(x.name, "QuestIcon"));
    }

    /// <summary>
    /// Проинициализировать состояние слоя карты, используя сохранённые данные
    /// </summary>
    public void AfterInitialize(MapLayerData mapLayerData)
    {
        RoomMap roomMap;
        foreach (RoomMapData rMapData in mapLayerData.roomMapsData)
        {
            roomMap = null;
            roomMap = roomMaps.Find(x => string.Equals(rMapData.roomName, x.gameObject.name));
            if (roomMap != null)
            {
                foreach (IconData iconData in rMapData.icons)
                {
                    if (string.Equals("Character", iconData.iconType))
                    {
                        roomMap.AddCharacter(characterIcon, charactersPanel, iconData.iconPosition);
                    }
                    else if (string.Equals("Enemy", iconData.iconType))
                    {
                        roomMap.AddEnemy(enemyIcon, enemiesPanel, iconData.iconPosition);
                    }
                    else if (string.Equals("Boss",iconData.iconType))
                    {
                        roomMap.AddEnemy(bossIcon, enemiesPanel, iconData.iconPosition);
                    }
                    else if (string.Equals("Checkpoint", iconData.iconType))
                    {
                        roomMap.AddItem(checkpointIcon, itemsPanel, iconData.iconPosition);
                    }
                    else if (string.Equals("Chest", iconData.iconType))
                    {
                        roomMap.AddItem(chestIcon, itemsPanel, iconData.iconPosition);
                    }
                    else if (string.Equals("Item", iconData.iconType))
                    {
                        roomMap.AddItem(itemIcon, itemsPanel, iconData.iconPosition);
                    }
                    else if (string.Equals("Quest", iconData.iconType))
                    {
                        roomMap.AddQuest(questIcon, questsPanel, iconData.iconPosition);
                    }
                    else if (string.Equals("Lock", iconData.iconType))
                    {
                        roomMap.AddDoor("Lock", enemiesPanel, iconData.iconPosition);
                    }
                    else if (string.Equals("SpecialLock", iconData.iconType))
                    {
                        roomMap.AddDoor("SpecialLock", enemiesPanel, iconData.iconPosition);
                    }
                }
                if (!rMapData.incognitta)
                {
                    roomMap.RefreshRoomMap();
                }
            }
        }
    }

    /// <summary>
    /// Обновить карту комнаты
    /// </summary>
    public void RefreshRoomMap(AreaClass room)
    {

        if (room != null)
        {
            RoomMap roomMap = roomMaps.Find(x => (string.Equals(x.gameObject.name, room.id.areaName)));

            if (roomMap != null)
            {
                roomMap.RefreshRoomMap(room);

                foreach (InterObjController interObj in room.container)
                {
                    ChestController chest;
                    AIController aiObj = null;
                    if ((aiObj = interObj.GetComponent<AIController>()) != null)
                    {
                        if (!aiObj.enemies.Contains(Tags.player))
                        {
                            roomMap.AddCharacter(characterIcon, charactersPanel, room.position, interObj.transform.position);
                        }
                        else if (aiObj is BossController)
                        {
                            roomMap.AddEnemy(bossIcon, enemiesPanel, room.position, interObj.transform.position);
                        }
                        else
                        {
                            roomMap.AddEnemy(enemyIcon, enemiesPanel, room.position, interObj.transform.position);
                        }
                    }
                    else if ((chest = interObj.GetComponent<ChestController>()) != null)
                    {
                        if (chest.GetChest().bag.Count != 0)
                        {
                            roomMap.AddItem(chestIcon, itemsPanel, room.position, interObj.transform.position);
                        }
                    }
                    else if (interObj.GetComponent<CheckpointActions>() != null)
                    {
                        roomMap.AddItem(checkpointIcon, itemsPanel, room.position, interObj.transform.position);
                    }
                    else if (interObj.GetComponent<DropClass>() != null)
                    {
                        roomMap.AddItem(itemIcon, itemsPanel, room.position, interObj.transform.position);
                    }
                }
                foreach (DropClass drop in room.drops)
                {
                    roomMap.AddItem(itemIcon, itemsPanel, room.position, drop.transform.position);
                }
            }
        }
    }

    /// <summary>
    /// Скрыть иконки определённого типа
    /// </summary>
    public void HideIcons(string iconType, bool hide)
    {
        if (string.Equals(iconType, "enemies"))
        {
            enemiesPanel.gameObject.SetActive(!hide);
        }
        else if (string.Equals(iconType, "characters"))
        {
            charactersPanel.gameObject.SetActive(!hide);
        }
        else if (string.Equals(iconType, "quests"))
        {
            questsPanel.gameObject.SetActive(!hide);
        }
        else if (string.Equals(iconType, "items"))
        {
            itemsPanel.gameObject.SetActive(!hide);
        }
        else if (string.Equals(iconType, "doors"))
        {
            foreach (RoomMap roomMap in roomMaps)
            {
                roomMap.HideDoors(hide);
            }
        }
    }

    /// <summary>
    /// Учесть положение главного героя
    /// </summary>
    public GameObject AddMainHero(GameObject _icon, AreaClass currentRoom, Vector3 objectPosition)
    {
        RoomMap roomMap = null;
        roomMap=roomMaps.Find(x => string.Equals(x.name, currentRoom.id.areaName));
        if (roomMap != null)
        {
            return roomMap.AddMainHero(_icon, transform, currentRoom, objectPosition);
        }
        return null;
    }

}
