using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildableBase : MonoBehaviour
{
    //静态变量 确定当前的Index
    public static int CurrentUsedIndex = 1;
    public static int CurrentGroupIndex = 1;
    //分组Buildable Map  <int(GroupIndex), List<BuildableBase>>(Buildable in this group)>
    public static Dictionary<int, List<BuildableBase>> BuildableGroupMap = new Dictionary<int, List<BuildableBase>>();
    
    private Vector3Int m_position;
    public Vector3Int Position => m_position;
    public static BuildableList buildableList;
    private BuildableType type;
    public BuildableType Type => type;
    private int index;
    public int Index => index;
    private int rotation;
    public int Rotation => rotation;

    public virtual void Init()
    {
        //TODO 初始化
    }
    
    public virtual void Dispose()
    {
        //TODO 销毁
    }
    
    public virtual void RegisterEvent()
    {
        //TODO 注册事件
    }
    
    public virtual void UnRegisterEvent()
    {
        //TODO 注销事件
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out MusicCurrentPosLine line))
        {
            TriggerThisBuildable(null);
        }

        if (other.TryGetComponent(out PlayerController player))
        {
            TriggerThisBuildable(player);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        
        if (other.TryGetComponent(out MusicCurrentPosLine line))
        {
            TriggerOffThisBuildable(null);
        }

        if(other.TryGetComponent(out PlayerController player))
        {
            TriggerOffThisBuildable(player);
        }
    }
    
    protected virtual void TriggerThisBuildable(PlayerController player)
    {
        //TODO 触发功能
    }

    protected virtual void TriggerOffThisBuildable(PlayerController player)
    {

    }

    // protected virtual void AdjustBuildableScale()
    // {
    //     transform.localScale = new Vector3(GameConsts.TILE_SIZE / 1f, GameConsts.TILE_SIZE / 1f, 1);
    // }
    
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
        transform.rotation = Quaternion.Euler(0, 0, rotation * -90);
        SetSortingOrder(sortingOrder);
    }

    
    //生成地块
    public static BuildableBase SpawnBuildable(BuildableType type, Vector3Int position, int index, int rotation, Transform parent, int sortingOrder = 0)
    {
        if(buildableList == null)
        {
            buildableList = Resources.Load<BuildableList>("AllBuildableList");
        }
        GameObject obj = PoolManager.Instance.SpawnFromPool(type.ToString(), buildableList.GetPrefab(type), parent);
        BuildableBase buildable = obj.GetComponent<BuildableBase>();
        buildable.type = type;
        buildable.index = index;
        buildable.rotation = rotation;
        buildable.SetPosition(position, sortingOrder);
        //buildable.AdjustBuildableScale();
        buildable.RegisterEvent();
        buildable.Init();
        PutBuildableToGroup(buildable);
        return buildable;
    }
    
    public static void DestroyBuildable(BuildableBase buildable)
    {
        if (buildable == null)
        {
            return;
        }
        buildable.Dispose();
        buildable.UnRegisterEvent();
        RemoveBuildableFromGroup(buildable);
        PoolManager.Instance.ReturnToPool(buildable.Type.ToString(), buildable.gameObject);
    }

    public static BuildableBase SpawnBuildableWithoutInit(BuildableType type, Vector3Int position, Transform parent,
        int sortingOrder = 0)
    {
        if(buildableList == null)
        {
            buildableList = Resources.Load<BuildableList>("AllBuildableList");
        }
        GameObject obj = Instantiate(buildableList.GetPrefab(type), parent);
        BuildableBase buildable = obj.GetComponent<BuildableBase>();
        buildable.type = type;
        buildable.index = -1;
        buildable.SetPosition(position, sortingOrder);
        //buildable.AdjustBuildableScale();
        return buildable;
    }
    
    public static void DestroyBuildableWithoutDispose(BuildableBase buildable)
    {
        if (buildable == null)
        {
            return;
        }
        Destroy(buildable.gameObject);
    }

    public static int GetBuildableTypeSpawnIndex(BuildableType type)
    {
        if (type == BuildableType.continuous_point)
        {
            return CurrentGroupIndex*1000 + CurrentUsedIndex++;
        }
        return -1;
    }
    
    public static int GetBuildableGroupIndex(int index)
    {
        return index / 1000;
    }
    
    public static BuildableBase GetLastBuildableInGroup(int groupIndex, int index)
    {
        if (BuildableGroupMap.ContainsKey(groupIndex))
        {
            BuildableBase lastBuildable = null;
            foreach (var buildable in BuildableGroupMap[groupIndex])
            {
                if (buildable.Index == index)
                {
                    return lastBuildable;
                }
                lastBuildable = buildable;
            }
        }
        return null;
    }
    
    public static BuildableBase GetNextBuildableInGroup(int groupIndex, int index)
    {
        if (BuildableGroupMap.ContainsKey(groupIndex))
        {
            bool find = false;
            foreach (var buildable in BuildableGroupMap[groupIndex])
            {
                if (find)
                {
                    return buildable;
                }
                if (buildable.Index == index)
                {
                    find = true;
                }
            }
        }
        return null;
    }

    public static void LinkAllGroup()
    {
        foreach (var allGroups in BuildableGroupMap)
        {
            foreach (var group in allGroups.Value)
            {
                var point = group as ContinuousPoint;
                point?.LinkPoint();
            }
        }
    }

    //GroupIndex更新到最大的GroupIndex+1，Index更新到1
    public static void UpdateGroupIndexAndIndex()
    {
        CurrentUsedIndex = 1;
        foreach (var VARIABLE in BuildableGroupMap)
        {
            CurrentGroupIndex = Math.Max(CurrentGroupIndex, VARIABLE.Key);
        }
        CurrentGroupIndex++;
    }
    
    public static void PutBuildableToGroup(BuildableBase buildable)
    {
        int groupIndex = GetBuildableGroupIndex(buildable.Index);
        if (groupIndex < 1)
        {
            return;
        }
        if (!BuildableGroupMap.ContainsKey(groupIndex))
        {
            BuildableGroupMap.Add(groupIndex, new List<BuildableBase>());
        }
        BuildableGroupMap[groupIndex].Add(buildable);
        //根据List中的Buildable的Index排序，从小到大
        BuildableGroupMap[groupIndex].Sort((a, b) => a.Index - b.Index);
    }
    
    public static void RemoveBuildableFromGroup(BuildableBase buildable)
    {
        int groupIndex = GetBuildableGroupIndex(buildable.Index);
        if (groupIndex < 1)
        {
            return;
        }
        if (BuildableGroupMap.ContainsKey(groupIndex))
        {
            BuildableGroupMap[groupIndex].Remove(buildable);
            if (BuildableGroupMap[groupIndex].Count == 0)
            {
                BuildableGroupMap.Remove(groupIndex);
            }
        }
    }
    
    public static void CompleteCurrentGroup()
    {
        CurrentUsedIndex = 1;
        CurrentGroupIndex++;
    }
}
