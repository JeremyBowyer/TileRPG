using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovementBlockerController))]
public class MovementBlockerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MovementBlockerController blocker = target as MovementBlockerController;

        if (GUILayout.Button("Initialize"))
        {
            blocker.Init();
        }

        if (GUILayout.Button("Front"))
        {
            blocker.SpawnBlocker(Vector3.forward);
        }

        if (GUILayout.Button("Back"))
        {
            blocker.SpawnBlocker(Vector3.back);
        }

        if (GUILayout.Button("Left"))
        {
            blocker.SpawnBlocker(Vector3.left);
        }

        if (GUILayout.Button("Right"))
        {
            blocker.SpawnBlocker(Vector3.right);
        }

        if (GUILayout.Button("Clear All"))
        {
            blocker.ClearBlockers();
        }

    }
}
