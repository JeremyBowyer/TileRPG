using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameController gc = target as GameController;

        if (GUILayout.Button("Create Battle Grid"))
        {
            gc.grid.CreateGrid();
        }

        if (GUILayout.Button("Delete Battle Grid"))
        {
            gc.grid.ClearGrid();
        }
    }

}