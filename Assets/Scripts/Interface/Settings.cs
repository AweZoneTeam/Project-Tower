using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Окно настроек со всеми функциями
/// </summary>
public class Settings : InterfaceWindow
{
    private Slider musSlider;
    private Slider fxSlider;
    private Toggle tutorToggle;
    private SoundManager sManager;

    public void Awake()
    {
        Initialize();
    }

    public override void Initialize()
    {
        sManager = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<SoundManager>();
        musSlider=transform.FindChild("MusicVolume").GetComponentInChildren<Slider>();
        fxSlider=transform.FindChild("SoundVolume").GetComponentInChildren<Slider>();
        tutorToggle = transform.FindChild("TutorialReset").GetComponent<Toggle>();
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVolume", musSlider.value);
        }
        if (PlayerPrefs.HasKey("EffectsVolume"))
        {
            fxSlider.value = PlayerPrefs.GetFloat("EffectsVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("EffectsVolume",fxSlider.value);
        }
    }

    public void ChangeMusicVolume()
    {
        sManager.MusicVolume = musSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", musSlider.value);
    }

    public void ChangeSoundVolume()
    {
        sManager.EffectsVolume = fxSlider.value;
        PlayerPrefs.SetFloat("EffectsVolume", fxSlider.value);
    }

}
