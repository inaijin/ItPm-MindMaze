using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGame : MonoBehaviour
{
    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        // Quits the game
        Debug.Log("Game is exiting");
        Application.Quit();

        // Note: The quit functionality will only work in a built application, 
        // not in the Unity Editor. Use Debug.Log to confirm functionality while testing in the Editor.
    }
}
