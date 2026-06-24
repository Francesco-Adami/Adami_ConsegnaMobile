using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : BaseUI
{
    [Header("Main Menu")]
    [SerializeField] private Button playLevelButton;
    [SerializeField] private Button playRandomLevelButton;
    [SerializeField] private Button ExitGameButton;

    private void OnEnable()
    {
        playLevelButton.onClick.AddListener(StartLevel);
        playRandomLevelButton.onClick.AddListener(StartRandomLevel);
        ExitGameButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        playLevelButton.onClick.RemoveListener(StartLevel);
        playRandomLevelButton.onClick.RemoveListener(StartRandomLevel);
        ExitGameButton.onClick.RemoveListener(ExitGame);
    }

    private void StartLevel()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null");
        }

        GameManager.Instance.StartGameLevel(0); // TODO: DA ERRORE
    }

    private void StartRandomLevel()
    {
        GameManager.Instance.StartRandomGame();
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
