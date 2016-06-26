using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Визуальное отображение комнаты на карте
/// </summary>
public class RoomMap : MonoBehaviour
{

    #region consts

    public const float iconRatioX = 0.016f;
    public const float iconRatioY = 0.0135f;

    #endregion //consts

    #region fields

    private GameObject roomPanel;
    private Image img;
    private bool incognitta;//Была ли разведанна эта территория ранее?
    public bool Incognitta { get { return incognitta; }}

    private Transform doors;//Двери в комнате (а точнее замки на них)
    public Transform Doors { get { return doors; } }

    private GameObject lockIcon, spLockIcon;//Иконки замка и специального замка

    #region iconLists

    private List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> Enemies { get { return enemies; } }

    private List<GameObject> characters = new List<GameObject>();
    public List<GameObject> Characters { get { return characters; } }

    private List<GameObject> quests = new List<GameObject>();
    public List<GameObject> Quests { get { return quests; } }

    private List<GameObject> items = new List<GameObject>();
    public List<GameObject> Items { get { return items; } }

    #endregion //iconLists

    private Text roomText;

    #endregion //fields

    public void Initialize()
    {
        roomPanel = transform.FindChild("Panel").gameObject;
        img = GetComponent<Image>();
        incognitta = true;
        doors = roomPanel.transform.FindChild("Doors");
        MapWindow mapWindow = transform.parent.parent.parent.parent.GetComponent<MapWindow>();
        lockIcon = mapWindow.icons.Find(x => string.Equals(x.name, "LockIcon"));
        spLockIcon = mapWindow.icons.Find(x => string.Equals(x.name, "SpecialLockIcon"));

        enemies = new List<GameObject>();
        characters = new List<GameObject>();
        quests = new List<GameObject>();
        items = new List<GameObject>();

        roomText = roomPanel.transform.FindChild("RoomName").GetComponent<Text>();
        roomText.text = gameObject.name;

    }

    /// <summary>
    /// Обновить карту комнаты
    /// </summary>
    public void RefreshRoomMap(AreaClass room)
    {
        if (incognitta)
        {
            incognitta = false;
            roomPanel.SetActive(true);
            img.color = new Color(0f, 0f, 0f, 0f);
        }

        ClearIconsLists();

        foreach (RoomConnection roomConnection in room.neigbAreas)
        {
            DoorClass door = roomConnection.door.GetComponent<DoorClass>();
            if (door != null)
            {
                if (door.locker.opened ? false : door.locker.lockType > 0)
                {
                    AddIcon(null, door.locker.lockType < 4 ? lockIcon : spLockIcon, doors, room.position, door.transform.position);
                }
            }
        }
    }

    /// <summary>
    /// Обновить карту комнаты
    /// </summary>
    public void RefreshRoomMap()
    {
        if (incognitta)
        {
            incognitta = false;
            roomPanel.SetActive(true);
            img.color = new Color(0f, 0f, 0f, 0f);
        }
    }

    /// <summary>
    /// Добавить врага на карту
    /// </summary>
    public void AddEnemy(GameObject _icon, Transform panel, Vector3 roomPosition, Vector3 objectPosition)
    {
        AddIcon(enemies, _icon, panel, roomPosition, objectPosition);
    }

    /// <summary>
    /// Добавить врага на карту
    /// </summary>
    public void AddEnemy(GameObject _icon, Transform panel, Vector3 iconPosition)
    {
        AddIcon(enemies, _icon, panel, iconPosition);
    }

    /// <summary>
    /// Добавить персонажа на карту
    /// </summary>
    public void AddCharacter(GameObject _icon, Transform panel, Vector3 roomPosition, Vector3 objectPosition)
    {
        AddIcon(characters, _icon, panel, roomPosition, objectPosition);
    }

    /// <summary>
    /// Добавить персонажа на карту
    /// </summary>
    public void AddCharacter(GameObject _icon, Transform panel, Vector3 iconPosition)
    {
        AddIcon(characters, _icon, panel, iconPosition);
    }

