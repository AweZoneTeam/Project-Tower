using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuWindow : MenuWindow
{

    /// <summary>
    /// Продолжить игру
    /// </summary>
    public void ContinueGame()
    {
        if (string.Equals(Application.loadedLevelName, "Main Menu"))
        {
#if UNITY_EDITOR
            SceneManager.LoadScene("ProjectTower");
#endif
            Application.LoadLevel("ProjectTower");
        }
        else
        {
            SpFunctions.Pause("menu");
        }
    }


    /// <summary>
    /// Новая игра
    /// </summary>
    public void NewGame()
    {
#if UNITY_EDITOR
        SceneManager.LoadScene("ProjectTower");
#endif
        Application.LoadLevel("ProjectTower");
    }

    /// <summary>
    /// Загрузить игру
    /// </summary>
    public void LoadGame()
    {
    }

    /// <summary>
    /// Перейти на уровень с заданным названием
    /// </summary>
    /// <param name="levelName"></param>
    public void GoToTheLevel(string levelName)
    {
#if UNITY_EDITOR
            SceneManager.LoadScene(levelName);
#endif
            Application.LoadLevel(levelName);
    }

    public void Quit()
    {
        Application.Quit();
    }

}

