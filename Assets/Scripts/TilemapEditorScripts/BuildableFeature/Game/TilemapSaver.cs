using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSaver : Singleton<TilemapSaver>
{
    private Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();

    private Dictionary<string,TilemapData> allSaveMapsDic;
    
    public List<SaveBridgeData> allSaveMapsList;

    protected override void OnAwake()
    {
        allSaveMapsDic = new Dictionary<string, TilemapData>();
        string data = PlayerPrefs.GetString(GameConsts.TILEMAP_SAVE_DATA);
        if (data != "")
        {
            allSaveMapsList = JsonUtility.FromJson<List<SaveBridgeData>>(data);
        }
        else
        {
            allSaveMapsList = new List<SaveBridgeData>();
        }
        foreach (var saveData in allSaveMapsList)
        {
            allSaveMapsDic.Add(saveData.key, saveData.tilemapData);
        }
        initTilemaps();
    }

    public void initTilemaps()
    {
        Transform tilemapParent = GameObject.Find("all_tilemaps").transform;
        foreach (Transform child in tilemapParent)
        {
            Tilemap tilemap = child.GetComponent<Tilemap>();
            if (tilemap != null && child.name != "preview" && child.name != "background")
            {
                tilemaps.Add(child.name, tilemap);
                Debug.Log("Add tilemap: " + child.name);
            }
        }
    }
    
    public void SaveTilemap()
    {
        TilemapData tilemapData = new TilemapData();
        foreach (var tilemap in tilemaps)
        {
            Tilemap tilemapComponent = tilemap.Value;
            BoundsInt bounds = tilemapComponent.cellBounds;
            TileBase[] allTiles = tilemapComponent.GetTilesBlock(bounds);
            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    TileBase tile = allTiles[x + y * bounds.size.x];
                    if (tile != null)
                    {
                        Vector3Int localPlace = (new Vector3Int(bounds.xMin, bounds.yMin, bounds.z) + new Vector3Int(x, y, 0));
                        Vector3 place = tilemapComponent.CellToWorld(localPlace);
                        tilemapData.tileInfos.Add(new TileInfo(tilemap.Key, tile, place));
                    }
                }
            }
        }
        string key = (allSaveMapsDic.Count + 1).ToString();
        allSaveMapsDic.Add(key, tilemapData);
        allSaveMapsList.Add(new SaveBridgeData(key, tilemapData));
        PlayerPrefs.SetString(GameConsts.TILEMAP_SAVE_DATA, JsonUtility.ToJson(allSaveMapsList));
    }
    
    public void LoadTilemap(string key)
    {
        if (key != null)
        {
            TilemapData tilemapData = allSaveMapsDic[key];
            foreach (var tileInfo in tilemapData.tileInfos)
            {
                Tilemap tilemapComponent = tilemaps[tileInfo.tilemapName];
                tilemapComponent.SetTile(tilemapComponent.WorldToCell(tileInfo.wolrdPosition), tileInfo.tile);
            }
        }
    }
    
    //在菜单栏 Tools/HZB/ClearAllTilemaps 中调用
    [UnityEditor.MenuItem("Tools/HZB/ClearAllTilemaps")]
    public static void ClearAllTilemaps()
    {
        PlayerPrefs.DeleteKey(GameConsts.TILEMAP_SAVE_DATA);
    }
}

[Serializable]
public class TilemapData
{
    public List<TileInfo> tileInfos = new List<TileInfo>();
}

[Serializable]
public class TileInfo
{
    public string tilemapName;
    public TileBase tile;
    public Vector3 wolrdPosition;
    public TileInfo(string tilemapName, TileBase tile, Vector3 position)
    {
        this.tilemapName = tilemapName;
        this.tile = tile;
        this.wolrdPosition = position;
    }
}

[Serializable]
public class SaveBridgeData
{
    public string key;
    public TilemapData tilemapData;
    public SaveBridgeData(string key, TilemapData tilemapData)
    {
        this.key = key;
        this.tilemapData = tilemapData;
    }
}
