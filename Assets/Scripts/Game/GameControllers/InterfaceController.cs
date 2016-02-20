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

    private GameObject allWindows;
    private GameObject cam;

    public void FixedUpdate()
    {
        allWindows.transform.position = cam.transform.position + new Vector3(xOffset, yOffset, zOffset);
    }

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        allWindows = GameObject.FindGameObjectWithTag(Tags.interfaceWindows);
        exchWindow = allWindows.GetComponentInChildren<InterfaceExchangeWindow>();
        exchWindow.gameObject.GetComponent<Canvas>().enabled=false;
        cam = GameObject.FindGameObjectWithTag(Tags.cam);
    }

    public void OpenExchangeWindow(BagClass leftBag, BagClass rightBag)
    {
        exchWindow.gameObject.GetComponent<Canvas>().enabled=true;
        exchWindow.leftBag = leftBag;
        exchWindow.rightBag = rightBag;
        exchWindow.SetImages();
    }
}
