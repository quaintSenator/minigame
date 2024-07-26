using System;
using System.Collections;
using System.Collections.Generic;
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
    private BuildableType selectedType = BuildableType.none;
    private BuildableBase previewObj;
    [SerializeField] private BuildableList buildableList; 
    [SerializeField] private Transform mapParent; 
    private TileMode currentTileMode = TileMode.None; 
    
    [SerializeField] private Transform mapStartPoint;
    private Vector3 mousePosition;
    private Vector3Int currentCellPosition;
    private Vector3Int lastCellPosition;

    private void OnEnable()
    {
        EventManager.AddListener(EventType.EscDownEvent,OnEscDown); //取消当前选择
        EventManager.AddListener(EventType.MouseMoveEvent, OnMouseMove); //鼠标移动
        EventManager.AddListener(EventType.MouseLeftClickEvent, OnMouseLeftClick); //绘制或者擦除
        EventManager.AddListener(EventType.EDownEvent, OnEDown); //切换擦除
        EventManager.AddListener(EventType.KDownEvent, OnKDown); //保存地图
        EventManager.AddListener(EventType.LDownEvent, OnLDown); //加载地图
        EventManager.AddListener(EventType.NumDownEvent, OnNumDown); //加载地图
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.EscDownEvent,OnEscDown);
        EventManager.RemoveListener(EventType.MouseMoveEvent, OnMouseMove);
        EventManager.RemoveListener(EventType.MouseLeftClickEvent, OnMouseLeftClick);
        EventManager.RemoveListener(EventType.EDownEvent, OnEDown);
        EventManager.RemoveListener(EventType.KDownEvent, OnKDown);
        EventManager.RemoveListener(EventType.LDownEvent, OnLDown);
    }
    
    private void OnNumDown(EventData data)
    {
        var numData = data as NumDownEventData;
        ClearAllTilemaps();
        TilemapSaver.Instance.LoadTilemap(numData.num.ToString());
    }

    private void OnKDown(EventData obj)
    {
        TilemapSaver.Instance.SaveTilemap();
        ClearAllTilemaps();
    }

    private void OnLDown(EventData obj)
    {
        ClearAllTilemaps();
        TilemapSaver.Instance.LoadTilemap(1.ToString());
    }

    private void OnEDown(EventData obj)
    {
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
        if (currentTileMode == TileMode.Build)
        {
            DrawTileMap();   
        }
        else if(currentTileMode == TileMode.Destroy)
        {
            EraseTileMap();
        }
    }


    private void OnEscDown(EventData data)
    {
        SetSelectedObject(BuildableType.none);
    }

    private void OnMouseMove(EventData data)
    {
        UpdateTilemap();
    }
    
    public void SetSelectedObject(BuildableType type)
    {
        Debug.Log("SetSelectedObject " + type);
        selectedType = type;
        currentTileMode = selectedType == null ? TileMode.None : TileMode.Build;
        UpdateTilemap();
    }

    private void UpdateTilemap()
    {
        mousePosition = InputManager.Instance.GetMousePosition();
        Vector3 offset = GetStartPositionOffset();
        int nearestX = Mathf.RoundToInt((mousePosition.x-offset.x) / GameConsts.TILE_SIZE); // 计算最近的 tile X 坐标
        int nearestY = Mathf.RoundToInt((mousePosition.y-offset.y) / GameConsts.TILE_SIZE); // 计算最近的 tile Y 坐标
        currentCellPosition = new Vector3Int(nearestX, nearestY, 0); // 计算最近的 tile 坐标
        Debug.Log("currentCellPosition " + currentCellPosition);
        if (currentCellPosition != lastCellPosition)
        {
            if (selectedType != BuildableType.none)
            {
                if (previewObj != null)
                {
                    previewObj.SetPosition(currentCellPosition);
                }
                else
                {
                    previewObj = Instantiate(buildableList.GetPrefab(selectedType)).GetComponent<BuildableBase>();
                    previewObj.transform.SetParent(mapParent);
                    previewObj.SetPosition(currentCellPosition);
                }
            }
            lastCellPosition = currentCellPosition;
        }
        //如果鼠标还在按着左键，继续操作
        if (InputManager.Instance.IsMouseLeftPressing())
        {
            OnMouseLeftClick();
        }
    }

    private void DrawTileMap()
    {
        //TODO 绘制
    }
    
    private void EraseTileMap()
    {
        //TODO 擦除
    }
    
    public void ClearAllTilemaps()
    {
        //TODO 清除所有地图
    }
    
    public Vector3 GetStartPositionOffset()
    {
        return new Vector3(mapStartPoint.position.x + GameConsts.TILE_SIZE / 2, mapStartPoint.position.y + GameConsts.TILE_SIZE / 2, 0);
    }
    
}
