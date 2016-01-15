using UnityEngine;
using System.Collections;

/// <summary>
/// Прикрепите этот скрипт к объекту, и он будет отвечать за все звуки в игре.
/// </summary>
public class SoundManager :  MonoBehaviour 
{
	public AudioSource musicSource;//Интересующий нас источник звука
	public float lowPitchRange=.95f;
	public float highPitchRange=1.05f;

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
	public void RandomizeSfx(AudioSource efxSource, params AudioClip[] clips)
	{
		int randomIndex = Random.Range (0, clips.Length);
		float randomPitch = Random.Range (lowPitchRange, highPitchRange);
		efxSource.pitch = randomPitch;
		efxSource.clip = clips [randomIndex];
		efxSource.Play ();
	}
}
