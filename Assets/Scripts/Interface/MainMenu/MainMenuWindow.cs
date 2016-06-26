using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuWindow : MenuWindow
{

    #region fields

    protected Button continueButton, loadButton;

    #endregion //fields

    public override void Initialize()
    {
        if (string.Equals(SceneManager.GetActiveScene().name, "Main Menu"))
        {
            continueButton = transform.FindChild("ContinueGame").GetComponent<Button>();
            loadButton = transform.FindChild("LoadGame").GetComponent<Button>();
        }
    }

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
    /// Специальная функция, деактивирующая кнопки загрузки и продолжения игры, если нет сохранений игры
    /// </summary>
    /// <param name="yes"></param>
    public void OnlyNewGame(bool yes)
    {
        continueButton.enabled = !yes;
        loadButton.enabled = !yes;
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

