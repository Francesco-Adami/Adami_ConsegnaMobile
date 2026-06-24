using System;
using System.Collections.Generic;
using UnityEngine;

public enum SandwichComponentType
{
    None,
    Bread,
    lettuce,
    Tomato,
    Meat,
    Cheese,
}

public interface ISandwichComponent
{
    SandwichComponentType GetSandwitchType();
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

public struct PlayerMove
{
    public Direction MoveDirection;
    public Vector2Int StartPos;
    public Vector2Int EndPos;
    public List<ISandwichComponent> SandwichComponents;

    public Direction InverseDirection()
    {
        switch (MoveDirection)
        {
            case Direction.None:
                return Direction.None;
            case Direction.Right:
                return Direction.Left;
            case Direction.Left:
                return Direction.Right;
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            default:
                return Direction.None;
        }
    }
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

    // ultima mossa eseguita
    public PlayerMove lastPlayerMove;

    private Dictionary<Direction, Vector2Int> directionsDict = new Dictionary<Direction, Vector2Int>();

    #region PUBLIC API
    public bool MoveComponentsToDir(GridElement selectedComponent, Direction dir)
    {
        bool win = false;

        // se il component è il pane allora non devono esserci altri elementi sulla board
        // devono essere tutti impilati sulle due fette di pane
        if (selectedComponent.GetSandwitchType() == SandwichComponentType.Bread && !IsBoardClearForBread()) return win;


        // se l'oggetto ha 2 oggetti adiacenti a lui (nelle direzioni opposte) non lo muovo
        // if (IsElementBetweenOthers(selectedComponent, dir)) return;

        // salvo la startPosition per muovere il componente del panino
        Vector2Int startPos = selectedComponent.GetGridPosition();

        if (dir == Direction.None || !IsDirectionValid(startPos, dir))
        {
            Debug.LogWarning("direction not valid");
            return win;
        }

        // se sto spostando una delle due fette di pane e sono arrivato qui ho vinto
        if (selectedComponent.GetSandwitchType() == SandwichComponentType.Bread)
        {
            win = true;
        }

        MoveComponentsTo(
            startPos,
            startPos + directionsDict[dir]
        );

        return win;
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

        // libero gli item dentro la posizione
        // Iterazione al contrario per evitare errori di modifica
        for (int i = componentsToRemove.Count - 1; i >= 0; i--)
        {
            _grid[pos].Remove(componentsToRemove[i]);
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
            string componentTypes = string.Join(", ", components.ConvertAll(c => c.GetSandwitchType().ToString()));

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
        string componentTypes = string.Join(", ", components.ConvertAll(c => c.GetSandwitchType().ToString()));
        Debug.Log($"Position: {pos}, Components: [{componentTypes}]");
    }

    public void UndoLastMove()
    {
        if (lastPlayerMove.MoveDirection == Direction.None) return;

        // dati
        List<ISandwichComponent> invertedComponentsToMove = new(lastPlayerMove.SandwichComponents);
        invertedComponentsToMove.Reverse();

        // rimuovo i componenti 
        RemoveComponentsOnPos(lastPlayerMove.EndPos, invertedComponentsToMove);

        // ri-aggiungo la lista di componenti alla vecchia posizione
        AddComponentsOnPos(lastPlayerMove.StartPos, lastPlayerMove.SandwichComponents);

        HandleGridPosStack(lastPlayerMove.EndPos);
        HandleGridPosStack(lastPlayerMove.StartPos);

        lastPlayerMove = default;
    }
    #endregion

    #region HELPERS
    private void MoveComponentsTo(Vector2Int startPos, Vector2Int endPos)
    {
        // salvo gli elementi che ho in startPos
        List<ISandwichComponent> componentsToMove = new List<ISandwichComponent>(_grid[startPos]);

        // creo lista invertita
        List<ISandwichComponent> invertedComponentsToMove = new List<ISandwichComponent>(componentsToMove);
        invertedComponentsToMove.Reverse();

        // aggiungo la lista invertita a endPos
        AddComponentsOnPos(endPos, invertedComponentsToMove);

        // rimuovo la lista da startPos
        RemoveComponentsOnPos(startPos, componentsToMove);

        // salvo la mossa appena fatta
        lastPlayerMove = new PlayerMove()
        {
            MoveDirection = CalculateDirection(startPos, endPos),
            StartPos = startPos,
            EndPos = endPos,
            SandwichComponents = componentsToMove
        };
    }

    private Direction CalculateDirection(Vector2Int startPos, Vector2Int endPos)
    {
        // differenza tra le due posizioni
        Vector2Int diff = endPos - startPos;

        // Se non c'è stato alcun movimento --> non dovrebbe succedere
        if (diff.x == 0 && diff.y == 0)
        {
            return Direction.None;
        }

        // Confronta i valori assoluti per determinare l'asse dominante
        if (Math.Abs(diff.x) > Math.Abs(diff.y))
        {
            // orizzontale
            return diff.x > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            // verticale
            return diff.y > 0 ? Direction.Up : Direction.Down;
        }
    }

    private bool IsDirectionValid(Vector2Int startPos, Direction dir)
    {
        Debug.Log($"startPos: {startPos} - dir: {dir}");
        Debug.Log($"endPos: {startPos + directionsDict[dir]}");

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
            if (sandwichComponent is GridElement element)
            {
                Transform t = element.transform;

                t.position = new Vector3(pos.x * 3, currentY, pos.y * 3); // TODO: rimuovere il bruttissimo * 3 con un metodo
                sandwichComponent.SetNewGridPos(pos);

                // se è impilato disattivo il collider,
                // non è più lui l'elemento che devo controllare, ma quello al "livello 0"
                if (currentY == 0)
                {
                    element.GetComponent<Collider>().enabled = true;
                }
                else
                {
                    element.GetComponent<Collider>().enabled = false;
                }

                currentY++;
            }
            else
            {
                Debug.LogWarning("ISandwichComponent non è un Component di Unity!");
            }
        }
    }

