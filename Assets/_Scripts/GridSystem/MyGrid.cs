using System;
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
    void SetNewGridPos(Vector2Int gridPos);
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

        directionsDict = new Dictionary<Direction, Vector2Int>
        {
            [Direction.Right] = new Vector2Int(1, 0),
            [Direction.Left] = new Vector2Int(-1, 0),
            [Direction.Up] = new Vector2Int(0, 1),
            [Direction.Down] = new Vector2Int(0, -1)
        };
    }

    // Dati della griglia
    // posizioni e storage di componenti del sandwich
    public Dictionary<Vector2Int, List<ISandwichComponent>> _grid = new Dictionary<Vector2Int, List<ISandwichComponent>>();

    private Dictionary<Direction, Vector2Int> directionsDict = new Dictionary<Direction, Vector2Int>();

    #region PUBLIC API
    public void MoveComponentsToDir(Vector2Int startPos, Direction dir)
    {
        if (dir == Direction.None || !IsDirectionValid(startPos, dir))
        {
            Debug.LogWarning("direction not valid");
            return;
        }

        MoveComponentsTo(
            startPos,
            startPos + directionsDict[dir]
        );
    }

    /// <summary>
    /// Aggiunge componenti in una posizione specifica della griglia.
    /// Serve anche per generare automaticamente i livelli
    /// </summary>
    public void AddComponentsOnPos(Vector2Int pos, List<ISandwichComponent> componentsToAdd)
    {
        if (!_grid.ContainsKey(pos)) // accade solamente con la generazione dei livelli
        {
            _grid[pos] = new List<ISandwichComponent>();
        }

        _grid[pos].AddRange(componentsToAdd);

        HandleGridPosStack(pos);
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

        // libero tutta la posizione nella griglia perché sposto sempre tutta la pila
        _grid[pos].Clear();
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
        Debug.Log($"startPos: {startPos} - dir: {dir}");

        // direzione valida se:
        // la lista ha nella direzione indicata ha almeno un elemento

        if (_grid.TryGetValue(startPos + directionsDict[dir], out List<ISandwichComponent> endList))
        {
            return endList.Count > 0;
        }

        return false;
    }

    /// <summary>
    /// funzione per modificare i transform di tutti i componenti di uno stack
    /// </summary>
    private void HandleGridPosStack(Vector2Int pos)
    {
        float currentY = 0;

        foreach (var sandwichComponent in _grid[pos])
        {
            // Controlla se il componente è effettivamente un Component di Unity
            if (sandwichComponent is Component unityComponent)
            {
                Transform t = unityComponent.transform;

                t.position = new Vector3(pos.x * 3, currentY, pos.y * 3); // TODO: rimuovere il bruttissimo * 3 con un metodo
                sandwichComponent.SetNewGridPos(pos);

                currentY++;
            }
            else
            {
                Debug.LogWarning("ISandwichComponent non è un Component di Unity!");
            }
        }
    }
    #endregion
}
