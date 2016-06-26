using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Скрипт экрана сохранения игры
/// </summary>
public class SaveMenu : InterfaceWindow
{

    #region consts

    protected const float beginPosY = -35f;
    protected const float offsetY = -51f;

    #endregion //consts

    #region fields

    protected SavesInfo savesInfo;//Информация о самих сохранениях
    protected string savesInfoPath;

    protected string savePath;//По какому пути находятся все сохранения

    public GameObject saveButton;
    protected GameObject createNewSaveButton;
    protected GameObject returnButton;
    protected SaveButton chosenButton;

    protected string saveMod = "Save";//Сохраняет или загружает это окно данные игры?

    protected Text modNameText;
    protected Text actionText;

    protected Transform savesPanel;

    protected GameObject createNewFadePanel;
    protected InputField saveNameInputPanel;

    public Sprite oldSaveImage;

    #endregion //fields

    public override void Initialize()
    {
        savePath = (Application.dataPath) + "/StreamingAssets/Saves/";
        savesInfoPath = (Application.dataPath) + "/StreamingAssets/SavesInfo.xml";
        Debug.LogError((Application.dataPath) + "/StreamingAssets/SavesInfo.xml");
        savesInfo = Serializator.DeXmlSavesInfo(savesInfoPath);

        chosenButton = null;
        Transform mainPanel = transform.FindChild("MainPanel");
        modNameText = mainPanel.FindChild("TextPanel").GetComponentInChildren<Text>();
        returnButton = transform.FindChild("ReturnButton").gameObject;
        actionText = transform.FindChild("ActionButton").GetComponentInChildren<Text>();
        savesPanel = mainPanel.FindChild("Saves").FindChild("SavesPanel");
        createNewFadePanel = transform.FindChild("CreateNewFadePanel").gameObject;
        saveNameInputPanel = createNewFadePanel.transform.FindChild("CreateNewPanel").GetComponentInChildren<InputField>();
        createNewSaveButton = savesPanel.FindChild("CreateNewSaveButton").gameObject;

        if (savesInfo.saves.Count > 0)
        {
            InitializeButtons();
        }
        else
        {
            PlayerPrefs.SetString("SaveDatapath", string.Empty);
            MainMenuWindow mainMenu = transform.parent.GetComponentInChildren<MainMenuWindow>();
            if ((mainMenu != null) && (string.Equals(SceneManager.GetActiveScene().name,"Main Menu")))
            {
                mainMenu.OnlyNewGame(true);
            }
        }
    }

    /// <summary>
    /// Функция, что создаст все сэйвбатоны и проинициализирует их
    /// </summary>
    protected void InitializeButtons()
    {
        float offset = 0f;
        foreach (SaveInfo saveInfo in savesInfo.saves)
        {
            GameObject _saveButton = Instantiate(saveButton, saveButton.transform.position, saveButton.transform.rotation) as GameObject;
            _saveButton.transform.SetParent(savesPanel);
            _saveButton.transform.localPosition += new Vector3(0f, offset, 0f);
            _saveButton.transform.localScale = new Vector3(1f, 0.17f, 1f);
            _saveButton.GetComponent<SaveButton>().Initialize(this, saveInfo.saveName, saveInfo.saveTime);
            _saveButton.GetComponent<SaveButton>().SetImage(oldSaveImage);
            offset += offsetY;
        }
        createNewSaveButton.transform.localPosition += new Vector3(0f, offset, 0f);
    }

    /// <summary>
    /// Поменять режим работы окна (сохранение или загрузка)
    /// </summary>
    /// <param name="mod"></param>
    public void ChangeMod(string mod)
    {
        saveMod = mod;
        if (string.Equals("Save", saveMod))
        {
            if (!string.Equals(SceneManager.GetActiveScene().name, "Main Menu"))
            {
                returnButton.SetActive(false);
            }
            else
            {
                returnButton.SetActive(true);
            }
            createNewSaveButton.gameObject.SetActive(true);
            modNameText.text = "Сохранение игры";
            actionText.text = "Сохранить";
        }
        else if (string.Equals("Load", saveMod))
        {
            returnButton.SetActive(true);
            createNewSaveButton.gameObject.SetActive(false);
            modNameText.text = "Загрузка игры";
            actionText.text = "Загрузить";
        }
    }

