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
        // TilemapData tilemapData = new TilemapData();
        // List<Tilemap> tilemaps = GetAllTilemaps();
        // foreach (var tilemap in tilemaps)
        // {
        //     BoundsInt bounds = tilemap.cellBounds;
        //     TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        //     for (int x = 0; x < bounds.size.x; x++)
        //     {
        //         for (int y = 0; y < bounds.size.y; y++)
        //         {
        //             TileBase tile = allTiles[x + y * bounds.size.x];
        //             if (tile != null)
        //             {
        //                 Vector3Int localPlace = (new Vector3Int(bounds.xMin, bounds.yMin, bounds.z) + new Vector3Int(x, y, 0));
        //                 Vector3 place = tilemap.CellToWorld(localPlace);
        //                 tilemapData.tileInfos.Add(new TileInfo(tilemap.name, tile, place));
        //             }
        //         }
        //     }
        // }
        // // 保存地图数据
        // TilemapSaveLocalFile saveData = ScriptableObject.CreateInstance<TilemapSaveLocalFile>();
        // saveData.saveTime = System.DateTime.Now.ToString();
        // saveData.tilemapData = JsonUtility.ToJson(tilemapData);
        //
        // //读取路径里面有多少文件
        // string path = "Assets/Scripts/TilemapEditorScripts/BuildableFeature/MapForDesigner";
        // string[] files = System.IO.Directory.GetFiles(path);
        // string saveName = "TilemapSaveData_" + (files.Length/2) + ".asset";
        // AssetDatabase.CreateAsset(saveData, path + "/" + saveName);
        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();
        // Debug.Log("Saved tilemaps to Assets/Scripts/TilemapEditorScripts/BuildableFeature/MapForDesigner/TilemapSaveData.asset");
    }
    
    //在菜单栏 Tools/HZB/Get Auto Save File 中调用
    [MenuItem("Tools/HZB/Get Auto Save File")]
    private static void GetAutoSaveFile()
    {
        // string data = PlayerPrefs.GetString(GameConsts.AUTO_TILEMAP_SAVE_DATA);
        // TilemapData tilemapData = JsonUtility.FromJson<TilemapData>(data);
        // // 保存地图数据
        // TilemapSaveLocalFile saveData = ScriptableObject.CreateInstance<TilemapSaveLocalFile>();
        // saveData.saveTime = System.DateTime.Now.ToString();
        // saveData.tilemapData = JsonUtility.ToJson(tilemapData);
        // string path = "Assets/Scripts/TilemapEditorScripts/BuildableFeature/AutoSave";
        // string saveName = "TilemapSaveData_auto" + ".asset";
        // AssetDatabase.CreateAsset(saveData, path + "/" + saveName);
        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();
        // Debug.Log("save as asset");
    }
    
    //选择 TilemapSaveLocalFile.asset 文件，选择菜单栏 Tools/HZB/LoadTilemaps 来调用
    [MenuItem("Tools/HZB/Load Tilemaps")]
    public static void LoadTilemaps()
    {
        // TilemapSaveLocalFile saveData = Selection.activeObject as TilemapSaveLocalFile;
        // if (saveData == null)
        // {
        //     Debug.LogError("Please select a TilemapSaveLocalFile.asset file.");
        //     return;
        // }
        // TilemapData tilemapData = JsonUtility.FromJson<TilemapData>(saveData.tilemapData);
        // foreach (var tileInfo in tilemapData.tileInfos)
        // {
        //     Tilemap tilemap = GameObject.Find(tileInfo.tilemapName).GetComponent<Tilemap>();
        //     tilemap.SetTile(tilemap.WorldToCell(tileInfo.wolrdPosition), tileInfo.tile);
        // }
        // Debug.Log("Loaded tilemaps from " + saveData.name);
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
    
    //在菜单栏 Tools/HZB/Fix BoxCollider2D Size 中调用，调整 BoxCollider2D 的大小为子物体中最大的 SpriteRenderer 的大小
    [MenuItem("Tools/HZB/Fix BoxCollider2D Size")]
    public static void FixBoxCollider2DSize()
    {
        GameObject selectedObject = Selection.activeGameObject;
        GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(selectedObject);

        if (prefabRoot != null)
        {
            BoxCollider2D boxCollider2D = prefabRoot.GetComponent<BoxCollider2D>();
            if (boxCollider2D != null)
            {
                SpriteRenderer[] spriteRenderers = prefabRoot.GetComponentsInChildren<SpriteRenderer>();
                if (spriteRenderers.Length > 0)
                {
                    Bounds bounds = spriteRenderers[0].bounds;
                    foreach (var spriteRenderer in spriteRenderers)
                    {
                        bounds.Encapsulate(spriteRenderer.bounds);
                    }
                    boxCollider2D.size = bounds.size;
                    boxCollider2D.offset = bounds.center - prefabRoot.transform.position;

                    PrefabUtility.ApplyPrefabInstance(prefabRoot, InteractionMode.UserAction);
                    Debug.Log("Fixed BoxCollider2D size and saved to Prefab.");
                }
                else
                {
                    Debug.LogError("No SpriteRenderer found in the selected Prefab.");
                }
            }
            else
            {
                Debug.LogError("Prefab does not contain BoxCollider2D component.");
            }
        }
        else
        {
            Debug.LogError("Please select a Prefab instance.");
        }
    }
}
