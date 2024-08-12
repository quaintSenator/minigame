using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

public class GameConsts
{
    public static readonly float GRAVITY = 9.8f;
    public static readonly Vector3 START_POSITION = new Vector3(0, -1f, 0.0f);
    public static readonly Quaternion ZERO_ROTATION = new Quaternion();
    public static readonly Vector2 START_VELOCITY = new Vector2();
    public static readonly string TILEMAP_SAVE_DATA = "__tilemap_save_data__";
    public static readonly string AUTO_TILEMAP_SAVE_DATA = "__auto_tilemap_save_data__";
    public static readonly string AUTO_TILEMAP_SAVE_DATA_2 = "__auto_tilemap_save_data_2__";
    public static readonly string CURRENT_SELECTED_MAPDATA = "__current_selected_mapdata__";
    public static readonly float TILE_SIZE = 1f;
    public static readonly float TILE_CHECK_GAP = 0.1f;
    public static readonly float SPEED = 8.0f;

}