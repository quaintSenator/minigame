using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapReader : Singleton<MapReader>
{
    [SerializeField] private TilemapSaveLocalFile selectedMapdata;
    [SerializeField] private int selectedMapIndex = 1;
    
    private Transform mapParent;
    public List<BuildableInfo> buildableInfos = new List<BuildableInfo>();
    private Dictionary<Vector3Int, BuildableBase> currentBuildableMap = new Dictionary<Vector3Int, BuildableBase>();

    [SerializeField] private bool showAllBuildable = false;
    [SerializeField] private MusicManager musicManager;
    
    protected override void OnAwake()
    {
        //当前系统时间
        float time = Time.realtimeSinceStartup;
        
        mapParent = transform;
        BuildableBase.BuildableGroupMap.Clear();
        LoadSelectedData();
        StartCoroutine(CheckBuildableVisibleCoroutine());
        UnityEngine.Application.targetFrameRate = 60;
        UnityEngine.QualitySettings.vSyncCount = 1;
        
        //加载地图数据所需时间
        Debug.Log("Load map data time: " + (Time.realtimeSinceStartup - time));
    }

    private void LoadSelectedData()
    {
        MapData mapData = null;
        string dataString = PlayerPrefs.GetString(GameConsts.CURRENT_SELECTED_MAPDATA);
        if (dataString != "")
        {
            Debug.Log("Load selected map data from PlayerPrefs");
            mapData = JsonUtility.FromJson<MapData>(dataString);
        }
        if(SceneManager.GetActiveScene().name == "LevelForMapEditor")
        {
            dataString = PlayerPrefs.GetString(GameConsts.UGC_SELECTED_MAPDATA);
            if (dataString != "")
            {
                Debug.Log("Load selected map data from PlayerPrefs");
                mapData = JsonUtility.FromJson<MapData>(dataString);
                string musicName = mapData.musicName;
                if (musicName != "")
                {
                    musicManager.SetLevelIndex(musicName);
                }
            }
        }
        else if (selectedMapdata != null)
        {
            Debug.Log("Load selected map data from selectedMapdata : " + selectedMapdata.mapData);
            mapData = JsonUtility.FromJson<MapData>(selectedMapdata.mapData);
        }
        if (mapData != null)
        {
            ReadDataFromBuildableInfos(mapData.buildableInfos);
            //EventManager.InvokeEvent(EventType.EndLoadMapEvent, new EventData());
            
            //待删除，放到角色初始化中
            //EventManager.InvokeEvent(EventType.StartLevelEvent, new EventData());
            EventManager.InvokeEvent(EventType.EndLoadMapEvent, new LoadMapDataEvent(selectedMapIndex));
            
            Debug.Log("Mapdata loaded!");
        }
        else
        {
            Debug.Log("No mapdata loaded!");
        }
    }
    
    private void ReadDataFromBuildableInfos(List<BuildableInfo> infos)
    {
        foreach (var buildableInfo in infos)
        {
            buildableInfos.Add(new BuildableInfo(buildableInfo));
        }
        CheckBuildableVisible();
    }
    
    private void SpawnBuildable(BuildableType type, Vector3Int position, int index, int rotation)
    {
        if (!currentBuildableMap.ContainsKey(position))
        {
            BuildableBase buildable = BuildableBase.SpawnBuildable(type, position, index, rotation, mapParent);
            currentBuildableMap.Add(position, buildable);
        }
    }
    
    private void DestoryBuildable(Vector3Int position)
    {
        if (currentBuildableMap.ContainsKey(position))
        {
            BuildableBase.DestroyBuildable(currentBuildableMap[position]);
            currentBuildableMap.Remove(position);
        }
    }
    
    //协程，每隔一段时间刷新一次地图，检查是否在摄像机视野内
    private IEnumerator CheckBuildableVisibleCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(GameConsts.TILE_CHECK_GAP);
        while (true)
        {
            
            CheckBuildableVisible();
            yield return wait;
        }
    }
    
    private void CheckBuildableVisible()
    {
        foreach (var buildableInfo in buildableInfos)
        {
            if (currentBuildableMap.ContainsKey(buildableInfo.position) && currentBuildableMap[buildableInfo.position] == null)
            {
                currentBuildableMap.Remove(buildableInfo.position);
            }
            
            if (showAllBuildable || Utils.IsAlwaysVisible(buildableInfo.type) || Utils.IsBuildableViewport(buildableInfo.position, Camera.main))
            {
                SpawnBuildable(buildableInfo.type, buildableInfo.position, buildableInfo.index, buildableInfo.rotation);
            }
            else
            {
                DestoryBuildable(buildableInfo.position);
            }
        }
            
        BuildableBase.LinkAllGroup();
    }

    [Button]
    public void PrintFrameRate()
    {
        Debug.Log(UnityEngine.QualitySettings.vSyncCount);
        Debug.Log(UnityEngine.Application.targetFrameRate);
    }
    
    [Button]
    public void PrintMap()
    {
        foreach (var VARIABLE in currentBuildableMap)
        {
            Debug.Log(VARIABLE.Key + " : " + VARIABLE.Value.Type.ToString());
        }
    }
         
}

public class LoadMapDataEvent : EventData
{
    public int index;
    
    public LoadMapDataEvent(int index)
    {
        this.index = index;
    }
}
