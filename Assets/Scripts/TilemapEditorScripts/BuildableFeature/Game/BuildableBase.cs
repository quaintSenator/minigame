using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildableBase : MonoBehaviour
{
    private Vector3Int m_position;
    public Vector3Int Position => m_position;
    public static BuildableList buildableList;
    private BuildableType type;
    public BuildableType Type => type;
    
    private void OnEnable()
    {
        SetSortingOrder();
    }

    public virtual void Init()
    {
        //TODO 初始化
    }
    
    public virtual void Dispose()
    {
        //TODO 销毁
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            TriggerThisBuildable();
        }
    }
    
    protected virtual void TriggerThisBuildable()
    {
        //TODO 触发功能
    }
    
    //设置渲染层级
    private void SetSortingOrder(int sortingOrder = 0)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.sortingOrder = sortingOrder;
        }
    }
    
    //设置位置
    public void SetPosition(Vector3Int position, int sortingOrder = 0)
    {
        //计算TILE_SIZE 和 起始位置偏移 和 以左下角为锚点 得到实际位置
        m_position = position;
        Vector3 offset = BuildableCreator.GetStartPositionOffset();
        Vector3 realPosition = new Vector3(position.x * GameConsts.TILE_SIZE + offset.x, position.y * GameConsts.TILE_SIZE + offset.y, 0);
        transform.position = realPosition;
        SetSortingOrder(sortingOrder);
    }

    
    //生成地块
    public static BuildableBase SpawnBuildable(BuildableType type, Vector3Int position, int sortingOrder = 0)
    {
        if(buildableList == null)
        {
            buildableList = Resources.Load<BuildableList>("AllBuildableList");
        }
        BuildableBase buildable = Instantiate(buildableList.GetPrefab(type)).GetComponent<BuildableBase>();
        buildable.type = type;
        buildable.SetPosition(position, sortingOrder);
        return buildable;
    }
    
    public static void DestroyBuildable(BuildableBase buildable)
    {
        if (buildable == null)
        {
            return;
        }
        buildable.Dispose();
        Destroy(buildable.gameObject);
    }
}
