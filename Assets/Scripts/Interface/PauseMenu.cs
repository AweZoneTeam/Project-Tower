using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour 
{
	public GameObject PauseUI;
	public GameObject HelpUI;

	private bool paused=false;
	private GameObject activeWindow;

	void Start()
	{
		activeWindow = PauseUI;
		HelpUI.SetActive (false);
		PauseUI.SetActive (false);
	}

	void Update	()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			StartCoroutine(Pause());
		}
	
		if (paused)
		{
			activeWindow.SetActive (true);
			Time.timeScale = 0f;
		}
		else
		{
			HelpUI.SetActive(false);
			PauseUI.SetActive (false);
			Time.timeScale = 1f;
		}
	}

	IEnumerator Pause()
	{
		paused = !paused;
		yield return new WaitForSeconds (1f);
	}

	public void Resume()
	{
		paused = false;
	}

	public void HelpMenu()
	{
		PauseUI.SetActive (false);
		HelpUI.SetActive (true);
		activeWindow = HelpUI;
	
	}

	public void Return()
	{
		activeWindow.SetActive (false);
		PauseUI.SetActive (true);
		activeWindow = PauseUI;
	}

	public void Quit()
	{
		Application.Quit ();
		Application.LoadLevel (Application.loadedLevel);
	}
}