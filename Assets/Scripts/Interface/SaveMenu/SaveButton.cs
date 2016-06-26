using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Скрипт, ответственный за работу кнопок выбора момента загрузки и места сохранения.
/// </summary>
public class SaveButton : MonoBehaviour
{

    #region fields

    protected SaveMenu saveMenu;

    protected Text saveName, saveTime;
    public string SaveName {get { return saveName.text; }}
    protected Image img;

    #endregion //fields

    /// <summary>
    /// Функция выбора сохраняемых, либо загружаемых данных
    /// </summary>
    public void ChooseSave()
    {
        saveMenu.ChooseButton(this);
    }

    public void Initialize(SaveMenu sMenu, string sName, string sTime)
    {
        saveName = transform.FindChild("SaveName").GetComponent<Text>();
        saveTime = transform.FindChild("SaveTime").GetComponent<Text>();
        img = transform.FindChild("SaveImage").GetComponent<Image>();
        saveMenu = sMenu;
        saveName.text = sName;
        saveTime.text = sTime;
    }

    public void SetImage(Sprite sprite)
    {
        img.sprite = sprite;
    }

}
