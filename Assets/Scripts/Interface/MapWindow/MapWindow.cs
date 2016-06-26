using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, ответственный за работу окна карты
/// </summary>
public class MapWindow : InterfaceWindow
{
    #region fields

    public List<GameObject> icons = new List<GameObject>();//Список всех иконок, используемых картой
    public GameObject mainHeroIcon;//Иконка главного героя
    private GameObject _mainHeroIcon;//Отслеживаемая метка главного героя

    private List<MapLayer> mapLayers = new List<MapLayer>();//Слои карты
    public List<MapLayer> MapLayers {get { return mapLayers; }}

    private Toggle legendToggle, enemiesToggle, charactersToggle, itemsToggle, doorsToggle, questsToggle;
    private GameObject legendPanel;

    private GameObject currentLayer = null;

    #endregion //fields

    public void Awake()
    {
        Initialize();
    }

    public override void Initialize()
    {

        _mainHeroIcon = null;

        Transform mapPanel = transform.FindChild("MapPanel");
        MapLayer mapLayer = null;
        currentLayer = null;
        for (int i = 0; i < mapPanel.childCount; i++)
        {
            if ((mapLayer = mapPanel.GetChild(i).GetComponent<MapLayer>()) != null)
            {
                mapLayer.Initialize();
                mapLayers.Add(mapLayer);
            }
        }
        if (mapLayers.Count > 0)
        {
            if (!PlayerPrefs.HasKey("MapLayer"))
            {
                PlayerPrefs.SetInt("MapLayer", 0);
            }
            currentLayer = mapLayers[PlayerPrefs.GetInt("MapLayer")].gameObject;
            currentLayer.SetActive(true);
        }

        KeyboardActorController player = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<KeyboardActorController>();
        player.RoomChangedEvent += RoomChangedEvent;//Подписаться на изменение комнат
        RoomChangedEvent(null, new RoomChangedEventArgs(player.GetRoomPosition()));

        #region toggles

        legendToggle = mapPanel.FindChild("ShowLegendToggle").GetComponent<Toggle>();
        legendPanel = mapPanel.FindChild("LegendPanel").gameObject;
        if (!PlayerPrefs.HasKey("LegendToggle"))
        {
            PlayerPrefs.SetInt("LegendToggle", 1);
        }
        legendToggle.isOn = ((PlayerPrefs.GetInt("LegendToggle") == 1));
        ShowLegend();

        Transform hidePanel = mapPanel.FindChild("HidePanel");

        enemiesToggle = hidePanel.FindChild("EnemiesToggle").GetComponent<Toggle>();
        if (!PlayerPrefs.HasKey("EnemiesToggle"))
        {
            PlayerPrefs.SetInt("EnemiesToggle", 1);
        }
        enemiesToggle.isOn = (PlayerPrefs.GetInt("EnemiesToggle") == 1);
        HideIcons("enemies");

        charactersToggle = hidePanel.FindChild("CharactersToggle").GetComponent<Toggle>();
        if (!PlayerPrefs.HasKey("CharactersToggle"))
        {
            PlayerPrefs.SetInt("CharactersToggle", 1);
        }
        charactersToggle.isOn = (PlayerPrefs.GetInt("CharactersToggle") == 1);
        HideIcons("characters");

        itemsToggle = hidePanel.FindChild("ItemsToggle").GetComponent<Toggle>();
        if (!PlayerPrefs.HasKey("ItemsToggle"))
        {
            PlayerPrefs.SetInt("ItemsToggle", 1);
        }
        itemsToggle.isOn = (PlayerPrefs.GetInt("ItemsToggle") == 1);
        HideIcons("items");

        doorsToggle = hidePanel.FindChild("DoorsToggle").GetComponent<Toggle>();
        if (!PlayerPrefs.HasKey("DoorsToggle"))
        {
            PlayerPrefs.SetInt("DoorsToggle", 1);
        }
        doorsToggle.isOn = (PlayerPrefs.GetInt("DoorsToggle") == 1);
        HideIcons("doors");

        questsToggle = hidePanel.FindChild("QuestsToggle").GetComponent<Toggle>();
        if (!PlayerPrefs.HasKey("QuestsToggle"))
        {
            PlayerPrefs.SetInt("QuestsToggle", 1);
        }
        questsToggle.isOn = (PlayerPrefs.GetInt("QuestsToggle") == 1);
        HideIcons("quests");

        #endregion //toggles

    }

    /// <summary>
    /// Проинициализировать карту, используя сохранённые данные
    /// </summary>
    public void AfterInitialize(MapData mInfo)
    {
        MapLayer mLayer;
        foreach (MapLayerData mLayerData in mInfo.mapLayersData)
        {
            mLayer = null;
            mLayer = mapLayers.Find(x => string.Equals(x.layer, mLayerData.layerName));
            if (mLayer != null)
            {
                mLayer.AfterInitialize(mLayerData);
            }
        }
    }

