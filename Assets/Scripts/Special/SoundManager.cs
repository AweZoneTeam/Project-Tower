using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Прикрепите этот скрипт к объекту, и он будет отвечать за все звуки в игре.
/// </summary>
public class SoundManager :  MonoBehaviour 
{
	public AudioSource musicSource;//Интересующий нас источник звука
    private float musVolume = 1f, fxVolume = 1f;
    public float MusicVolume {set { musVolume = value; if (musicSource != null){ musicSource.volume = musVolume; } } }
    public float EffectsVolume { set { fxVolume = value; } }

    public float lowPitchRange=.95f;
	public float highPitchRange=1.05f;

    public void Awake()
    {
        Initialize();
    }

    public void Update()
    {
    }

    public void Initialize()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musVolume = PlayerPrefs.GetFloat("MusicVolume");
            if (musicSource!=null)
            {
                musicSource.volume = musVolume;
            }
        }
        if (PlayerPrefs.HasKey("EffectsVolume"))
        {
            fxVolume = PlayerPrefs.GetFloat("EffectsVolume");
        }
    }

	/// <summary>
	/// Проиграть данный клип в данном источнике
	/// </summary>
	public void PlaySingle(AudioSource efxSource, AudioClip clip)
	{
		efxSource.clip = clip;
		efxSource.Play ();
	}

	/// <summary>
	/// Проиграть случайный клип из данного списка и изменить его случайным образом
	/// </summary>
	public void RandomizeSfx(AudioSource efxSource, List<AudioClip> clips)
	{
		int randomIndex = Random.Range (0, clips.Count);
		float randomPitch = Random.Range (lowPitchRange, highPitchRange);
		efxSource.pitch = randomPitch;
		efxSource.clip = clips [randomIndex];
        efxSource.volume = fxVolume;
		efxSource.Play ();
	}

}
