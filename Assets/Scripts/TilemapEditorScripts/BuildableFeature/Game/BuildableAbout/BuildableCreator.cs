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
    private bool inSelectMode = false;
    private int currentRotate = 0;
    
    private Vector3 mousePosition;
    private Vector3Int currentCellPosition;
    private Vector3Int lastCellPosition;

    private List<BuildableInfo> buildableInfos = new List<BuildableInfo>();
    private Dictionary<Vector3Int, BuildableBase> currentBuildableMap = new Dictionary<Vector3Int, BuildableBase>();

    [SerializeField] private bool showAllBuildable = false;

    [SerializeField] private GameObject selectIcon;
    public List<BuildableInfo> selectedBuildableInfos = new List<BuildableInfo>();
    public List<BuildableInfo> currentMoveBuildableInfos = new List<BuildableInfo>();
    private Dictionary<BuildableInfo, GameObject> selectIcons = new Dictionary<BuildableInfo, GameObject>();
    
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
            Debug.Log("Read buildableInfo : " + buildableInfo.index);
            BuildableInfo newBuildableInfo = new BuildableInfo(buildableInfo);
            buildableInfos.Add(new BuildableInfo(newBuildableInfo));
            TilemapSaver.Instance.AddThisBuildable(newBuildableInfo);
        }
        CheckBuildableVisible();
        BuildableBase.UpdateGroupIndexAndIndex();
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.CancelCurrentSelectEvent,CancelCurrentSelectToBuild); //取消当前选择
        EventManager.AddListener(EventType.MouseMoveEvent, OnMouseMove); //鼠标移动
        EventManager.AddListener(EventType.DrawOrEraseEvent, OnDrawOrErase); //绘制或者擦除
        EventManager.AddListener(EventType.ChangeTileModeEvent, ChangeTileMode); //切换擦除
        EventManager.AddListener(EventType.SaveMapEvent, SaveMap); //保存地图
        EventManager.AddListener(EventType.LoadMapOneEvent, LoadMapOne); //加载地图一
        EventManager.AddListener(EventType.LoadMapOneEvent, LoadMapOne); //加载地图一
        EventManager.AddListener(EventType.LoadMapTwoEvent, LoadMapTwo); //加载地图二
        EventManager.AddListener(EventType.LoadMapThreeEvent, LoadMapThree); //加载地图三
        EventManager.AddListener(EventType.LeftMoveBuildableEvent, LeftMoveBuildable); //左移
        EventManager.AddListener(EventType.RightMoveBuildableEvent, RightMoveBuildable); //右移
        EventManager.AddListener(EventType.UpMoveBuildableEvent, UpMoveBuildable); //上移
        EventManager.AddListener(EventType.DownMoveBuildableEvent, DownMoveBuildable); //下移
        EventManager.AddListener(EventType.EnterSelectModeEvent, EnterSelectMode); //进入选择模式
        EventManager.AddListener(EventType.ExitSelectModeEvent, ExitSelectMode); //退出选择模式
        EventManager.AddListener(EventType.SelectBuildableEvent, OnSelectBuildable); //选择物体
        EventManager.AddListener(EventType.CancelAllSelectEvent, CancelAllSelect); //取消选择所有物体
        EventManager.AddListener(EventType.StartSelectZoneEvent, OnStartSelectZone); //开始选中框
        EventManager.AddListener(EventType.CompleteSelectZoneEvent, OnCompleteSelectZone); //完成选中框
        EventManager.AddListener(EventType.CompleteContinuousPointEvent, OnCompleteContinuousPoint); //完成连续点
        EventManager.AddListener(EventType.RotateBuildableEvent, OnRotateBuildable); //旋转物体
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.CancelCurrentSelectEvent,CancelCurrentSelectToBuild); //取消当前选择
        EventManager.RemoveListener(EventType.MouseMoveEvent, OnMouseMove); //鼠标移动
        EventManager.RemoveListener(EventType.DrawOrEraseEvent, OnDrawOrErase); //绘制或者擦除
        EventManager.RemoveListener(EventType.ChangeTileModeEvent, ChangeTileMode); //切换擦除
        EventManager.RemoveListener(EventType.SaveMapEvent, SaveMap); //保存地图
        EventManager.RemoveListener(EventType.LoadMapOneEvent, LoadMapOne); //加载地图一
        EventManager.RemoveListener(EventType.LoadMapOneEvent, LoadMapOne); //加载地图一
        EventManager.RemoveListener(EventType.LoadMapTwoEvent, LoadMapTwo); //加载地图二
        EventManager.RemoveListener(EventType.LoadMapThreeEvent, LoadMapThree); //加载地图三
        EventManager.RemoveListener(EventType.LeftMoveBuildableEvent, LeftMoveBuildable); //左移
        EventManager.RemoveListener(EventType.RightMoveBuildableEvent, RightMoveBuildable); //右移
        EventManager.RemoveListener(EventType.UpMoveBuildableEvent, UpMoveBuildable); //上移
        EventManager.RemoveListener(EventType.DownMoveBuildableEvent, DownMoveBuildable); //下移
        EventManager.RemoveListener(EventType.EnterSelectModeEvent, EnterSelectMode); //进入选择模式
        EventManager.RemoveListener(EventType.ExitSelectModeEvent, ExitSelectMode); //退出选择模式
        EventManager.RemoveListener(EventType.SelectBuildableEvent, OnSelectBuildable); //选择物体
        EventManager.RemoveListener(EventType.CancelAllSelectEvent, CancelAllSelect); //取消选择所有物体
        EventManager.RemoveListener(EventType.StartSelectZoneEvent, OnStartSelectZone); //开始选中框
        EventManager.RemoveListener(EventType.CompleteSelectZoneEvent, OnCompleteSelectZone); //完成选中框
        EventManager.RemoveListener(EventType.CompleteContinuousPointEvent, OnCompleteContinuousPoint); //完成连续点
        EventManager.RemoveListener(EventType.RotateBuildableEvent, OnRotateBuildable); //旋转物体
        
        AutoSaveMap();
    }

    public void AutoSaveMap()
    {
        string key = "auto_save_2";
        List<BuildableInfo> buildableInfos = new List<BuildableInfo>();
        foreach (var buildableInfo in this.buildableInfos)
        {
            buildableInfos.Add(buildableInfo);
        }
        MapData mapData = new MapData(key, buildableInfos);
        PlayerPrefs.SetString(GameConsts.AUTO_TILEMAP_SAVE_DATA_2, JsonUtility.ToJson(mapData));
        Debug.Log("Auto save tilemap data v2");
    }

    private void OnRotateBuildable(EventData obj)
    {
        if (selectedType != BuildableType.none)
        {
            currentRotate += 1;
            if (currentRotate == 4)
            {
                currentRotate = 0;
            }
            if (previewObj != null)
            {
                previewObj.transform.rotation = Quaternion.Euler(0, 0, currentRotate * -90);
            }
        }
    }

    private void OnCompleteContinuousPoint(EventData obj)
    {
        if (selectedType == BuildableType.continuous_point)
        {
            BuildableBase.CompleteCurrentGroup();
            SetSelectedObject(BuildableType.none);
        }
    }

    private void OnStartSelectZone(EventData obj)
    {
        SetSelectedObject(BuildableType.none);
    }

    private void OnCompleteSelectZone(EventData data)
    {
        var selectZoneData = data as SelectZoneEventData;
        float xMin = Mathf.Min(selectZoneData.startPos.x, selectZoneData.endPos.x);
        float xMax = Mathf.Max(selectZoneData.startPos.x, selectZoneData.endPos.x);
        float yMin = Mathf.Min(selectZoneData.startPos.y, selectZoneData.endPos.y);
        float yMax = Mathf.Max(selectZoneData.startPos.y, selectZoneData.endPos.y);
        //检查buildableInfos里面的物体是否在选择框内
        foreach (var buildableInfo in buildableInfos)
        {
            Vector3 realPos = Utils.GetRealPostion(buildableInfo.position);
            if (realPos.x >= xMin && realPos.x <= xMax && realPos.y >= yMin && realPos.y <= yMax)
            {
                if(selectedBuildableInfos.Find(info => info.position == buildableInfo.position) == null)
                {
                    selectedBuildableInfos.Add(buildableInfo);
                    GameObject icon = PoolManager.Instance.SpawnFromPool("selectIcons", selectIcon, mapParent);
                    icon.transform.position = realPos;
                    selectIcons.Add(buildableInfo, icon);
                }
                else
                {
                    selectedBuildableInfos.Remove(buildableInfo);
                    if (selectIcons.ContainsKey(buildableInfo))
                    {
                        PoolManager.Instance.ReturnToPool("selectIcons", selectIcons[buildableInfo]);
                        selectIcons.Remove(buildableInfo);
                    }
                }
            }
            else
            {
                Debug.Log(realPos + " is not in the select zone");
            }
        }
    }

    public bool GetInSelectMode()
    {
        return inSelectMode;
    }
    
    private void EnterSelectMode(EventData data)
    {
        SetSelectedObject(BuildableType.none);
        inSelectMode = true;
    }
    
    private void ExitSelectMode(EventData data)
    {
        inSelectMode = false;
        foreach (var buildableInfo in currentMoveBuildableInfos)
        {
            while(buildableInfos.Find(info => info.position == buildableInfo.position) != null)
            {
                Debug.Log("Remove buildableInfo : " + buildableInfo.position);
                buildableInfos.Remove(buildableInfos.Find(info => info.position == buildableInfo.position));
            }
            buildableInfos.Add(buildableInfo);
        }
        TilemapSaver.Instance.ClearCurrentBuildableInfos();
        TilemapSaver.Instance.CopyCurrentBuildableInfos(this.buildableInfos);
        currentMoveBuildableInfos.Clear();
    }

    private void OnSelectBuildable(EventData obj)
    {
        if(!inSelectMode)
        {
            return;
        }
        SelectBuildable();
    }
    
    private void SelectBuildable()
    {
        //从当前位置射出射线
        Ray ray = InputManager.Instance.RaycastMouseRay();
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        //如果射线没有碰到任何物体，返回
        if (!hit)
        {
            return;
        }
        if(hit.collider.TryGetComponent(out BuildableBase selectable))
        {
            BuildableInfo selectInfo = selectedBuildableInfos.Find(info => info.position == selectable.Position);
            if (selectInfo != null)
            {
                selectedBuildableInfos.Remove(selectInfo);
                if (selectIcons.ContainsKey(selectInfo))
                {
                    PoolManager.Instance.ReturnToPool("selectIcons", selectIcons[selectInfo]);
                    selectIcons.Remove(selectInfo);
                }
            }
            else
            {
                BuildableInfo buildableInfo = buildableInfos.Find(info => info.position == selectable.Position);
                selectedBuildableInfos.Add(buildableInfo);
                GameObject icon = PoolManager.Instance.SpawnFromPool("selectIcons", selectIcon, mapParent);
                icon.transform.position = selectable.transform.position;
                selectIcons.Add(buildableInfo, icon);
            }
        }
    }

    private void CancelAllSelect(EventData obj)
    {
        foreach (var iconKeyValue in selectIcons)
        {
            PoolManager.Instance.ReturnToPool("selectIcons", iconKeyValue.Value);
        }
        selectIcons.Clear();
        selectedBuildableInfos.Clear();
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

    private void OnMouseLeftClick()
    {
        if (InputManager.Instance.IsMouseOverUI())
        {
            return;
        }

        if (inSelectMode)
        {
            SelectBuildable();
        }
        OnDrawOrErase();
    }

    private void OnDrawOrErase(EventData data = null)
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
    
    private void CancelCurrentSelectToBuild(EventData data)
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
        currentRotate = 0;
        currentTileMode = selectedType == BuildableType.none ? TileMode.None : TileMode.Build;
        if (selectedType != BuildableType.none)
        {
            if (previewObj != null)
            {
                BuildableBase.DestroyBuildableWithoutDispose(previewObj);
                previewObj = null;
            }
            previewObj = BuildableBase.SpawnBuildableWithoutInit(selectedType, currentCellPosition, transform, 1);
        }
        else
        {
            if (previewObj != null)
            {
                BuildableBase.DestroyBuildableWithoutDispose(previewObj);
                previewObj = null;
            }
        }
        UpdateTilemap();
    }
    
    private void MoveBuildable(Vector3Int offset)
    {
        if (!inSelectMode)
        {
            return;
        }
        foreach (var buildableInfo in currentMoveBuildableInfos)
        {
            buildableInfo.position += offset;
        }
        foreach (var buildable in currentBuildableMap)
        {
            BuildableBase.DestroyBuildable(buildable.Value);
        }
        currentBuildableMap.Clear();
        CheckBuildableVisible();
    }
    
    private List<BuildableInfo> GetSelectedBuildableInfos()
    {
        List<BuildableInfo> needMoveInfos;
        if(selectedBuildableInfos.Count != 0)
        {
            needMoveInfos = new List<BuildableInfo>(selectedBuildableInfos);
        }
        else
        {
            needMoveInfos = new List<BuildableInfo>();
            
            float currentX = RhythmViewer.Instance.GetCurrentMusicLinePos().x;
            foreach (var buildableInfo in buildableInfos)
            {
                float realX = buildableInfo.position.x * GameConsts.TILE_SIZE + GetStartPositionOffset().x;
                if (realX >= currentX)
                {
                    needMoveInfos.Add(buildableInfo);
                }
            }
        }
        return needMoveInfos; 
    }
    
    private void LeftMoveBuildable(EventData data)
    {
        if(currentMoveBuildableInfos.Count == 0)
        {
            currentMoveBuildableInfos = GetSelectedBuildableInfos();
        }
        MoveBuildable(new Vector3Int(-1, 0, 0));
    }
    
    private void RightMoveBuildable(EventData data)
    {
        if(currentMoveBuildableInfos.Count == 0)
        {
            currentMoveBuildableInfos = GetSelectedBuildableInfos();
        }
        MoveBuildable(new Vector3Int(1, 0, 0));
    }
    
    private void UpMoveBuildable(EventData data)
    {
        if(currentMoveBuildableInfos.Count == 0)
        {
            currentMoveBuildableInfos = GetSelectedBuildableInfos();
        }
        MoveBuildable(new Vector3Int(0, 1, 0));
    }
    
    private void DownMoveBuildable(EventData data)
    {
        if(currentMoveBuildableInfos.Count == 0)
        {
            currentMoveBuildableInfos = GetSelectedBuildableInfos();
        }
        MoveBuildable(new Vector3Int(0, -1, 0));
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
                previewObj.transform.rotation = Quaternion.Euler(0, 0, currentRotate * -90);
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
                BuildableBase buildable = currentBuildableMap[currentCellPosition];
                DestoryBuildable(buildable.Position);
                buildableInfos.Remove(buildableInfos.Find(info => info.position == buildable.Position));
                TilemapSaver.Instance.RemoveThisBuildable(buildable.Position);
            }
            else
            {
                return;
            }
        }
        
        //生成选择的物体
        if (selectedType != BuildableType.none)
        {
            BuildableInfo newBuildableInfo = new BuildableInfo(selectedType, currentCellPosition, BuildableBase.GetBuildableTypeSpawnIndex(selectedType), currentRotate);
            TilemapSaver.Instance.AddThisBuildable(newBuildableInfo);
            buildableInfos.Add(newBuildableInfo);
            SpawnBuildable(selectedType, currentCellPosition, newBuildableInfo.index, newBuildableInfo.rotation);
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
    
    private void DrawTileMap(BuildableType type, Vector3Int position)
    {
        //如果当前鼠标位置上有物体并且和当前选择的不一样，Destroy
        if (currentBuildableMap.ContainsKey(position))
        {
            if (currentBuildableMap[position].Type != type)
            {
                BuildableBase buildable = currentBuildableMap[position];
                DestoryBuildable(buildable.Position);
                buildableInfos.Remove(buildableInfos.Find(info => info.position == buildable.Position));
                TilemapSaver.Instance.RemoveThisBuildable(buildable.Position);
            }
            else
            {
                return;
            }
        }
        
        //生成选择的物体
        if (type != BuildableType.none)
        {
            BuildableInfo newBuildableInfo = new BuildableInfo(type, position, BuildableBase.GetBuildableTypeSpawnIndex(type), currentRotate);
            TilemapSaver.Instance.AddThisBuildable(newBuildableInfo);
            buildableInfos.Add(newBuildableInfo);
            SpawnBuildable(type, position, newBuildableInfo.index, newBuildableInfo.rotation);
        }
    }
    
    private void EraseTileMap(Vector3Int position)
    {
        DestoryBuildable(position);
        buildableInfos.Remove(buildableInfos.Find(info => info.position == position));
        TilemapSaver.Instance.RemoveThisBuildable(position);
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
            if (showAllBuildable || Utils.IsAlwaysVisible(buildableInfo.type) || Utils.IsBuildableViewport(buildableInfo.position, Camera.main))
            {
                SpawnBuildable(buildableInfo.type, buildableInfo.position, buildableInfo.index, buildableInfo.rotation);
            }
            else
            {
                DestoryBuildable(buildableInfo.position);
            }
        }

        foreach (var iconKeyValue in selectIcons)
        {
            BuildableInfo buildableInfo = iconKeyValue.Key;
            GameObject icon = iconKeyValue.Value;
            Vector3 realPosition = Utils.GetRealPostion(buildableInfo.position);
            icon.transform.position = realPosition;
        }

        BuildableBase.LinkAllGroup();
    }

    public TileMode GetTileMode()
    {
        return currentTileMode;
    }


    #region 测试打印

    [Button]
    public void PrintListAndDic()
    {
        Debug.Log("currentBuildableMap count : " + currentBuildableMap.Count);
        Debug.Log("buildableInfos count : " + buildableInfos.Count);

        foreach (var VARIABLE in buildableInfos)
        {
            Debug.Log(VARIABLE.position + " : " + VARIABLE.type);
        }
    }

    [Button]
    public void PrintBuildableGroupMap()
    {
        Debug.Log("BuildableGroupMap count : " + BuildableBase.BuildableGroupMap.Count);
        foreach (var VARIABLE in BuildableBase.BuildableGroupMap)
        {
            foreach (var buildableBase in VARIABLE.Value)
            {
                Debug.Log(VARIABLE.Key + " : " + buildableBase.Index + " : " + buildableBase.Position);
            }
        }
    }

    #endregion
}
