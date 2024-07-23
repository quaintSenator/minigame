using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildableCreator : Singleton<BuildableCreator>
{
    [SerializeField] private Tilemap priviewTilemap;
    private TileBase tileBase;
    private BuildableObjectBase selectedObj;
    
    private Vector2 mousePosition;
    private Vector3Int currentCellPosition;
    private Vector3Int lastCellPosition;

    private void OnEnable()
    {
        EventManager.AddListener(EventType.OnEscDown,OnEscDown);
        EventManager.AddListener(EventType.OnMouseMove, OnMouseMove);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.OnEscDown,OnEscDown);
        EventManager.RemoveListener(EventType.OnMouseMove, OnMouseMove);
    }
    
    public void SetSelectedObject(BuildableObjectBase obj)
    {
        selectedObj = obj;
        UpdateTilemap();
    }

    private void OnMouseMove(EventData data)
    {
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
        }
    }

    private void OnEscDown(EventData data)
    {
        SetSelectedObject(null);
    }
}
