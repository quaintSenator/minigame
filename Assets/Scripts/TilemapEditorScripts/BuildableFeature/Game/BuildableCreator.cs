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
    [Serializable]
    class Type2Tilemap
    {
        public BuildableObjectType type;
        public Tilemap tilemap;
    }

    [SerializeField] private List<Type2Tilemap> tilemaps;
    [SerializeField] private Tilemap priviewTilemap;
    private TileBase tileBase;
    private BuildableObjectBase selectedObj;
    private TileMode currentTileMode = TileMode.None; 
    
    private Vector2 mousePosition;
    private Vector3Int currentCellPosition;
    private Vector3Int lastCellPosition;

    private void OnEnable()
    {
        EventManager.AddListener(EventType.EscDownEvent,OnEscDown);
        EventManager.AddListener(EventType.MouseMoveEvent, OnMouseMove);
        EventManager.AddListener(EventType.MouseLeftClickEvent, OnMouseLeftClick);
        EventManager.AddListener(EventType.EDownEvent, OnEDown);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.EscDownEvent,OnEscDown);
        EventManager.RemoveListener(EventType.MouseMoveEvent, OnMouseMove);
        EventManager.RemoveListener(EventType.MouseLeftClickEvent, OnMouseLeftClick);
        EventManager.RemoveListener(EventType.EDownEvent, OnEDown);
    }

    private void OnEDown(EventData obj)
    {
        if(currentTileMode != TileMode.Destroy)
        {
            SetSelectedObject(null);
            currentTileMode = TileMode.Destroy;
        }
        else
        {
            SetSelectedObject(null);
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
        SetSelectedObject(null);
    }

    private void OnMouseMove(EventData data)
    {
        UpdateTilemap();
    }
    
    public void SetSelectedObject(BuildableObjectBase obj)
    {
        selectedObj = obj;
        currentTileMode = selectedObj == null ? TileMode.None : TileMode.Build;
        UpdateTilemap();
    }

    private void UpdateTilemap()
    {
        mousePosition = InputManager.Instance.GetMousePosition();
        currentCellPosition = priviewTilemap.WorldToCell(mousePosition);
        if (currentCellPosition != lastCellPosition)
        {
            priviewTilemap.ClearAllTiles();
            if (selectedObj == null)
            {
                priviewTilemap.ClearAllTiles();
            }
            else
            {
                priviewTilemap.SetTile(currentCellPosition, selectedObj.Tile);
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
        if(selectedObj == null || InputManager.Instance.IsMouseOverUI())
        {
            return;
        }
        EraseTileMap();
        Tilemap tilemap = GetTilemap(selectedObj.Type);
        tilemap.SetTile(currentCellPosition, selectedObj.Tile);
    }
    
    private void EraseTileMap()
    {
        foreach (var tilemap in GetAllTilemaps())
        {
            tilemap.SetTile(currentCellPosition, null);
        }
    }
    
    private Tilemap GetTilemap(BuildableObjectType type)
    {
        foreach (var item in tilemaps)
        {
            if (item.type == type)
            {
                return item.tilemap;
            }
        }
        return null;
    }
    
    private List<Tilemap> GetAllTilemaps()
    {
        List<Tilemap> tilemapList = new List<Tilemap>();
        foreach (var item in tilemaps)
        {
            tilemapList.Add(item.tilemap);
        }
        return tilemapList;
    }
}
