using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSaver : Singleton<TilemapSaver>
{
    private Dictionary<string, MapData> allSaveMapsDic;
    public List<MapData> allSaveMapsList;
    public Transform mapParent;

    protected override void OnAwake()
    {
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
        mapParent = GameObject.Find("map").transform;
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        //AutoSaveTilemap();
#endif
    }
    
    public void SaveTilemap()
    {
        MapData mapData = CopyCurrentTilempydata();
        string key = mapData.key;
        allSaveMapsDic.Add(key, mapData);
        allSaveMapsList.Add(mapData);
        PlayerPrefs.SetString(GameConsts.TILEMAP_SAVE_DATA, JsonUtility.ToJson(new SerializeBridge<MapData>(allSaveMapsList)));
    }

    public MapData CopyCurrentTilempydata()
    {
        List<BuildableInfo> buildableInfos = new List<BuildableInfo>();
        string key = (allSaveMapsList.Count+1).ToString();
        foreach (var buildableInfo in BuildableCreator.Instance.GetCurrentBuildableMap())
        {
            Vector3Int position = buildableInfo.Value.Position;
            BuildableType type = buildableInfo.Value.Type;
            buildableInfos.Add(new BuildableInfo(type, position));
        }
        return new MapData(key, buildableInfos);
    }
    
    public void AutoSaveTilemap()
    {
        MapData mapData = CopyCurrentTilempydata();
        PlayerPrefs.SetString(GameConsts.AUTO_TILEMAP_SAVE_DATA, JsonUtility.ToJson(mapData));
        Debug.Log("Auto save tilemap");
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
