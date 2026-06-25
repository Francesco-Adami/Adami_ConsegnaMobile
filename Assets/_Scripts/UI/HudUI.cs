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

        if (GridManager.Instance != null)
        {
            GridManager.Instance.OnResetLastMove += EnableUndoButton;
        }

        undoButton.interactable = false;
    }

    private void OnDisable()
    {
        undoButton.onClick.RemoveListener(UndoMove);
        resetBoardButton.onClick.RemoveListener(ResetLevel);
        nextLevelButton.onClick.RemoveListener(NextLevel);

        if (GridManager.Instance != null)
        {
            GridManager.Instance.OnResetLastMove -= EnableUndoButton;
        }
    }

    private void EnableUndoButton()
    {
        undoButton.interactable = true;
    }

    private void NextLevel()
    {
        GameManager.Instance.StartGameLevel(GameManager.Instance.CurrentLevelIndex + 1);
    }

    private void UndoMove()
    {
        if (GridManager.Instance.UndoLastMove())
        {
            // siccome ho solamente una mossa salvata disattivo il button
            undoButton.interactable = false;
        }
    }

    private void ResetLevel()
    {
        GridManager.Instance.ResetLevel();
    }
}
