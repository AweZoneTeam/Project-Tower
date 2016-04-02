using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Окно интерфейса, отвечающий за журнал
/// </summary>
public class JournalWindow : InterfaceWindow
{

    #region fields

    private Canvas activeWindow;
    private Text activeName, activeSecondName, activeDescription;
    private Image activeImage;
    private Canvas quests, characters, beasts, locations;
    public GameObject journalButton;

    #region quests

    private List<QuestData> activeQuests = new List<QuestData>(), completedQuests = new List<QuestData>(), failedQuests = new List<QuestData>(); //Списки всех заданий, с которыми встречался персонаж
    private Transform activeQuestsPanel, completedQuestsPanel, failedQuestsPanel;
    private Image questImage;
    private Text questNameText, questConditionsText, questDescriptionText;

    #endregion //quests

    #region  characters

    private List<JournalData> charactersList = new List<JournalData>();
    private Transform charactersPanel;
    private Image characterImage;
    private Text charNameText, charSecondNameText, charDescriptionText;

    #endregion //characters

    #region beasts

    private List<JournalData> beastsList = new List<JournalData>();
    private Transform beastsPanel;
    private Image beastImage;
    private Text beastNameText, beastTypeText, beastDescriptionText;

    #endregion //beasts

    #region locations

    private List<JournalData> locationsList = new List<JournalData>();
    private Transform locationsPanel;
    private Image locationImage;
    private Text locationNameText,actNameText, locationDescriptionText;

    #endregion //locations

    #endregion //fields

    public void Awake()
    {
        Initialize();
    }

    public override void Initialize()
    {
        activeWindow = null;
        Transform allWindows = transform.parent;
        quests = allWindows.FindChild("Quests").GetComponent<Canvas>();
        characters = allWindows.FindChild("Characters").GetComponent<Canvas>();
        beasts = allWindows.FindChild("Beasts").GetComponent<Canvas>();
        locations = allWindows.FindChild("Locations").GetComponent<Canvas>();

        #region quests

        Transform questsPanel1 = quests.transform.FindChild("Panel1");
        Transform questsPanel2 = quests.transform.FindChild("Panel2");
        activeQuestsPanel = questsPanel1.FindChild("ActiveQuests").FindChild("QuestsPanel");
        completedQuestsPanel = questsPanel1.FindChild("CompletedQuests").FindChild("QuestsPanel");
        failedQuestsPanel = questsPanel1.FindChild("FailedQuests").FindChild("QuestsPanel");
        questImage = questsPanel2.FindChild("QuestImage").GetComponent<Image>();
        questNameText = questsPanel2.FindChild("QuestNameText").GetComponent<Text>();
        questConditionsText = questsPanel2.FindChild("ConditionText").FindChild("Conditions").GetComponent<Text>();
        questDescriptionText = questsPanel2.FindChild("Description").FindChild("DescriptionScroll").FindChild("DescriptionText").GetComponent<Text>();

        #endregion //quests

        #region characters

        Transform charPanel1 = characters.transform.FindChild("Panel1");
        Transform charPanel2 = characters.transform.FindChild("Panel2");
        charactersPanel = charPanel1.FindChild("CharactersList").FindChild("CharactersPanel");
        characterImage = charPanel2.FindChild("CharacterImage").GetComponent<Image>();
        charNameText = charPanel2.FindChild("NameText").GetComponent<Text>();
        charSecondNameText = charNameText.transform.FindChild("SecondNameText").GetComponent<Text>();
        charDescriptionText = charPanel2.FindChild("Description").FindChild("DescriptionScroll").FindChild("DescriptionText").GetComponent<Text>();

        #endregion //characters

        #region beasts

        Transform beastPanel1 = beasts.transform.FindChild("Panel1");
        Transform beastPanel2 = beasts.transform.FindChild("Panel2");
        beastsPanel = beastPanel1.FindChild("BeastsList").FindChild("BeastsPanel");
        beastImage = beastPanel2.FindChild("BeastImage").GetComponent<Image>();
        beastNameText = beastPanel2.FindChild("NameText").GetComponent<Text>();
        beastTypeText= beastPanel2.FindChild("TypeText").GetComponent<Text>();
        beastDescriptionText = beastPanel2.FindChild("Description").FindChild("DescriptionScroll").FindChild("DescriptionText").GetComponent<Text>();

        #endregion //beasts

        #region locations

        Transform locationPanel1 = locations.transform.FindChild("Panel1");
        Transform locationPanel2 = locations.transform.FindChild("Panel2");
        locationsPanel = locationPanel1.FindChild("LocationsList").FindChild("LocationsPanel");
        locationImage = locationPanel2.FindChild("LocationImage").GetComponent<Image>();
        locationNameText = locationPanel2.FindChild("NameText").GetComponent<Text>();
        actNameText = locationNameText.transform.FindChild("ActText").GetComponent<Text>();
        locationDescriptionText = locationPanel2.FindChild("Description").FindChild("DescriptionScroll").FindChild("DescriptionText").GetComponent<Text>();

        #endregion //locations

        JournalScriptStock jStock = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameHistory>().journalEvents;
        jStock.NewJournalDataEvent += JournalDataAddEventHandler;
        jStock.QuestCompletedEvent += LevelCompleteEventHandler;

    }

