using UnityEngine;
using System.Collections;

/// <summary>
/// Скрипт, хранящий данные по всем окошкам интерфейса и управляющий ими
/// </summary>
public class InterfaceController : MonoBehaviour
{

    #region consts

    const float xOffset = 470f;
    const float yOffset = 210f;
    const float zOffset = 30f;

    #endregion //consts

    public InterfaceExchangeWindow exchWindow;//Окно обмена предметами
    public Texture2D cursorSprite;

    private Canvas activeWindow;
    private Canvas auxActiveWindow;

    private MenuWindow menu;
    private SaveMenu saves;
    private Settings settings;
    public JournalWindow journal;
    private EquipmentWindow equipment;
    private MapWindow mapWindow;
    private DialogWindow dialog;
    private GameObject allWindows;
    private GameObject cam;

    public void Awake()
    {
        Cursor.SetCursor(cursorSprite, Vector2.zero, CursorMode.Auto);
    }

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        activeWindow = null;
        auxActiveWindow = null;
        cam = GameObject.FindGameObjectWithTag(Tags.cam);
        allWindows = GameObject.FindGameObjectWithTag(Tags.interfaceWindows);
        if (allWindows.GetComponentInChildren<InterfaceExchangeWindow>() != null)
        {
            exchWindow = allWindows.GetComponentInChildren<InterfaceExchangeWindow>();
            exchWindow.gameObject.GetComponent<Canvas>().enabled = false;
        }
        if (allWindows.GetComponentInChildren<MenuWindow>()!=null)
        {
            menu = allWindows.GetComponentInChildren<MenuWindow>();
            menu.Initialize();
            activeWindow = menu.GetComponent<Canvas>();
        }
        if (allWindows.GetComponentInChildren<SaveMenu>() != null)
        {
            saves = allWindows.GetComponentInChildren<SaveMenu>();
            saves.Initialize();
        }
        if (allWindows.GetComponentInChildren<Settings>() != null)
        {
            settings = allWindows.GetComponentInChildren<Settings>();
        }
        if (allWindows.GetComponentInChildren<JournalWindow>() != null)
        {
            journal = allWindows.GetComponentInChildren<JournalWindow>();
        }
        if (allWindows.GetComponentInChildren<EquipmentWindow>() != null)
        {
            equipment = allWindows.GetComponentInChildren<EquipmentWindow>();
        }
        if (allWindows.GetComponentInChildren<DialogWindow>() != null)
        {
            dialog = allWindows.GetComponentInChildren<DialogWindow>();
        }
        if (allWindows.GetComponentInChildren<MapWindow>() != null)
        {
           mapWindow = allWindows.GetComponentInChildren<MapWindow>();
        }
    }

    public void OpenExchangeWindow(BagClass bag, EquipmentClass _equip, Transform _exchanger1, Transform _exchanger2)
    {
        exchWindow.SetBag(bag, _equip,_exchanger1,_exchanger2);
        exchWindow.GetComponent<Canvas>().enabled = true;
    }

    public void OpenDialogWindow(NPCActions _NPC)
    {
        dialog.Init(_NPC);
    }

    /// <summary>
    /// Закрытие активного окна
    /// </summary>
    public void CloseActiveWindow()
    {
        if (activeWindow != null)
        {
            activeWindow.enabled = false;
        }
        if (auxActiveWindow != null)
        {
            auxActiveWindow.enabled = false;
        }
    }

    /// <summary>
    /// Функция, что по строке открывает нужное окно и закрывает то, что открыто в данный момент
    /// </summary>
    public void ChangeWindow(string windowName)
    {
        if (activeWindow != null)
        {
            activeWindow.enabled = false;
        }
        if (auxActiveWindow != null)
        {
            auxActiveWindow.enabled = false;
        }
        if (string.Equals("menu", windowName))
        {
            activeWindow = menu.GetComponent<Canvas>();
        }
        else if (string.Equals("saves", windowName))
        {
            activeWindow = saves.GetComponent<Canvas>();
        }
        else if (string.Equals("settings", windowName))
        {
            activeWindow = settings.GetComponent<Canvas>();
        }
        else if (string.Equals("journal", windowName))
        {
            activeWindow = journal.GetComponent<Canvas>();
        }
        else if (string.Equals("equipment", windowName))
        {
            activeWindow = equipment.GetComponent<Canvas>();
        }
        else if (string.Equals("dialog", windowName))
        {
            activeWindow = dialog.GetComponent<Canvas>();
        }
        else if (string.Equals("map", windowName))
        {
            activeWindow = mapWindow.GetComponent<Canvas>();
            mapWindow.RefreshRoomMap();
        }
        activeWindow.enabled = true;
    }

    /// <summary>
    /// Функция, что по строке открывает нужное вспомогательное окно
    /// </summary>
    public void ChangeAuxWindow(string windowName)
    {
        if (auxActiveWindow != null)
        {
            auxActiveWindow.enabled = false;
        }
        if (string.Equals(windowName, "Quests"))
        {
            auxActiveWindow = transform.FindChild("Interface").FindChild("Quests").GetComponent<Canvas>();
            journal.ChangeWindow("Quests");
        }
        else if (string.Equals(windowName, "Characters"))
        {
            auxActiveWindow = transform.FindChild("Interface").FindChild("Characters").GetComponent<Canvas>();
            journal.ChangeWindow("Characters");
        }
        else if (string.Equals(windowName, "Beasts"))
        {
            auxActiveWindow = transform.FindChild("Interface").FindChild("Beasts").GetComponent<Canvas>();
            journal.ChangeWindow("Beasts");
        }
        else if (string.Equals(windowName, "Locations"))
        {
            auxActiveWindow = transform.FindChild("Interface").FindChild("Locations").GetComponent<Canvas>();
            journal.ChangeWindow("Locations");
        }
    }

}
