using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator
{
    int minIngredients;
    int maxIngredients;

    // tipi di ingredienti disponibili (escluso Bread e None)
    private static readonly SandwichComponentType[] ingredientTypes = new SandwichComponentType[]
    {
        SandwichComponentType.lettuce,
        SandwichComponentType.Tomato,
        SandwichComponentType.Meat,
        SandwichComponentType.Cheese,
    };

    public LevelGenerator(int minIngredients = 2, int maxIngredients = 6)
    {
        this.minIngredients = minIngredients;
        this.maxIngredients = maxIngredients;
    }

    /// <summary>
    /// Crea un SO_GridData (livello da giocare) randomico
    /// </summary>
    public SO_GridData GenerateLevel()
    {
        // Numero di ingredienti da piazzare
        int ingredientCount = Random.Range(minIngredients, maxIngredients + 1);

        // Dimensione della griglia: abbastanza grande da contenere tutto
        // 2 pani + ingredienti, con margine
        int totalCells = 2 + ingredientCount;
        int gridSize = Mathf.Max(3, Mathf.CeilToInt(Mathf.Sqrt(totalCells)) + 1);

        // griglia di lavoro
        SandwichComponentType[,] workGrid = new SandwichComponentType[gridSize, gridSize];

        // posizioni occupate
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

        // direzioni per l'espansione (solo le 4 cardinali)
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.right,
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.down
        };

        // piazzo le 2 fette di pane adiacenti
        Vector2Int bread1Pos = new Vector2Int(Random.Range(1, gridSize - 1), Random.Range(1, gridSize - 1));
        workGrid[bread1Pos.x, bread1Pos.y] = SandwichComponentType.Bread;
        occupied.Add(bread1Pos);

        // scelgo una direzione random per la seconda fetta di pane
        Vector2Int bread2Dir = directions[Random.Range(0, directions.Length)];
        Vector2Int bread2Pos = bread1Pos + bread2Dir;

        // se la seconda fetta esce dalla griglia, provo la direzione opposta
        if (!IsInsideGrid(bread2Pos, gridSize))
        {
            bread2Pos = bread1Pos - bread2Dir;
        }

        workGrid[bread2Pos.x, bread2Pos.y] = SandwichComponentType.Bread;
        occupied.Add(bread2Pos);

        // piazzo gli ingredienti collegati
        // Ogni nuovo ingrediente deve essere adiacente ad almeno una cella già occupata
        List<Vector2Int> frontier = new List<Vector2Int>();

        // Aggiungo i vicini delle fette di pane come punti di espansione
        AddFrontier(bread1Pos, directions, gridSize, occupied, frontier);
        AddFrontier(bread2Pos, directions, gridSize, occupied, frontier);

        int placed = 0;
        int maxAttempts = ingredientCount * 10; // sicurezza anti-loop infinito
        int attempts = 0;

        while (placed < ingredientCount && frontier.Count > 0 && attempts < maxAttempts)
        {
            attempts++;

            // scelgo una posizione random dalla frontiera
            int frontierIndex = Random.Range(0, frontier.Count);
            Vector2Int pos = frontier[frontierIndex];
            frontier.RemoveAt(frontierIndex);

            // se già occupata, skippo
            if (occupied.Contains(pos)) continue;

            // scelgo un ingrediente random
            SandwichComponentType ingredient = ingredientTypes[Random.Range(0, ingredientTypes.Length)];

            workGrid[pos.x, pos.y] = ingredient;
            occupied.Add(pos);
            placed++;

            // espando la frontiera
            AddFrontier(pos, directions, gridSize, occupied, frontier);
        }

        // converto in SO_GridData
        SO_GridData gridData = ScriptableObject.CreateInstance<SO_GridData>();
        gridData.rows = gridSize;
        gridData.cols = gridSize;
        gridData.gridCells = new SandwichComponentType[gridSize * gridSize];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                gridData.gridCells[i * gridSize + j] = workGrid[i, j];
            }
        }

        return gridData;
    }

    #region HELPERS
    private bool IsInsideGrid(Vector2Int pos, int gridSize)
    {
        return pos.x >= 0 && pos.x < gridSize && pos.y >= 0 && pos.y < gridSize;
    }

    private void AddFrontier(Vector2Int pos, Vector2Int[] directions, int gridSize,
        HashSet<Vector2Int> occupied, List<Vector2Int> frontier)
    {
        foreach (var dir in directions)
        {
            Vector2Int neighbor = pos + dir;
            if (IsInsideGrid(neighbor, gridSize) && !occupied.Contains(neighbor))
            {
                frontier.Add(neighbor);
            }
        }
    }
    #endregion
}