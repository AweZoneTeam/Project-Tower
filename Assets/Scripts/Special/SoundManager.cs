using UnityEngine;
using System.Collections;

	public class SoundManager :  MonoBehaviour 
	{
		public AudioSource musicSource;
		public static SoundManager instance=null;
		public float lowPitchRange=.95f;
		public float highPitchRange=1.05f;
	

		public void PlaySingle(AudioSource efxSource, AudioClip clip)
		{
			efxSource.clip = clip;
			efxSource.Play ();
		}

		public void RandomizeSfx(AudioSource efxSource, params AudioClip[] clips)
		{
			int randomIndex = Random.Range (0, clips.Length);
			float randomPitch = Random.Range (lowPitchRange, highPitchRange);
			efxSource.pitch = randomPitch;
			efxSource.clip = clips [randomIndex];
			efxSource.Play ();
		}
	}
