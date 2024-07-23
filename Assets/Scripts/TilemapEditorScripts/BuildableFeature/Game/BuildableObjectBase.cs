using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


//可建造物体种类
public enum BuildableObjectType
{
    //地面 -- 可踩踏的地面 
    floor,
    //障碍 -- 不可触碰的障碍
    obstacle,
}

//在创建菜单栏中 Buildable/Create BuildableObject 创建asset
[CreateAssetMenu(fileName = "BuildableObject", menuName = "Buildable/Create BuildableObject")]
public class BuildableObjectBase : ScriptableObject
{
    [SerializeField] private BuildableObjectType buildableObjectType;
    [SerializeField] private TileBase tileBase;

    public TileBase Tile
    {
        get { return tileBase; }
    }
    public BuildableObjectType Type
    {
        get { return buildableObjectType; }
    }
}
