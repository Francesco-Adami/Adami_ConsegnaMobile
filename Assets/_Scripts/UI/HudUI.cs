using System;
using UnityEngine;
using UnityEngine.UI;

public class HudUI : BaseUI
{
    [Header("Hud")]
    [SerializeField] private Button undoButton;
    [SerializeField] private Button resetBoardButton;
    [SerializeField] private Button nextLevelButton;

    private void OnEnable()
    {
        undoButton.onClick.AddListener(UndoMove);
        resetBoardButton.onClick.AddListener(ResetLevel);
        nextLevelButton.onClick.AddListener(NextLevel);
    }

    private void OnDisable()
    {
        undoButton.onClick.RemoveListener(UndoMove);
        resetBoardButton.onClick.RemoveListener(ResetLevel);
        nextLevelButton.onClick.RemoveListener(NextLevel);
    }

    private void NextLevel()
    {
        GameManager.Instance.StartGameLevel(GameManager.Instance.CurrentLevelIndex + 1);
    }

    private void UndoMove()
    {
        GridManager.Instance.UndoLastMove();
    }

    private void ResetLevel()
    {
        GridManager.Instance.ResetLevel();
    }
}
