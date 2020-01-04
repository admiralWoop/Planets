using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetController))]
public class PlanetControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Recalculate"))
        {
            (target as PlanetController)?.OnValidate();
        }
    }
}