    //Учесть положение главного героя
    public GameObject AddMainHero(GameObject _icon, Transform panel, AreaClass currentRoom, Vector3 objectPosition)
    {
        GameObject icon1 = Instantiate(_icon, transform.position + new Vector3(iconRatioX * (objectPosition.x- currentRoom.position.x), 
                                                                               iconRatioY*(objectPosition.y - currentRoom.position.y),
                                                                               0f), Quaternion.identity) as GameObject;
        icon1.transform.SetParent(panel);
        icon1.transform.localScale = new Vector3(1f, 1f, 1f);
        return icon1;
    }

    /// <summary>
    /// Добавить предмет либо сундук на карту
    /// </summary>
    public void AddItem(GameObject _icon, Transform panel, Vector3 roomPosition, Vector3 objectPosition)
    {
        AddIcon(items, _icon, panel, roomPosition, objectPosition);
    }

    /// <summary>
    /// Добавить предмет либо сундук на карту
    /// </summary>
    public void AddItem(GameObject _icon, Transform panel, Vector3 iconPosition)
    {
        AddIcon(items, _icon, panel, iconPosition);
    }

    /// <summary>
    /// Добавить квестовый объект на карту
    /// </summary>
    public void AddQuest(GameObject _icon, Transform panel, Vector3 roomPosition, Vector3 objectPosition)
    {
        AddIcon(quests, _icon, panel, roomPosition, objectPosition);
    }

    /// <summary>
    /// Добавить квестовый объект на карту
    /// </summary>
    public void AddQuest(GameObject _icon, Transform panel, Vector3 iconPosition)
    {
        AddIcon(quests, _icon, panel, iconPosition);
    }

    /// <summary>
    /// Добавить дверь (а точнее замок на неё на карту)
    /// </summary>
    public void AddDoor(string lockType, Transform panel, Vector3 iconPosition)
    {
        if (string.Equals(lockType, "Lock"))
        {
            AddIcon(null, lockIcon, doors, iconPosition);
        }
        else if (string.Equals(lockType, "SpecialLock"))
        {
            AddIcon(null,spLockIcon,doors,iconPosition);
        }
    }

    /// <summary>
    /// Добавить иконку на карту
    /// </summary>
    private void AddIcon(List<GameObject> _iconsList, GameObject _icon, Transform panel, Vector3 roomPosition, Vector3 objectPosition)
    {
        GameObject icon1 = Instantiate(_icon, transform.position + new Vector3(iconRatioX*(objectPosition.x-roomPosition.x),
                                                                               iconRatioY*(objectPosition.y-roomPosition.y),
                                                                               0f), Quaternion.identity) as GameObject;
        if (_iconsList != null)
        {
            _iconsList.Add(icon1);
        }
        icon1.transform.SetParent(panel);
        icon1.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    /// <summary>
    /// Добавить иконку на карту
    /// </summary>
    public void AddIcon(List<GameObject> _iconsList, GameObject _icon, Transform panel, Vector3 iconPosition)
    {
        GameObject icon1 = Instantiate(_icon) as GameObject;
        if (_iconsList != null)
        {
            _iconsList.Add(icon1);
        }
        icon1.transform.SetParent(panel);
        icon1.transform.localPosition = iconPosition;
        icon1.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void HideDoors(bool hide)
    {
        doors.gameObject.SetActive(!hide);
    }

    /// <summary>
    /// Исследовать территорию
    /// </summary>
    void Investigate()
    {
        incognitta = false;
        roomPanel.SetActive(true);
    }

    /// <summary>
    /// Очистить все списки
    /// </summary>
    void ClearIconsLists()
    {
        GameObject icon = null;
        for (int i = 0; i < enemies.Count; i++)
        {
            icon = enemies[i];
            enemies.RemoveAt(i);
            Destroy(icon);
            i--;
        }
        for (int i = 0; i < characters.Count; i++)
        {
            icon = characters[i];
            characters.RemoveAt(i);
            Destroy(icon);
            i--;
        }
        for (int i = 0; i < items.Count; i++)
        {
            icon = items[i];
            items.RemoveAt(i);
            Destroy(icon);
            i--;
        }
        for (int i = 0; i < quests.Count; i++)
        {
            icon = quests[i];
            quests.RemoveAt(i);
            Destroy(icon);
            i--;
        }

        for (int i = 0; i < doors.childCount; i++)
        {
            Destroy(doors.GetChild(i).gameObject);
        }
    }

}
