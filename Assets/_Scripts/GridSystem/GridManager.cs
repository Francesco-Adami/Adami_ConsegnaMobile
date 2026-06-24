using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private SO_GridElementsPrefab gridElements;

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
    }

    public void UndoLastMove()
    {
        grid.UndoLastMove();
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
        grid = new MyGrid();

        var elements = FindObjectsByType<GridElement>(sortMode: FindObjectsSortMode.None);

        foreach (var element in elements)
        {
            if (element != null)
            {
                Destroy(element.gameObject);
            }
        }
    }
    #endregion
}
