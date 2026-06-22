using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private SO_GridElementsPrefab gridElements;

    private MyGrid grid;

    private void Start()
    {
        grid = new MyGrid();

        grid.AddComponentsOnPos(new Vector2Int(0, 0), new List<ISandwichComponent>
        {
            InstantiateGridElement(SandwichComponentType.Bread, new Vector2(0, 0))
        });

        grid.AddComponentsOnPos(new Vector2Int(0, 1), new List<ISandwichComponent>
        {
            InstantiateGridElement(SandwichComponentType.Cheese, new Vector2(0, 1))
        });

        grid.AddComponentsOnPos(new Vector2Int(1, 0), new List<ISandwichComponent>
        {
            InstantiateGridElement(SandwichComponentType.Tomato, new Vector2(1, 0))
        });

        grid.PrintGridValues();
    }

    private GridElement InstantiateGridElement(SandwichComponentType type, Vector2 position)
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

        GridElement prefab = Instantiate(prefabToSpawn);

        return prefab;
    }
}
