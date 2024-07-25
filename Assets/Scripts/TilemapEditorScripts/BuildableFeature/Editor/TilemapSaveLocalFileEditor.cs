using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilemapSaveLocalFile))]
public class TilemapSaveLocalFileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TilemapSaveLocalFile scriptableObject = (TilemapSaveLocalFile)target;

        GUILayout.Space(50);
        GUIStyle centeredStyle = GUI.skin.GetStyle("Button");
        centeredStyle.alignment = TextAnchor.MiddleCenter;
        centeredStyle.fontSize = 20;
        EditorGUILayout.BeginHorizontal();
        float inspectorWidth = EditorGUIUtility.currentViewWidth;
        GUILayout.Space(inspectorWidth / 2 - 100);
        if (GUILayout.Button("读取该地图", centeredStyle, GUILayout.Height(50), GUILayout.Width(150)))
        {
            TilemapEditor.LoadTilemaps(scriptableObject);
        }
        EditorGUILayout.EndHorizontal();
    }
}
