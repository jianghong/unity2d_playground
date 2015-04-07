using UnityEngine;
using UnityEditor;
using System.Collections;

public class GridSettings : EditorWindow {

    Grid grid;

    public void Init()
    {
        grid = (Grid)FindObjectOfType(typeof(Grid));
    }

    void OnGUI()
    {
        Init();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Gridsettings:");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        grid.color = EditorGUILayout.ColorField(grid.color, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(" Grid Width ");
        grid.width = EditorGUILayout.FloatField(grid.width, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(" Grid Height ");
        grid.height = EditorGUILayout.FloatField(grid.height, GUILayout.Width(50));
        GUILayout.EndHorizontal();
        EditorUtility.SetDirty(grid);
    }
	
}
