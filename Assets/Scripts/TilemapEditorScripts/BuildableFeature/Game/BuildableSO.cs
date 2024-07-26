using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


//在创建菜单栏中 Buildable/Create BuildableObject 创建asset
[CreateAssetMenu(fileName = "BuildableObject", menuName = "Buildable/Create BuildableObject")]
public class BuildableSO : ScriptableObject
{
    [SerializeField] private BuildableType buildableObjectType;
    [SerializeField] private GameObject prefab;

    public BuildableType BuildableObjectType => buildableObjectType;
    public GameObject Prefab => prefab;
}
