using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using TacticsGame.Obstacle;

[CustomEditor(typeof(ObstacleData_SO))]
public class ObstacleDataEditor : Editor
{
    private const int GridWidth = 10;
    private const int GridHeight = 10;
    private ObstacleData_SO _obstacleData;

    private void OnEnable()
    {
        _obstacleData = (ObstacleData_SO)target;

        if (_obstacleData.blockedTiles == null)
        {
            _obstacleData.blockedTiles = new List<Vector2Int>();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Obstacle Grid", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("Toggle a tile to mark it as blocked.", MessageType.Info);

        EditorGUILayout.Space();

        DrawGrid();

        EditorGUILayout.Space();

        DrawControls();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField($"Blocked Tiles: {_obstacleData.blockedTiles.Count}", EditorStyles.boldLabel);
    }

    /// <summary>
    /// Draws the 10x10 obstacle grid.
    /// </summary>
    private void DrawGrid()
    {
        for (int y = GridHeight - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < GridWidth; x++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);

                bool isBlocked = _obstacleData.blockedTiles.Contains(gridPosition);

                GUI.backgroundColor = isBlocked ? Color.red : Color.white;

                bool newState = GUILayout.Toggle(isBlocked, $"{x},{y}", "Button", GUILayout.Width(55), GUILayout.Height(35));

                GUI.backgroundColor = Color.white;

                if (newState != isBlocked)
                {
                    ToggleTile(gridPosition, newState);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Adds or removes a tile from the blocked tile list.
    /// </summary>
    private void ToggleTile(Vector2Int gridPosition, bool blocked)
    {
        Undo.RecordObject(_obstacleData, "Modify Obstacle Grid");

        if (blocked)
        {
            if (!_obstacleData.blockedTiles.Contains(gridPosition))
            {
                _obstacleData.blockedTiles.Add(gridPosition);
            }
        }
        else
        {
            _obstacleData.blockedTiles.Remove(gridPosition);
        }

        EditorUtility.SetDirty(_obstacleData);
    }

    /// <summary>
    /// Draws buttons for clearing or filling the grid.
    /// </summary>
    private void DrawControls()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear All"))
        {
            ClearAllTiles();
        }

        if (GUILayout.Button("Block All"))
        {
            BlockAllTiles();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ClearAllTiles()
    {
        Undo.RecordObject(_obstacleData, "Clear Obstacles");
        _obstacleData.blockedTiles.Clear();
        EditorUtility.SetDirty(_obstacleData);
    }

    private void BlockAllTiles()
    {
        Undo.RecordObject(_obstacleData, "Block All Tiles");

        _obstacleData.blockedTiles.Clear();

        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                _obstacleData.blockedTiles.Add(new Vector2Int(x, y));
            }
        }

        EditorUtility.SetDirty(_obstacleData);
    }
}
