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

    private MenuWindow menu;
    private Settings settings;
    private GameObject allWindows;
    private GameObject cam;

    public void FixedUpdate()
    {
        //allWindows.transform.position = cam.transform.position + new Vector3(xOffset, yOffset, zOffset);
    }

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        activeWindow = null;
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
        if (string.Equals("menu", windowName))
        {
            activeWindow = menu.GetComponent<Canvas>();
        }
        else if (string.Equals("settings", windowName))
        {
            activeWindow = settings.GetComponent<Canvas>();
        }
        activeWindow.enabled = true;
    }

}
