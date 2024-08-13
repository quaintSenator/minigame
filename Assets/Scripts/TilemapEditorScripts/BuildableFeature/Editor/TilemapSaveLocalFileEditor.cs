using System;
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
            TilemapEditor.LoadTilemaps();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        centeredStyle.fontSize = 15;
        GUILayout.Space(inspectorWidth / 2 - 100);
        if (GUILayout.Button("转换为新数据结构", centeredStyle, GUILayout.Height(50), GUILayout.Width(150)))
        {
            ChangeToNewDataStructure(scriptableObject);
        }
        EditorGUILayout.EndHorizontal();
    }
    
    public static void ChangeToNewDataStructure(TilemapSaveLocalFile scriptableObject)
    {
        string dataString = scriptableObject.mapData;
        var mapData = JsonUtility.FromJson<OldMapData>(dataString);
        List<BuildableInfo> newBuildableInfos = new List<BuildableInfo>();
        foreach (var buildableInfo in mapData.buildableInfos)
        {
            newBuildableInfos.Add(new BuildableInfo(buildableInfo.type, buildableInfo.position, -1));
        }
        MapData newMapData = new MapData(mapData.key, newBuildableInfos);
        string newMapDataString = JsonUtility.ToJson(newMapData);
        scriptableObject.mapData = newMapDataString;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [Serializable]
    public class OldMapData
    {
        public string key; 
        public List<OldBuildableInfo> buildableInfos;
        public OldMapData(string key, List<OldBuildableInfo> buildableInfos)
        {
            this.key = key;
            this.buildableInfos = buildableInfos;
        }
    
        public OldMapData()
        {
            buildableInfos = new List<OldBuildableInfo>();
        }
    }
    
    [Serializable]
    public class OldBuildableInfo
    {
        public BuildableType type;
        public Vector3Int position;
    
        public OldBuildableInfo(BuildableType type, Vector3Int position)
        {
            this.type = type;
            this.position = position;
        }
    
        public OldBuildableInfo(OldBuildableInfo buildableInfo)
        {
            this.type = buildableInfo.type;
            this.position = buildableInfo.position;
        }
    }
}
