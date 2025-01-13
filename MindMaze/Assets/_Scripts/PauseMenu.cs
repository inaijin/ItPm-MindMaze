using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject currentCanv;
    public GameObject weapon;
    public GameManager gameManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        currentCanv.SetActive(true);
        weapon.SetActive(true);
        gameManager.SetCursorIcon();
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        currentCanv.SetActive(false);
        weapon.SetActive(false);
        gameManager.SetCursorDefault();
        Time.timeScale = 0;
        gameIsPaused = true;
    }

    public void loadMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void quiteGame()
    {
        Application.Quit();
    }
}
