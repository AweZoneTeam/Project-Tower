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

    private Canvas activeWindow;
    private Canvas auxActiveWindow;

    private MenuWindow menu;
    private Settings settings;
    public JournalWindow journal;
    private GameObject allWindows;
    private GameObject cam;

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
        }
        if (allWindows.GetComponentInChildren<MenuWindow>()!=null)
        {
            menu = allWindows.GetComponentInChildren<MenuWindow>();
            activeWindow = menu.GetComponent<Canvas>();
        }
        if (allWindows.GetComponentInChildren<Settings>() != null)
        {
            settings = allWindows.GetComponentInChildren<Settings>();
        }
        if (allWindows.GetComponentInChildren<JournalWindow>() != null)
        {
            journal = allWindows.GetComponentInChildren<JournalWindow>();
        }
        exchWindow.gameObject.GetComponent<Canvas>().enabled=false;
    }

    public void OpenExchangeWindow(BagClass leftBag, BagClass rightBag)
    {
        exchWindow.gameObject.GetComponent<Canvas>().enabled=true;
        exchWindow.leftBag = leftBag;
        exchWindow.rightBag = rightBag;
        exchWindow.SetImages();
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
        else if (string.Equals("settings", windowName))
        {
            activeWindow = settings.GetComponent<Canvas>();
        }
        else if (string.Equals("journal", windowName))
        {
            activeWindow = journal.GetComponent<Canvas>();
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
