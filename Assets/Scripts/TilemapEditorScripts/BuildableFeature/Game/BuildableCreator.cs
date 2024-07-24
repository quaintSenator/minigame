using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



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
    
    private Vector2 mousePosition;
    private Vector3Int currentCellPosition;
    private Vector3Int lastCellPosition;

    private void OnEnable()
    {
        EventManager.AddListener(EventType.EscDownEvent,OnEscDown);
        EventManager.AddListener(EventType.MouseMoveEvent, OnMouseMove);
        EventManager.AddListener(EventType.MouseRightClickEvent, OnRightClick);
        EventManager.AddListener(EventType.MouseLeftClickEvent, OnMouseLeftClick);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.EscDownEvent,OnEscDown);
        EventManager.RemoveListener(EventType.MouseMoveEvent, OnMouseMove);
        EventManager.RemoveListener(EventType.MouseRightClickEvent, OnRightClick);
        EventManager.RemoveListener(EventType.MouseLeftClickEvent, OnMouseLeftClick);
    }

    private void OnMouseLeftClick(EventData data)
    {
        DrawTileMap();
    }


    private void OnEscDown(EventData data)
    {
        SetSelectedObject(null);
    }

    private void OnMouseMove(EventData data)
    {
        UpdateTilemap();
    }

    private void OnRightClick(EventData obj)
    {
        EraseTileMap();
    }
    
    public void SetSelectedObject(BuildableObjectBase obj)
    {
        selectedObj = obj;
        UpdateTilemap();
    }

    private void UpdateTilemap()
    {
        if(selectedObj == null)
        {
            priviewTilemap.ClearAllTiles();
            return;
        } 
        mousePosition = InputManager.Instance.GetMousePosition();
        currentCellPosition = priviewTilemap.WorldToCell(mousePosition);
        if (currentCellPosition != lastCellPosition)
        {
            priviewTilemap.ClearAllTiles();
            priviewTilemap.SetTile(currentCellPosition, selectedObj.Tile);
            lastCellPosition = currentCellPosition;
            
            //如果鼠标还在按着左键，继续绘制
            if (InputManager.Instance.IsMouseLeftPressing())
            {
                DrawTileMap();
            }
            
            //如果鼠标还在按着右键，继续擦除
            if (InputManager.Instance.IsMouseRightPressing())
            {
                EraseTileMap();
            }
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
