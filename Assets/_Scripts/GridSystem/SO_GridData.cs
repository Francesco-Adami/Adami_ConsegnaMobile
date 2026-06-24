using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "Scriptable Objects/GridData")]
public class SO_GridData : ScriptableObject
{
    public int rows;
    public int cols;

    // devo fare un array 1D per serializzazione
    // Unity non supporta nativamente la serializzazione di array multidimensionali
    [HideInInspector]
    public SandwichComponentType[] gridCells;

    // Adjusts the array size when rows or columns change
    public void ValidateGrid()
    {
        if (gridCells == null || gridCells.Length != rows * cols)
        {
            SandwichComponentType[] newGrid = new SandwichComponentType[rows * cols];

            // Keeps existing data if the grid is resized
            if (gridCells != null)
            {
                int minRows = Mathf.Min(rows, gridCells.Length / (cols == 0 ? 1 : cols));
                int minCols = Mathf.Min(cols, gridCells.Length / (rows == 0 ? 1 : rows));

                for (int r = 0; r < minRows; r++)
                {
                    for (int c = 0; c < minCols; c++)
                    {
                        // To avoid out of bounds in case of weird manual resizing
                        if (r * cols + c < newGrid.Length && r * minCols + c < gridCells.Length)
                        {
                            newGrid[r * cols + c] = gridCells[r * minCols + c];
                        }
                    }
                }
            }
            gridCells = newGrid;
        }
    }
}
