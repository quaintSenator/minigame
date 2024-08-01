using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapReader : Singleton<MapReader>
{
    [SerializeField] private TilemapSaveLocalFile selectedMapdata;
    
    private Transform mapParent;
    private List<BuildableInfo> buildableInfos = new List<BuildableInfo>();
    private Dictionary<Vector3Int, BuildableBase> currentBuildableMap = new Dictionary<Vector3Int, BuildableBase>();
    
    protected override void OnAwake()
    {
        mapParent = transform;
        LoadSelectedData();
        StartCoroutine(CheckBuildableVisibleCoroutine());
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
        else if (selectedMapdata != null)
        {
            Debug.Log("Load selected map data from selectedMapdata : " + selectedMapdata.mapData);
            mapData = JsonUtility.FromJson<MapData>(selectedMapdata.mapData);
        }
        if (mapData != null)
        {
            ReadDataFromBuildableInfos(mapData.buildableInfos);
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
            buildableInfos.Add(new BuildableInfo(buildableInfo.type, buildableInfo.position));
        }
    }
    
    private void SpawnBuildable(BuildableType type, Vector3Int position)
    {
        if (!currentBuildableMap.ContainsKey(position))
        {
            BuildableBase buildable = BuildableBase.SpawnBuildable(type, position, mapParent);
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
            foreach (var buildableInfo in buildableInfos)
            {
                if (BuildableBase.IsBuildableViewport(buildableInfo.position, Camera.main))
                {
                    SpawnBuildable(buildableInfo.type, buildableInfo.position);
                }
                else
                {
                    DestoryBuildable(buildableInfo.position);
                }
            }
            yield return wait;
        }
    }
    
}
