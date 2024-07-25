using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConsts
{
    public static readonly float GRAVITY = 9.8f;
    public static readonly Vector3 START_POSITION = new Vector3(-7.16f, -0.67f, 0.0f);
    public static readonly Quaternion ZERO_ROTATION = new Quaternion();
    public static readonly Vector2 START_VELOCITY = new Vector2();
    public static readonly string TILEMAP_SAVE_DATA = "__tilemap_save_data__";
    public static readonly string AUTO_TILEMAP_SAVE_DATA = "__auto_tilemap_save_data__";
}