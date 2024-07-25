using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapEditor : Editor
{
    //在菜单栏 Tools/HZB/ClearAllTilemaps 中调用
    [MenuItem("Tools/HZB/Clear All Tilemaps")]
    public static void ClearAllTilemaps()
    {
        Tilemap[] tilemaps = GameObject.FindObjectsOfType<Tilemap>(); // 获取场景中所有 Tilemap

        foreach (Tilemap tilemap in tilemaps)
        {
            if(tilemap.name == "preview" || tilemap.name == "background")
            {
                continue;
            }
            tilemap.ClearAllTiles(); // 清除当前 Tilemap 的所有地块
        }
        Debug.Log("Cleared all tilemaps in the scene.");
    }
    
    //在菜单栏 Tools/HZB/ClearAllSavePlayerPrefs 中调用
    [MenuItem("Tools/HZB/Clear All Save PlayerPrefs")]
    public static void ClearAllSavePlayerPrefs()
    {
        PlayerPrefs.DeleteKey(GameConsts.TILEMAP_SAVE_DATA);
    }
    
    //在菜单栏 Tools/HZB/SaveTilemaps 中调用
    [MenuItem("Tools/HZB/Save Tilemaps")]
    public static void SaveTilemaps()
    {
        TilemapData tilemapData = new TilemapData();
        List<Tilemap> tilemaps = GetAllTilemaps();
        foreach (var tilemap in tilemaps)
        {
            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    TileBase tile = allTiles[x + y * bounds.size.x];
                    if (tile != null)
                    {
                        Vector3Int localPlace = (new Vector3Int(bounds.xMin, bounds.yMin, bounds.z) + new Vector3Int(x, y, 0));
                        Vector3 place = tilemap.CellToWorld(localPlace);
                        tilemapData.tileInfos.Add(new TileInfo(tilemap.name, tile, place));
                    }
                }
            }
        }
        // 保存地图数据到 Assets/Scripts/TilemapEditorScripts/BuildableFeature/MapForDesigner
        TilemapSaveLocalFile saveData = ScriptableObject.CreateInstance<TilemapSaveLocalFile>();
        saveData.saveTime = System.DateTime.Now.ToString();
        saveData.tilemapData = JsonUtility.ToJson(tilemapData);
        AssetDatabase.CreateAsset(saveData, "Assets/Scripts/TilemapEditorScripts/BuildableFeature/MapForDesigner/TilemapSaveData.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Saved tilemaps to Assets/Scripts/TilemapEditorScripts/BuildableFeature/MapForDesigner/TilemapSaveData.asset");
    }
    
    //选择 TilemapSaveLocalFile.asset 文件，选择菜单栏 Tools/HZB/LoadTilemaps 来调用
    [MenuItem("Tools/HZB/Load Tilemaps")]
    public static void LoadTilemaps(TilemapSaveLocalFile data)
    {
        TilemapSaveLocalFile saveData = data ? data : Selection.activeObject as TilemapSaveLocalFile;
        if (saveData == null)
        {
            Debug.LogError("Please select a TilemapSaveLocalFile.asset file.");
            return;
        }
        TilemapData tilemapData = JsonUtility.FromJson<TilemapData>(saveData.tilemapData);
        foreach (var tileInfo in tilemapData.tileInfos)
        {
            Tilemap tilemap = GameObject.Find(tileInfo.tilemapName).GetComponent<Tilemap>();
            tilemap.SetTile(tilemap.WorldToCell(tileInfo.wolrdPosition), tileInfo.tile);
        }
        Debug.Log("Loaded tilemaps from " + saveData.name);
    }
    [MenuItem("Tools/HZB/Load Tilemaps", true)]
    public static bool LoadTilemapsValidate()
    {
        return Selection.activeObject != null && Selection.activeObject.GetType() == typeof(TilemapSaveLocalFile);
    }

    
    
    
    public static List<Tilemap> GetAllTilemaps()
    {
        Tilemap[] tilemaps = GameObject.FindObjectsOfType<Tilemap>();
        List<Tilemap> tilemapList = new List<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            if (tilemap.name == "preview" || tilemap.name == "background")
            {
                continue;
            }
            tilemapList.Add(tilemap);
        }
        return tilemapList;
    }
}
