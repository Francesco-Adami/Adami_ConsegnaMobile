using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinUI : BaseUI
{
    [Header("Win")]
    [SerializeField] private Button NextLevelButton;
    [SerializeField] private Button QuitButton;

    private void OnEnable()
    {
        Time.timeScale = 0.0f;

        if (!GameManager.Instance.HasMoreLevels())
        {
            NextLevelButton.interactable = false;
        }
        else
        {
            NextLevelButton.interactable = true;
        }

        NextLevelButton.onClick.AddListener(HandleNextLevel);
        QuitButton.onClick.AddListener(HandleQuit);
    }

    private void OnDisable()
    {
        Time.timeScale = 1.0f;

        NextLevelButton.onClick.RemoveListener(HandleNextLevel);
        QuitButton.onClick.RemoveListener(HandleQuit);
    }

    private void HandleNextLevel()
    {
        GameManager.Instance.StartGameLevel(GameManager.Instance.CurrentLevelIndex + 1);
        UIManager.Instance.ShowUI(GameUI.Hud);
    }

    private void HandleQuit()
    {
        UIManager.Instance.ShowUI(GameUI.MainMenu);
    }
}
