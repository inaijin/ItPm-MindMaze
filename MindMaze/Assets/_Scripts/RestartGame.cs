using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGame : MonoBehaviour
{
    public void restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
