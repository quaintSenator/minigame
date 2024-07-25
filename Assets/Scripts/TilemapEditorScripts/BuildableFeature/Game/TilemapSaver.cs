using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSaver : Singleton<TilemapSaver>
{
    private Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();

    private string tempSave = "";

    protected override void OnAwake()
    {
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
                        tilemapData.tileInfos.Add(new TileInfo(tile, place));
                    }
                }
            }
        }
        tempSave = JsonUtility.ToJson(tilemapData);
        Debug.Log(tempSave);
    }
    
    public void LoadTilemap()
    {
        if (tempSave != "")
        {
            TilemapData tilemapData = JsonUtility.FromJson<TilemapData>(tempSave);
            foreach (var tilemap in tilemaps)
            {
                Tilemap tilemapComponent = tilemap.Value;
                foreach (var tileInfo in tilemapData.tileInfos)
                {
                    tilemapComponent.SetTile(tilemapComponent.WorldToCell(tileInfo.wolrdPosition), tileInfo.tile);
                }
            }
        }
    }
}

[Serializable]
public class TilemapData
{
    public string tilemapName;
    public List<TileInfo> tileInfos = new List<TileInfo>();
}

[Serializable]
public class TileInfo
{
    public TileBase tile;
    public Vector3 wolrdPosition;
    public TileInfo(TileBase tile, Vector3 position)
    {
        this.tile = tile;
        this.wolrdPosition = position;
    }
}
