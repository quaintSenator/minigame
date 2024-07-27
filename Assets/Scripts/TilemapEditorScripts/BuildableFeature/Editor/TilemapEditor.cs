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
        Transform mapParent = GameObject.Find("map").transform;
        foreach (Transform child in mapParent)
        {
            DestroyImmediate(child.gameObject);
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
        MapData mapData = new MapData();
        Transform mapParent = GameObject.Find("map").transform;
        foreach (Transform child in mapParent)
        {
            Vector3 buildablePos = child.position;
            Vector3 offset = BuildableCreator.GetStartPositionOffset();
            int nearestX = Mathf.RoundToInt((buildablePos.x-offset.x) / GameConsts.TILE_SIZE); // 计算最近的 tile X 坐标
            int nearestY = Mathf.RoundToInt((buildablePos.y-offset.y) / GameConsts.TILE_SIZE); // 计算最近的 tile Y 坐标
            Vector3Int realPos = new Vector3Int(nearestX, nearestY, 0); // 计算最近的 tile 坐标
            BuildableType type = child.GetComponent<BuildableBase>().Type;
            BuildableInfo buildableInfo = new BuildableInfo(type, realPos);
            mapData.buildableInfos.Add(buildableInfo);
        }
        
        // 保存地图数据
        string key = System.DateTime.Now.ToString();
        mapData.key = key;
        TilemapSaveLocalFile saveData = ScriptableObject.CreateInstance<TilemapSaveLocalFile>();
        saveData.saveTime = key;
        saveData.mapData = JsonUtility.ToJson(mapData);
        
        //读取路径里面有多少文件
        string path = "Assets/Scripts/TilemapEditorScripts/BuildableFeature/MapForDesigner";
        string[] files = System.IO.Directory.GetFiles(path);
        string saveName = "MapSaveData_" + (files.Length/2) + ".asset";
        AssetDatabase.CreateAsset(saveData, path + "/" + saveName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Saved tilemaps to Assets/Scripts/TilemapEditorScripts/BuildableFeature/MapForDesigner/MapSaveData.asset");
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
        TilemapSaveLocalFile saveData = Selection.activeObject as TilemapSaveLocalFile;
        if (saveData == null)
        {
            Debug.LogError("Please select a TilemapSaveLocalFile.asset file.");
            return;
        }
        PlayerPrefs.SetString(GameConsts.CURRENT_SELECTED_MAPDATA, saveData.mapData);
        Debug.Log("Loaded tilemaps from " + saveData.name);
    }
    [MenuItem("Tools/HZB/Load Tilemaps", true)]
    public static bool LoadTilemapsValidate()
    {
        return Selection.activeObject != null && Selection.activeObject.GetType() == typeof(TilemapSaveLocalFile);
    }
    
    //在菜单栏 Tools/HZB/Unload Tilemaps 中调用，卸载当前选择的地图数据
    [MenuItem("Tools/HZB/Unload Tilemaps")]
    public static void UnloadTilemaps()
    {
        PlayerPrefs.DeleteKey(GameConsts.CURRENT_SELECTED_MAPDATA);
        Debug.Log("Unloaded current selected map data.");
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
