using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_GridData))]
public class GridDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SO_GridData gridData = (SO_GridData)target;

        #region CONTROLLO VALORI ROWS/COLS CAMBIATI
        // Start tracking changes in the Inspector
        EditorGUI.BeginChangeCheck();

        // Renamed labels: 'rows' acts as X (Width), 'cols' acts as Y (Height)
        gridData.rows = EditorGUILayout.IntField("Width (X)", gridData.rows);
        gridData.cols = EditorGUILayout.IntField("Height (Y)", gridData.cols);

        // Prevent negative or zero sizes to avoid errors
        gridData.rows = Mathf.Max(1, gridData.rows);
        gridData.cols = Mathf.Max(1, gridData.cols);

        // If width or height changed, resize the array
        if (EditorGUI.EndChangeCheck() || gridData.gridCells == null || gridData.gridCells.Length != gridData.rows * gridData.cols)
        {
            gridData.ValidateGrid();
            // Marks the ScriptableObject as modified so Unity saves it
            EditorUtility.SetDirty(gridData);
        }
        #endregion

        GUILayout.Space(15);
        GUILayout.Label("Grid Layout", EditorStyles.boldLabel);

        // Draw the grid from top to bottom (max Y down to 0) to match Cartesian coordinates
        for (int y = gridData.cols - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();

            // Add a flexible space to center the grid
            GUILayout.FlexibleSpace();

            // Draw from left to right (X from 0 to max)
            for (int x = 0; x < gridData.rows; x++)
            {
                // This index perfectly matches i * cols + j from your generator
                int index = x * gridData.cols + y;

                GUI.color = GetColorByType(gridData.gridCells[index]);
                // Draw a dropdown for each cell
                gridData.gridCells[index] = (SandwichComponentType)EditorGUILayout.EnumPopup(
                    gridData.gridCells[index],
                    GUILayout.Width(70)
                );
            }
            GUI.color = Color.white;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        // Save changes if the user modified any dropdown
        if (GUI.changed)
        {
            EditorUtility.SetDirty(gridData);
        }
    }

    private Color GetColorByType(SandwichComponentType sandwichComponentType)
    {
        Color color = Color.white;

        switch (sandwichComponentType)
        {
            case SandwichComponentType.None:
                color = Color.gray5;
                break;
            case SandwichComponentType.Bread:
                color = Color.sandyBrown;
                break;
            case SandwichComponentType.lettuce:
                color = Color.lightGreen;
                break;
            case SandwichComponentType.Tomato:
                color = Color.mediumVioletRed;
                break;
            case SandwichComponentType.Meat:
                color = Color.brown;
                break;
            case SandwichComponentType.Cheese:
                color = Color.yellow;
                break;
            default:
                break;
        }

        return color;
    }
}