    /// <summary>
    /// ritorna true se ci sono solamente 2 posti in griglia con elementi stackati, altrimenti false
    /// </summary>
    private bool IsBoardClearForBread()
    {
        int count = 0;

        foreach (var item in _grid)
        {
            if (item.Value.Count > 0)
            {
                count++;
            }
        }

        return count == 2;
    }

    /// <summary>
    /// ritorna true se l'elemento in input è in mezzo tra 2 elementi orizzontalmente e verticalmente
    /// </summary>
    //private bool IsElementBetweenOthers(GridElement selectedComponent, Direction dir)
    //{
    //    // check orizzontale
    //    int adiancentCount = 0;
    //    adiancentCount += CheckItemInDir(selectedComponent, directionsDict[Direction.Right]);
    //    adiancentCount += CheckItemInDir(selectedComponent, directionsDict[Direction.Left]);
    //    if (adiancentCount > 1)
    //    {
    //        return true;
    //    }

    //    // check verticale
    //    adiancentCount = 0;
    //    adiancentCount += CheckItemInDir(selectedComponent, directionsDict[Direction.Up]);
    //    adiancentCount += CheckItemInDir(selectedComponent, directionsDict[Direction.Down]);
    //    if (adiancentCount > 1)
    //    {
    //        return true;
    //    }

    //    return false;
    //}

    //private int CheckItemInDir(GridElement selectedComponent, Vector2Int dir)
    //{
    //    if (!_grid.ContainsKey(selectedComponent.GetGridPosition() + dir))
    //    {
    //        return 0;
    //    }

    //    if (_grid[selectedComponent.GetGridPosition() + dir].Count > 0)
    //    {
    //        return 1;
    //    }

    //    return 0;
    //}
    #endregion
}
