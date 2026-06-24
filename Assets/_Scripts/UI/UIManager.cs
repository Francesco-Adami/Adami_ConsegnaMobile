using System;
using System.Buffers.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum GameUI
{
    None,
    MainMenu,
    Hud,
    Win,
    Lose
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameUI startingGameUI = GameUI.MainMenu;

    private Dictionary<GameUI, BaseUI> GameUIDict;
    private GameUI currentGameUI;

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

        InitializeDict();

        ShowUI(startingGameUI);
    }

    private void InitializeDict()
    {
        BaseUI[] uiPanels = GetComponentsInChildren<BaseUI>(true);

        GameUIDict = new Dictionary<GameUI, BaseUI>();

        foreach (var panel in uiPanels)
        {
            GameUIDict[panel.GetUIType()] = panel;
            panel.gameObject.SetActive(false);
        }
    }

    public void ShowUI(GameUI uiToShow)
    {
        if (!GameUIDict.ContainsKey(uiToShow))
        {
            Debug.LogWarning($"UI doesn't have this type of UI: {uiToShow}");
            return;
        }

        // evito di cambiare la UI con lo stesso tipo di quello attivo
        if (currentGameUI == uiToShow) return;

        if (currentGameUI != GameUI.None)
        {
            GameUIDict[currentGameUI].gameObject.SetActive(false);
        }
        GameUIDict[uiToShow].gameObject.SetActive(true);

        currentGameUI = uiToShow;
    }
}
