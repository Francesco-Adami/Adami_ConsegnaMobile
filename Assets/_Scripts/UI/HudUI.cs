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
            // Quando viene eseguita una mossa con successo, abilito Undo e Reset
            GridManager.Instance.AddOnMovePerformed(OnMovePerformedHandler);

            // Quando l'ultima mossa viene resettata (dopo undo), disabilito Undo
            GridManager.Instance.OnResetLastMove += OnResetLastMoveHandler;
        }

        // Tutti e tre i pulsanti partono disabilitati
        undoButton.interactable = false;
        resetBoardButton.interactable = false;

        // Next Level si sblocca solo se esiste un livello successivo
        UpdateNextLevelButton();
    }

    private void OnDisable()
    {
        undoButton.onClick.RemoveListener(UndoMove);
        resetBoardButton.onClick.RemoveListener(ResetLevel);
        nextLevelButton.onClick.RemoveListener(NextLevel);

        if (GridManager.Instance != null)
        {
            GridManager.Instance.RemoveOnMovePerformed(OnMovePerformedHandler);
            GridManager.Instance.OnResetLastMove -= OnResetLastMoveHandler;
        }
    }

    /// <summary>
    /// Chiamato quando uno swipe va a buon fine.
    /// Sblocca Undo e Reset.
    /// </summary>
    private void OnMovePerformedHandler()
    {
        undoButton.interactable = true;
        resetBoardButton.interactable = true;
    }

    /// <summary>
    /// Chiamato quando l'ultima mossa viene cancellata (dopo undo o reset).
    /// Disabilita il pulsante Undo.
    /// </summary>
    private void OnResetLastMoveHandler()
    {
        undoButton.interactable = false;
    }

    private void NextLevel()
    {
        if (GameManager.Instance.HasMoreLevels())
        {
            GameManager.Instance.StartGameLevel(GameManager.Instance.CurrentLevelIndex + 1);
            if (!GameManager.Instance.HasMoreLevels())
            {
                nextLevelButton.interactable = false;
            }
        }
    }

    private void UndoMove()
    {
        if (GridManager.Instance.UndoLastMove())
        {
            // Dopo undo, disattivo il pulsante (una sola mossa salvata)
            undoButton.interactable = false;
        }
    }

    private void ResetLevel()
    {
        GridManager.Instance.ResetLevel();

        // Dopo il reset, disabilito sia Undo che Reset
        // (non ci sono più mosse da annullare)
        undoButton.interactable = false;
        resetBoardButton.interactable = false;
    }

    /// <summary>
    /// Aggiorna lo stato del pulsante Next Level in base ai livelli disponibili.
    /// </summary>
    private void UpdateNextLevelButton()
    {
        if (GameManager.Instance != null)
        {
            nextLevelButton.interactable = GameManager.Instance.HasMoreLevels();
        }
        else
        {
            nextLevelButton.interactable = false;
        }
    }
}