    /// <summary>
    /// Сменить активное окно журнала
    /// </summary>
    public void ChangeWindow(string windowName)
    {
        if (activeWindow != null)
        {
            activeWindow.enabled = false;
        }
        if (string.Equals(windowName, "Quests"))
        {
            activeWindow = quests;
            activeName = questNameText;
            activeSecondName = questConditionsText;
            activeDescription = questDescriptionText;
            activeImage = questImage;
        }
        else if (string.Equals(windowName, "Characters"))
        {
            activeWindow = characters;
            activeName = charNameText;
            activeSecondName = charSecondNameText;
            activeDescription = charDescriptionText;
            activeImage = characterImage;
        }
        else if (string.Equals(windowName, "Beasts"))
        {
            activeWindow = beasts;
            activeName = beastNameText;
            activeSecondName = beastTypeText;
            activeDescription = beastDescriptionText;
            activeImage = beastImage;
        }
        else if (string.Equals(windowName, "Locations"))
        {
            activeWindow = locations;
            activeName = locationNameText;
            activeSecondName = actNameText;
            activeDescription = locationDescriptionText;
            activeImage = locationImage;
        }
        activeWindow.enabled = true;
        SetVoidData();
    }

    /// <summary>
    /// Начать показ выбранной журнальной информации
    /// </summary>
    public void SetData(JournalData jData)
    {
        activeName.text = jData.dataName;
        activeDescription.text = jData.dataDescription;
        if (jData is QuestData)
        {
            QuestData qData = (QuestData)jData;
            activeSecondName.text = qData.conditions;
        }
        else
        {
            activeSecondName.text = jData.dataSecondName;
        }
        activeImage.sprite = jData.dataImage;
    }

    /// <summary>
    /// Установить активное окно пустым
    /// </summary>
    public void SetVoidData()
    {
        activeName.text = "";
        activeSecondName.text = "";
        activeDescription.text = "";
        activeImage.sprite = null;
    }

