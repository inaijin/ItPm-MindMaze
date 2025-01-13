using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    public GameObject mainMenuCanvas; // Reference to the Main Menu canvas
    [SerializeField]
    public GameObject howToPlayCanvas; // Reference to the How To Play canvas

    public void playGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void goToHowToPlay()
    {
        // Disable the Main Menu canvas and enable the How To Play canvas
        mainMenuCanvas.SetActive(false);
        howToPlayCanvas.SetActive(true);
    }

    public void goBackToMainMenu()
    {
        // Disable the How To Play canvas and enable the Main Menu canvas
        howToPlayCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
}