    /// <summary>
    /// Функция, что обновляет карту в той комнате, в которой в данный момент находится песонаж
    /// </summary>
    public void RefreshRoomMap()
    {
        AreaClass nextRoom = SpFunctions.GetCurrentArea();
        if (nextRoom != null)
        {
            mapLayers.Find(x => string.Equals(x.layer, nextRoom.id.plane)).RefreshRoomMap(nextRoom);
        }
        ChangeMainHeroPosition();
    }

    /// <summary>
    /// Указать на карте нынешнее положение главного героя
    /// </summary>
    private void ChangeMainHeroPosition()
    {
        KeyboardActorController player = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<KeyboardActorController>();

        if (_mainHeroIcon != null)
        {
            Destroy(_mainHeroIcon);
        }

        AreaClass room = player.GetRoomPosition();

        MapLayer mapLayer = mapLayers.Find(x => string.Equals(room.id.plane, x.layer));
        _mainHeroIcon = mapLayer.AddMainHero(mainHeroIcon, room, player.transform.position);
    }

    /// <summary>
    /// Показать или скрыть легенду
    /// </summary>
    public void ShowLegend()
    {
        legendPanel.SetActive(legendToggle.isOn);
        PlayerPrefs.SetInt("LegendToggle", legendToggle.isOn ? 1 : 0);
    }

    /// <summary>
    /// Спрятать все иконки определённого типа
    /// </summary>
    public void HideIcons(string iconType)
    {
        if (string.Equals(iconType, "enemies"))
        {
            PlayerPrefs.SetInt("EnemiesToggle", enemiesToggle.isOn ? 1 : 0);
            foreach (MapLayer mapLayer in mapLayers)
            {
                mapLayer.HideIcons(iconType, !enemiesToggle.isOn);
            }
        }
        else if (string.Equals(iconType, "characters"))
        {
            PlayerPrefs.SetInt("CharactersToggle", charactersToggle.isOn ? 1 : 0);
            foreach (MapLayer mapLayer in mapLayers)
            {
                mapLayer.HideIcons(iconType, !charactersToggle.isOn);
            }
        }
        else if (string.Equals(iconType, "items"))
        {
            PlayerPrefs.SetInt("ItemsToggle", itemsToggle.isOn ? 1 : 0);
            foreach (MapLayer mapLayer in mapLayers)
            {
                mapLayer.HideIcons(iconType, !itemsToggle.isOn);
            }
        }
        else if (string.Equals(iconType, "doors"))
        {
            PlayerPrefs.SetInt("DoorsToggle", doorsToggle.isOn ? 1 : 0);
            foreach (MapLayer mapLayer in mapLayers)
            {
                mapLayer.HideIcons(iconType, !doorsToggle.isOn);
            }
        }
        else if (string.Equals(iconType, "quests"))
        {
            PlayerPrefs.SetInt("QuestsToggle", questsToggle.isOn ? 1 : 0);
            foreach (MapLayer mapLayer in mapLayers)
            {
                mapLayer.HideIcons(iconType, !questsToggle.isOn);
            }
        }
    }

    /// <summary>
    /// Функция, что меняет отображаемый слой карты
    /// </summary>
    public void ChangeCurrentLayer(GameObject nextLayer)
    {
        if (currentLayer != null)
        {
            currentLayer.SetActive(false);
        }
        currentLayer = nextLayer;
        currentLayer.SetActive(true);
        PlayerPrefs.SetInt("MapLayer", mapLayers.IndexOf(currentLayer.GetComponent<MapLayer>()));
    }

    #region events

    /// <summary>
    /// Функция, что меняет карту, учитывая полученную информацию, получаемую при прохождении комнат
    /// </summary>
    void RoomChangedEvent(object sender, RoomChangedEventArgs e)
    {
        AreaClass prevRoom = e.PrevRoom;
        AreaClass nextRoom = e.Room;
        if (prevRoom != null)
        {
            mapLayers.Find(x => string.Equals(x.layer, prevRoom.id.plane)).RefreshRoomMap(prevRoom);
        }
        if (nextRoom != null)
        {
            foreach (MapLayer mLayer in mapLayers)
            {
                if (string.Equals(mLayer.layer, nextRoom.id.plane))
                {
                    mLayer.RefreshRoomMap(nextRoom);
                    break;
                }
            }
            //mapLayers.Find(x => string.Equals(x.layer, nextRoom.id.plane)).RefreshRoomMap(nextRoom);
        }
        ChangeMainHeroPosition();
    }

    #endregion //events

}