    /// <summary>
    /// Сформировать спискок кнопок журнала
    /// </summary>
    void FormDataButtons(Transform panel, List<JournalData> jList)
    {
        for (int i = panel.childCount-1; i >=0 ; i--)
        {
            DestroyImmediate(panel.GetChild(i).gameObject);
        }
        for (int i=0;i<jList.Count;i++)
        {
            GameObject button = GameObject.Instantiate(journalButton) as GameObject;
            RectTransform bTransform = button.GetComponent<RectTransform>();
            button.GetComponentInChildren<Text>().text = jList[i].dataName;
            button.GetComponent<JournalButton>().jData = jList[i];
            button.transform.parent = panel;
            Vector3 localPos = bTransform.anchoredPosition;
            bTransform.anchoredPosition3D = new Vector3(0f, -30f * (i + 0.5f), 0f);
            bTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        RectTransform rTransform = panel.GetComponent<RectTransform>();
        if (30f * (jList.Count + 0.5f) > rTransform.sizeDelta.y)
        {
            panel.GetComponent<RectTransform>().sizeDelta = new Vector2(rTransform.sizeDelta.x, 30f * (jList.Count + 0.5f));
        }
    }

    /// <summary>
    /// Сформировать список кнопок журнала (а именно, список квестов)
    /// </summary>
    void FormDataButtons(Transform panel, List<QuestData> jList)
    {

        for (int i = panel.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(panel.GetChild(i).gameObject);
        }
        for (int i = 0; i < jList.Count; i++)
        {
            GameObject button = GameObject.Instantiate(journalButton) as GameObject;
            RectTransform bTransform = button.GetComponent<RectTransform>();
            button.GetComponentInChildren<Text>().text = jList[i].dataName;
            button.GetComponent<JournalButton>().jData = jList[i];
            button.transform.parent = panel;
            Vector3 localPos = bTransform.anchoredPosition;
            bTransform.anchoredPosition3D = new Vector3(0f, -30f * (i + 0.5f),0f);
            bTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        RectTransform rTransform = panel.GetComponent<RectTransform>();
        if (30f * (jList.Count + 0.5f) > rTransform.sizeDelta.y)
        {
            panel.GetComponent<RectTransform>().sizeDelta = new Vector2(rTransform.sizeDelta.x, 30f * (jList.Count + 0.5f));
        }
    }

    /// <summary>
    /// Обработчик события "Добавлена запись"
    /// </summary>
    public void JournalDataAddEventHandler(object sender, JournalRefreshEventArgs e)
    {

        #region quests

        if (string.Equals(e.JData.dataType,"quest"))
        {
            if (!activeQuests.Contains((QuestData)e.JData))
            {
                activeQuests.Add((QuestData)e.JData);
                SpFunctions.SendMessage(new MessageSentEventArgs("Добавленно новое задание " + e.JData.dataName, 2, 1.5f));
                FormDataButtons(activeQuestsPanel, activeQuests);    
            }
        }

        #endregion //quests

        #region characters

        else if (string.Equals(e.JData.dataType, "character"))
        {
            if (!charactersList.Contains(e.JData))
            {
                charactersList.Add(e.JData);
                SpFunctions.SendMessage(new MessageSentEventArgs("Новая запись в журнале: " + e.JData.dataName, 2, 1.5f));
                FormDataButtons(charactersPanel, charactersList);
            }
        }

        #endregion //characters

        #region beasts

        else if (string.Equals(e.JData.dataType, "beast"))
        {
            if (!beastsList.Contains(e.JData))
            {
                beastsList.Add(e.JData);
                SpFunctions.SendMessage(new MessageSentEventArgs("Новая запись в журнале: " + e.JData.dataName, 2, 1.5f));
                FormDataButtons(beastsPanel, beastsList);
            }
        }

        #endregion //beasts

        #region locations

        else if (string.Equals(e.JData.dataType, "location"))
        {
            if (!locationsList.Contains(e.JData))
            {
                locationsList.Add(e.JData);
                SpFunctions.SendMessage(new MessageSentEventArgs("Новая запись в журнале: " + e.JData.dataName, 2, 1.5f));
                FormDataButtons(locationsPanel, locationsList);
            }
        }

        #endregion //locations

    }

    /// <summary>
    /// Обработчик журнального события "Задание выполнено!"
    /// </summary>
    public void LevelCompleteEventHandler(object sender, JournalRefreshEventArgs e)
    {
        if (string.Equals(e.JData.dataType, "quest"))
        {
            if (activeQuests.Contains((QuestData)e.JData))
            {
                activeQuests.Remove((QuestData)e.JData);
            }
            completedQuests.Add((QuestData)e.JData);
            SpFunctions.SendMessage(new MessageSentEventArgs("Задание: " + e.JData.dataName+" выполнено!", 2, 1.5f));
            FormDataButtons(activeQuestsPanel, activeQuests);
            FormDataButtons(completedQuestsPanel, completedQuests);
        }

    }

}
