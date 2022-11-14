using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PerlinMapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PerlinMapGenerator mapGen = (PerlinMapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GeneratePerlinMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GeneratePerlinMap();
        }
        //base.OnInspectorGUI();
    }
}