    /// <summary>
    /// Выбрать то или иное сохранение и произвестис ним действие
    /// </summary>
    public void ChooseButton(SaveButton saveData)
    {
        if (saveData != chosenButton)
        {
            chosenButton = saveData;
        }
        else
        {
            SaveLoad(savePath + saveData.SaveName+".xml");
        }
    }

    /// <summary>
    /// Выбрать то или иное сохранение и произвестис ним действие
    /// </summary>
    public void ChooseButton()
    {
        if (chosenButton != null)
        {
            SaveLoad(savePath + chosenButton.SaveName+".xml");
        }
    }

    /// <summary>
    /// Произвести загрузку, либо сохранение, либо создать новое сохранение
    /// </summary>
    public void SaveLoad(string datapath)
    {
        if (string.Equals("Save", saveMod))
        {
            Save(datapath);
        }
        else if (string.Equals("Load", saveMod))
        {
            Load(datapath);
        }
    }

    /// <summary>
    /// Сохраниться
    /// </summary>
    protected void Save(string datapath)
    {
        MainMenuWindow mainMenu=transform.parent.GetComponentInChildren<MainMenuWindow>();
        if (string.Equals(SceneManager.GetActiveScene().name, "Main Menu"))
        {
            //Если вызвалась функция сохранения игры в главном меню, то это будет означать начать новую игру.
            Serializator.SaveXml(null, datapath);
            SaveInfo sInfo = savesInfo.saves.Find(x => string.Equals(x.saveName,datapath.Substring(datapath.LastIndexOf("/")+1).Substring(0, datapath.Substring(datapath.LastIndexOf("/") + 1).Length-4)));
            sInfo.saveTime = GameStatistics.GetDefaultTime();
            Serializator.SaveXmlSavesInfo(savesInfo, savesInfoPath);
            PlayerPrefs.SetString("SaveDatapath", datapath);
            mainMenu.NewGame();
        }
        else
        {
            //В ином случае мы делаем сохранение в игре
            GameStatistics gameStats = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameStatistics>();
            Serializator.SaveXml(gameStats.GetGameData(), datapath);
            SaveInfo sInfo = savesInfo.saves.Find(x => string.Equals(x.saveName, datapath.Substring(datapath.LastIndexOf("/") + 1).Substring(0, datapath.Substring(datapath.LastIndexOf("/") + 1).Length - 4)));
            sInfo.saveTime = GameTime.TimeString();
            Serializator.SaveXmlSavesInfo(savesInfo, savesInfoPath);
            PlayerPrefs.SetString("SaveDatapath", datapath);
        }

    }

    /// <summary>
    /// Загрузиться по данному пути
    /// </summary>
    protected void Load(string datapath)
    {
        PlayerPrefs.SetString("SaveDatapath", datapath);
#if UNITY_EDITOR
        SceneManager.LoadScene("ProjectTower");
#endif
        Application.LoadLevel("ProjectTower");
    }

    /// <summary>
    /// Начать создание нового сохранения
    /// </summary>
    public void OpenCreateNewWindow(bool yes)
    {
        createNewFadePanel.SetActive(yes);
    }

    /// <summary>
    /// Создать новые сохраняемые игровые данные
    /// </summary>
    public void CreateNew()
    {
        if ((saveNameInputPanel.text != string.Empty) && (!savesInfo.saves.ConvertAll<string>(x => x.saveName).Contains(saveNameInputPanel.text)))
        {
            SaveInfo sInfo = new SaveInfo(saveNameInputPanel.text, GameStatistics.GetDefaultTime());
            savesInfo.saves.Add(sInfo);
            Serializator.SaveXmlSavesInfo(savesInfo, savesInfoPath);
            GameObject _saveButton = Instantiate(saveButton, createNewSaveButton.transform.position, saveButton.transform.rotation) as GameObject;
            _saveButton.transform.SetParent(savesPanel);
            _saveButton.transform.localScale = new Vector3(1f, 0.17f, 1f);
            _saveButton.GetComponent<SaveButton>().Initialize(this, sInfo.saveName, sInfo.saveTime);
            _saveButton.GetComponent<SaveButton>().SetImage(oldSaveImage);
            createNewSaveButton.transform.localPosition += new Vector3(0f, offsetY, 0f);
            createNewFadePanel.SetActive(false);
            Save(savePath + saveNameInputPanel.text+".xml");
        }
    }

}
