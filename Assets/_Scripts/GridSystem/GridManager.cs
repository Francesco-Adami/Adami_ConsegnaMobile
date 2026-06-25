using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private SO_GridElementsPrefab gridElements;

    public event Action OnMovePerformed;
    public event Action OnResetLastMove;

    private MyGrid grid;
    private SO_GridData currentLevel;

    private void Awake()
    {
        #region Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        #endregion

        grid = new();

        grid.OnResetLastMove += HandleResetLastMove;
    }

    private void HandleResetLastMove()
    {
        OnResetLastMove?.Invoke();
    }

    #region PUBLIC API
    public void TryMove(GridElement selectedGridElement, Direction direction)
    {
        if (grid.MoveComponentsToDir(selectedGridElement, direction))
        {
            Debug.LogError("YOU WIN");
            GameManager.Instance.WinGame();
        }
    }

    public void GenerateLevel(SO_GridData gridData)
    {
        // evito errori stupidi
        if (gridData == null || gridData.gridCells == null) return;

        CleanBoard();
        currentLevel = gridData;

        for (int i = 0; i < gridData.rows; i++)
        {
            for (int j = 0; j < gridData.cols; j++)
            {
                if (NeedToSpawn(gridData, i, j, out SandwichComponentType type))
                {
                    Vector2Int gridPos = new(i, j);

                    // spawno l'oggetto nella posizione [i, j]
                    grid.AddComponentsOnPos(gridPos, new List<ISandwichComponent>
                    {
                        InstantiateGridElement(type, gridPos)
                    });
                }
            }
        }
    }

    public void ResetLevel()
    {
        if (currentLevel == null) return;

        GenerateLevel(currentLevel);
        grid.LastPlayerMove = default;
    }

    /// <summary>
    /// ritorna true se è andato a buon fine, false altrimenti
    /// </summary>
    /// <returns></returns>
    public bool UndoLastMove()
    {
        if (grid.UndoLastMove()) return true;

        return false;
    }

    public void ResetLastMove()
    {
        grid.LastPlayerMove = default;
    }
    #endregion

    #region HELPERS
    private GridElement InstantiateGridElement(SandwichComponentType type, Vector2Int position)
    {
        GridElement element = InstantiateByType(type);
        element.Init(type, position);
        return element;
    }

    private GridElement InstantiateByType(SandwichComponentType type)
    {
        GridElement prefabToSpawn = null;

        switch (type)
        {
            case SandwichComponentType.Bread:
                prefabToSpawn = gridElements.breadPrefab;
                break;
            case SandwichComponentType.lettuce:
                prefabToSpawn = gridElements.lettucePrefab;
                break;
            case SandwichComponentType.Tomato:
                prefabToSpawn = gridElements.tomatoPrefab;
                break;
            case SandwichComponentType.Meat:
                prefabToSpawn = gridElements.meatPrefab;
                break;
            case SandwichComponentType.Cheese:
                prefabToSpawn = gridElements.cheesePrefab;
                break;
            default:
                break;
        }

        GridElement prefab = Instantiate(prefabToSpawn, transform);

        return prefab;
    }

    private bool NeedToSpawn(SO_GridData gridData, int i, int j, out SandwichComponentType type)
    {
        // Converto le coordinate 2D per l'array di gridData
        int index = i * gridData.cols + j;

        // Assegno il type al mio type in out
        type = gridData.gridCells[index];

        // Ritorno true se la cella non è vuota
        return type != SandwichComponentType.None;
    }

    private void CleanBoard()
    {
        grid.Clean();

        var elements = FindObjectsByType<GridElement>(sortMode: FindObjectsSortMode.None);

        foreach (var element in elements)
        {
            if (element != null)
            {
                Destroy(element.gameObject);
            }
        }
    }

    internal void AddOnMovePerformed(Action enableUndoButton)
    {
        grid.OnMovePerformed += enableUndoButton;
    }

    public void RemoveOnMovePerformed(Action enableUndoButton)
    {
        grid.OnMovePerformed -= enableUndoButton;
    }
    #endregion
}
