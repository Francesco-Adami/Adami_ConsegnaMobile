using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameType
{
    Normal,
    Random
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private List<SO_GridData> levels = new List<SO_GridData>();

    private int currentLevelIndex = 0;
    public int CurrentLevelIndex { get { return currentLevelIndex; } }

    public GameType GameType;

    private void Awake()
    {
        #region SINGLETON PATTERN
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        #endregion
    }

    public void StartGameLevel(int index)
    {
        if (levels.Count <= index)
        {
            Debug.LogWarning($"No more level available: {index}");
            return;
        }

        GridManager.Instance.GenerateLevel(levels[index]);

        GameType = GameType.Normal;
        currentLevelIndex = index;

        UIManager.Instance.ShowUI(GameUI.Hud);
    }

    public void StartRandomGame()
    {
        GridManager.Instance.GenerateRandomLevel();

        GameType = GameType.Random;
        currentLevelIndex = 9999; // livello random, non ha indice

        UIManager.Instance.ShowUI(GameUI.Hud);
    }

    public void WinGame()
    {
        FindAnyObjectByType<HudUI>()?.DisableCancelButtons();
        StartCoroutine(WinRoutine());
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(1.0f);

        UIManager.Instance.ShowUI(GameUI.Win);
    }

    public bool HasMoreLevels()
    {
        return currentLevelIndex < levels.Count - 1 && GameType == GameType.Normal;
    }
}