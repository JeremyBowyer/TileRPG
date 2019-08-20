using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LockToRect))]
public class LockToRectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LockToRect rect = target as LockToRect;

        if (GUILayout.Button("Lock"))
        {
            rect.Lock();
        }
    }
}
