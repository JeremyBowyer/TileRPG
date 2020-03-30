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
            blocker.SpawnBlocker(Grid.forwardDirection);
        }

        if (GUILayout.Button("Back"))
        {
            blocker.SpawnBlocker(Grid.backwardDirection);
        }

        if (GUILayout.Button("Left"))
        {
            blocker.SpawnBlocker(Grid.leftDirection);
        }

        if (GUILayout.Button("Right"))
        {
            blocker.SpawnBlocker(Grid.rightDirection);
        }

        if (GUILayout.Button("Front Left"))
        {
            blocker.SpawnBlocker(Grid.forwardLeftDirection);
        }

        if (GUILayout.Button("Front Right"))
        {
            blocker.SpawnBlocker(Grid.forwardRightDirection);
        }

        if (GUILayout.Button("Back Left"))
        {
            blocker.SpawnBlocker(Grid.backwardLeftDirection);
        }

        if (GUILayout.Button("Back Right"))
        {
            blocker.SpawnBlocker(Grid.backwardRightDirection);
        }

        if (GUILayout.Button("Clear All"))
        {
            blocker.ClearBlockers();
        }

    }
}
