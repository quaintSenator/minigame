using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;



public enum TileMode
{
    None,
    Build,
    Destroy,
}

public class BuildableCreator : Singleton<BuildableCreator>
{
    [SerializeField] private TilemapSaveLocalFile selectedMapdata;
    
    private BuildableType selectedType = BuildableType.none;
    private BuildableBase previewObj;
    [SerializeField] private Transform mapParent; 
    private TileMode currentTileMode = TileMode.None; 
    
    private Vector3 mousePosition;
    private Vector3Int currentCellPosition;
    private Vector3Int lastCellPosition;

    private List<BuildableInfo> buildableInfos = new List<BuildableInfo>();
    private Dictionary<Vector3Int, BuildableBase> currentBuildableMap = new Dictionary<Vector3Int, BuildableBase>();

    [SerializeField] private bool showAllBuildable = false;
    protected override void OnAwake()
    {
        //唤醒TilemapSaver
        TilemapSaver.Instance.Init();
        
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
            TilemapSaver.Instance.AddThisBuildable(buildableInfo.type, buildableInfo.position);
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.CancelCurrentSelectEvent,CancelCurrentSelect); //取消当前选择
        EventManager.AddListener(EventType.MouseMoveEvent, OnMouseMove); //鼠标移动
        EventManager.AddListener(EventType.DrawOrEraseEvent, OnMouseLeftClick); //绘制或者擦除
        EventManager.AddListener(EventType.ChangeTileModeEvent, ChangeTileMode); //切换擦除
        EventManager.AddListener(EventType.SaveMapEvent, SaveMap); //保存地图
        EventManager.AddListener(EventType.LoadMapOneEvent, LoadMapOne); //加载地图一
        EventManager.AddListener(EventType.LoadMapOneEvent, LoadMapOne); //加载地图一
        EventManager.AddListener(EventType.LoadMapTwoEvent, LoadMapTwo); //加载地图二
        EventManager.AddListener(EventType.LoadMapThreeEvent, LoadMapThree); //加载地图三
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.CancelCurrentSelectEvent,CancelCurrentSelect); //取消当前选择
        EventManager.RemoveListener(EventType.MouseMoveEvent, OnMouseMove); //鼠标移动
        EventManager.RemoveListener(EventType.DrawOrEraseEvent, OnMouseLeftClick); //绘制或者擦除
        EventManager.RemoveListener(EventType.ChangeTileModeEvent, ChangeTileMode); //切换擦除
        EventManager.RemoveListener(EventType.SaveMapEvent, SaveMap); //保存地图
        EventManager.RemoveListener(EventType.LoadMapOneEvent, LoadMapOne); //加载地图一
        EventManager.RemoveListener(EventType.LoadMapOneEvent, LoadMapOne); //加载地图一
        EventManager.RemoveListener(EventType.LoadMapTwoEvent, LoadMapTwo); //加载地图二
        EventManager.RemoveListener(EventType.LoadMapThreeEvent, LoadMapThree); //加载地图三
    }

    private void LoadMapOne(EventData data)
    {
        Debug.Log("Load map data from map one");
        ClearAllTilemaps();
        ReadDataFromBuildableInfos(TilemapSaver.Instance.LoadTilemap(1.ToString()));
        CheckBuildableVisible();
    }
    
    private void LoadMapTwo(EventData data)
    {
        Debug.Log("Load map data from map two");
        ClearAllTilemaps();
        ReadDataFromBuildableInfos(TilemapSaver.Instance.LoadTilemap(2.ToString()));
        CheckBuildableVisible();
    }
    
    private void LoadMapThree(EventData data)
    {
        Debug.Log("Load map data from map three");
        ClearAllTilemaps();
        ReadDataFromBuildableInfos(TilemapSaver.Instance.LoadTilemap(3.ToString()));
        CheckBuildableVisible();
    }

    private void SaveMap(EventData obj)
    {
        if(buildableInfos.Count == 0)
        {
            Debug.Log("No buildable to save");
            return;
        }
        TilemapSaver.Instance.SaveTilemap();
        ClearAllTilemaps();
    }

    private void ChangeTileMode(EventData obj)
    {
        Debug.Log("Change tile mode");
        if(currentTileMode != TileMode.Destroy)
        {
            SetSelectedObject(BuildableType.none);
            currentTileMode = TileMode.Destroy;
        }
        else
        {
            SetSelectedObject(BuildableType.none);
        }
    }

    private void OnMouseLeftClick(EventData data = null)
    {
        if (InputManager.Instance.IsMouseOverUI())
        {
            return;
        }
        
        if (currentTileMode == TileMode.Build)
        {
            DrawTileMap();   
        }
        else if(currentTileMode == TileMode.Destroy)
        {
            EraseTileMap();
        }
    }


    private void CancelCurrentSelect(EventData data)
    {
        SetSelectedObject(BuildableType.none);
    }

    private void OnMouseMove(EventData data)
    {
        UpdateTilemap();
    }
    
    public void SetSelectedObject(BuildableType type)
    {
        selectedType = type;
        currentTileMode = selectedType == BuildableType.none ? TileMode.None : TileMode.Build;
        if (selectedType != BuildableType.none)
        {
            if (previewObj != null)
            {
                BuildableBase.DestroyBuildable(previewObj);
                previewObj = null;
            }
            previewObj = BuildableBase.SpawnBuildable(selectedType, currentCellPosition, transform, 1);
        }
        else
        {
            if (previewObj != null)
            {
                BuildableBase.DestroyBuildable(previewObj);
                previewObj = null;
            }
        }
        UpdateTilemap();
    }

    private void UpdateTilemap()
    {
        mousePosition = InputManager.Instance.GetMouseWolrdPosition();
        Vector3 offset = GetStartPositionOffset();
        int nearestX = Mathf.RoundToInt((mousePosition.x-offset.x) / GameConsts.TILE_SIZE); // 计算最近的 tile X 坐标
        int nearestY = Mathf.RoundToInt((mousePosition.y-offset.y) / GameConsts.TILE_SIZE); // 计算最近的 tile Y 坐标
        currentCellPosition = new Vector3Int(nearestX, nearestY, 0); // 计算最近的 tile 坐标
        if (currentCellPosition != lastCellPosition)
        {
            if (previewObj != null)
            {
                previewObj.SetPosition(currentCellPosition, 1);
            }
            lastCellPosition = currentCellPosition;
            //如果鼠标还在按着左键，继续操作
            if (InputManager.Instance.IsMouseLeftPressing())
            {
                OnMouseLeftClick();
            }
        }
    }

    private void DrawTileMap()
    {
        //如果当前鼠标位置上有物体并且和当前选择的不一样，Destroy
        if (currentBuildableMap.ContainsKey(currentCellPosition))
        {
            if (currentBuildableMap[currentCellPosition].Type != selectedType)
            {
                DestoryBuildable(currentCellPosition);
            }
            else
            {
                return;
            }
        }
        
        //生成选择的物体
        if (selectedType != BuildableType.none)
        {
            TilemapSaver.Instance.AddThisBuildable(selectedType, currentCellPosition);
            buildableInfos.Add(new BuildableInfo(selectedType, currentCellPosition));
            SpawnBuildable(selectedType, currentCellPosition);
        }
    }
    
    private void EraseTileMap()
    {
        //从当前位置射出射线
        Ray ray = InputManager.Instance.RaycastMouseRay();
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        //如果射线没有碰到任何物体，返回
        if (!hit)
        {
            return;
        }
        if(hit.collider.TryGetComponent(out BuildableBase buildable))
        {
            DestoryBuildable(buildable.Position);
            buildableInfos.Remove(buildableInfos.Find(info => info.position == buildable.Position));
            TilemapSaver.Instance.RemoveThisBuildable(buildable.Position);
        }
        
    }
    
    public void ClearAllTilemaps()
    {
        Debug.Log("Clear all tilemaps");
        foreach (var buildableInfo in currentBuildableMap)
        {
            BuildableBase.DestroyBuildable(buildableInfo.Value);
        }

        foreach (var buildableInfo in buildableInfos)
        {
            TilemapSaver.Instance.RemoveThisBuildable(buildableInfo.position);
        }
        buildableInfos.Clear();
        currentBuildableMap.Clear();
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
    
    public static Vector3 GetStartPositionOffset()
    {
        Vector3 mapStartPoint = GameObject.Find("start_point").transform.position;
        return new Vector3(mapStartPoint.x + GameConsts.TILE_SIZE / 2, mapStartPoint.y + GameConsts.TILE_SIZE / 2, 0);
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
            if (Utils.IsBuildableViewport(buildableInfo.position, Camera.main) || showAllBuildable)
            {
                SpawnBuildable(buildableInfo.type, buildableInfo.position);
            }
            else
            {
                DestoryBuildable(buildableInfo.position);
            }
        }
    }


    [Button]
    public void PrintListAndDic()
    {
        Debug.Log("currentBuildableMap count : " + currentBuildableMap.Count);
        Debug.Log("buildableInfos count : " + buildableInfos.Count);
    }
}
