using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BSPController))]
public class BSPControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BSPController controller = target as BSPController;

        if (GUILayout.Button("Generate Dungeon"))
        {
            controller.GenerateMap();
        }

        if (GUILayout.Button("Clear Dungeon"))
        {
            controller.ClearMap();
        }
    }

}