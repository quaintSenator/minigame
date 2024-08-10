using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSaver : Singleton<TilemapSaver>
{
    private bool isInit = false;
    private Dictionary<string, MapData> allSaveMapsDic;
    public List<MapData> allSaveMapsList;
    public Dictionary<Vector3Int, BuildableInfo> currentBuildableInfosDic;

    protected override void OnAwake()
    {
        if (!isInit)
        {
            Init();
        }
    }

    private void OnDisable()
    {
        AutoSaveMap();
    }

    public void Init()
    {
        isInit = true;
        allSaveMapsDic = new Dictionary<string, MapData>();
        string data = PlayerPrefs.GetString(GameConsts.TILEMAP_SAVE_DATA);
        if (data != "")
        {
            allSaveMapsList = JsonUtility.FromJson<SerializeBridge<MapData>>(data).list;
        }
        else
        {
            allSaveMapsList = new List<MapData>();
        }
        foreach (var saveData in allSaveMapsList)
        {
            allSaveMapsDic.Add(saveData.key, saveData);
        }
        currentBuildableInfosDic = new Dictionary<Vector3Int, BuildableInfo>();
    }
    
    public void AddThisBuildable(BuildableType type, Vector3Int position)
    {
        currentBuildableInfosDic.Add(position, new BuildableInfo(type, position));
    }
    
    public void RemoveThisBuildable(Vector3Int position)
    {
        currentBuildableInfosDic.Remove(position);
    }
    
    public void ClearCurrentBuildableInfos()
    {
        currentBuildableInfosDic.Clear();
    }
    
    public void CopyCurrentBuildableInfos(List<BuildableInfo> buildableInfos)
    {
        foreach (var buildableInfo in buildableInfos)
        {
            currentBuildableInfosDic.Add(buildableInfo.position, new BuildableInfo(buildableInfo.type, buildableInfo.position));
        }
    }

    public void SaveTilemap()
    {
        Debug.Log("Save tilemap data key : " + (allSaveMapsList.Count+1));
        string key = (allSaveMapsList.Count+1).ToString();
        List<BuildableInfo> buildableInfos = new List<BuildableInfo>();
        foreach (var buildableInfo in currentBuildableInfosDic.Values)
        {
            buildableInfos.Add(buildableInfo);
        }
        MapData mapData = new MapData(key, buildableInfos);
        allSaveMapsDic.Add(key, mapData);
        allSaveMapsList.Add(mapData);
        string data = JsonUtility.ToJson(new SerializeBridge<MapData>(allSaveMapsList));
        PlayerPrefs.SetString(GameConsts.TILEMAP_SAVE_DATA, data);
        Debug.Log("allSaveMapsList count : " + buildableInfos.Count);
        Debug.Log(data);
    }
    
    public void AutoSaveMap()
    {
        string key = "auto_save";
        List<BuildableInfo> buildableInfos = new List<BuildableInfo>();
        foreach (var buildableInfo in currentBuildableInfosDic.Values)
        {
            buildableInfos.Add(buildableInfo);
        }
        MapData mapData = new MapData(key, buildableInfos);
        PlayerPrefs.SetString(GameConsts.AUTO_TILEMAP_SAVE_DATA, JsonUtility.ToJson(mapData));
        Debug.Log("Auto save tilemap data");
    }
    
    public List<BuildableInfo> LoadTilemap(string key)
    {
        if (key != null)
        {
            if (allSaveMapsDic.ContainsKey(key))
            {
                return allSaveMapsDic[key].buildableInfos;
            }
        }
        return null;
    }
    
    public List<BuildableInfo> GetCurrentBuildableInfos()
    {
        List<BuildableInfo> buildableInfos = new List<BuildableInfo>();
        foreach (var buildableInfo in currentBuildableInfosDic.Values)
        {
            buildableInfos.Add(buildableInfo);
        }

        return buildableInfos;
    }
}

[Serializable]
public class MapData
{
    public string key; 
    public List<BuildableInfo> buildableInfos;
    public MapData(string key, List<BuildableInfo> buildableInfos)
    {
        this.key = key;
        this.buildableInfos = buildableInfos;
    }
    
    public MapData()
    {
        buildableInfos = new List<BuildableInfo>();
    }
}

[Serializable]
public class BuildableInfo
{
    public BuildableType type;
    public Vector3Int position;
    
    public BuildableInfo(BuildableType type, Vector3Int position)
    {
        this.type = type;
        this.position = position;
    }
}

[Serializable]
public class SerializeBridge<T>
{
    [SerializeField] public List<T> list;
    public SerializeBridge(List<T> list)
    {
        this.list = list;
    }
}
