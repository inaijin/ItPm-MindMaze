﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursorTexture = null;

    private void Start()
    {
        Time.timeScale = 1;
        SetCursorIcon();
    }

    public void SetCursorIcon()
    {
        Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f), CursorMode.Auto);
    }

    public void SetCursorDefault()
    {
        Cursor.SetCursor(null, new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f), CursorMode.Auto);
    }

    public void RestartLevel()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
