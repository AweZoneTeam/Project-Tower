using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Окно в котором реализуются игровые сообщения
/// </summary>
public class InterfaceMessageWindow : InterfaceWindow
{
    public Text upText, mediumText, downText;//Текстовые окошки, которыми управляет окно

    public override void Initialize()
    {
        upText = transform.FindChild("UpText").gameObject.GetComponent<Text>();
        mediumText = transform.FindChild("MediumText").gameObject.GetComponent<Text>();
        downText = transform.FindChild("DownText").gameObject.GetComponent<Text>();
        Transform leftBagTrans = transform.FindChild("LeftBag");
        Transform rightBagTrans = transform.FindChild("RightBag");
    }

    /// <summary>
    /// Установить тект в верхнем окошке
    /// </summary>
    public void SetUpMessage(string msg)
    {
        upText.text = msg;
    }

    /// <summary>
    /// Установить текст в среднем окошке
    /// </summary>
    public void SetMediumMessage(string msg)
    {
        mediumText.text = msg;
    }

    /// <summary>
    /// Установить текст в нижнем окошке
    /// </summary>
    public void SetDownMessage(string msg)
    {
        downText.text = msg;
    }

    public void Start()
    {
        Initialize();
    }

}
