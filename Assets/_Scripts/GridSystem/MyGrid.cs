using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SandwichComponentType
{
    Bread,
    lettuce,
    Tomato,
    Meat,
    Cheese
}

public interface ISandwichComponent
{
    SandwichComponentType GetType();
}

public enum Direction
{
    None,
    Right,
    Left,
    Up,
    Down,
}

public class MyGrid
{
    public MyGrid()
    {
        _grid = new Dictionary<Vector2Int, List<ISandwichComponent>>();
    }

    // Dati della griglia
    // posizioni e storage di componenti del sandwich
    public Dictionary<Vector2Int, List<ISandwichComponent>> _grid = new Dictionary<Vector2Int, List<ISandwichComponent>>();

    #region PUBLIC API
    public void MoveComponentsToDir(Vector2Int startPos, Direction dir)
    {
        if (dir == Direction.None || !IsDirectionValid(startPos, dir))
        {
            Debug.LogWarning("direction not valid");
            return;
        }

        switch (dir)
        {
            case Direction.Right:
                MoveComponentsTo(
                    startPos,
                    new(startPos.x + 1, startPos.y)
                );
                break;

            case Direction.Left:
                MoveComponentsTo(
                    startPos,
                    new(startPos.x - 1, startPos.y)
                );
                break;

            case Direction.Up:
                MoveComponentsTo(
                    startPos,
                    new(startPos.x, startPos.y + 1)
                );
                break;

            case Direction.Down:
                MoveComponentsTo(
                    startPos,
                    new(startPos.x, startPos.y - 1)
                );
                break;
        }
    }

    /// <summary>
    /// Aggiunge componenti in una posizione specifica della griglia.
    /// Serve anche per generare automaticamente i livelli
    /// </summary>
    public void AddComponentsOnPos(Vector2Int pos, List<ISandwichComponent> componentsToAdd)
    {
        if (!_grid.ContainsKey(pos))
        {
            _grid[pos] = new List<ISandwichComponent>();
        }

        _grid[pos].AddRange(componentsToAdd);
    }

    /// <summary>
    /// Rimuove componenti in una posizione specifica della griglia.
    /// Serve anche per generare automaticamente i livelli
    /// </summary>
    public void RemoveComponentsOnPos(Vector2Int pos, List<ISandwichComponent> componentsToRemove)
    {
        if (!_grid.ContainsKey(pos))
        {
            Debug.LogError($"grid does not contain key: {pos}");
            return;
        }

        foreach (ISandwichComponent component in componentsToRemove)
        {
            _grid[pos].Remove(component);
        }
    }

    public void PrintGridValues()
    {
        foreach (var kvp in _grid)
        {
            Vector2Int pos = kvp.Key;

            // mi prendo la lista dei componenti presenti in quella pos
            List<ISandwichComponent> components = kvp.Value;

            // stringa ordinata dei tipi di componenti presenti nella posizione
            string componentTypes = string.Join(", ", components.ConvertAll(c => c.GetType().ToString()));

            Debug.Log($"Position: {pos}, Components: [{componentTypes}]");
        }
    }

    public void PrintGridValuesAtPosition(Vector2Int pos)
    {
        if (!_grid.ContainsKey(pos))
        {
            Debug.LogError($"grid does not contain key: {pos}");
            return;
        }
        List<ISandwichComponent> components = _grid[pos];
        string componentTypes = string.Join(", ", components.ConvertAll(c => c.GetType().ToString()));
        Debug.Log($"Position: {pos}, Components: [{componentTypes}]");
    }
    #endregion

    #region HELPERS
    private void MoveComponentsTo(Vector2Int startPos, Vector2Int endPos)
    {
        // salvo gli elementi che ho in startPos
        List<ISandwichComponent> componentsToMove = _grid[startPos];

        // creo lista invertita
        List<ISandwichComponent> invertedComponentsToMove = new List<ISandwichComponent>(componentsToMove);
        invertedComponentsToMove.Reverse();

        // aggiungo la lista invertita a endPos
        AddComponentsOnPos(endPos, invertedComponentsToMove);

        // rimuovo la lista da startPos
        RemoveComponentsOnPos(startPos, componentsToMove);
    }

    private bool IsDirectionValid(Vector2Int startPos, Direction dir)
    {
        return true;
    }
    #endregion
}
