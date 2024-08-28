using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum BuildableType
{
    none,
    floor_1 = 1,
    floor_2 = 2,
    floor_3 = 3,
    spring = 4,
    twoStage = 5,
    flyOrbit = 6,
    enemy = 7,
    enemy_2 = 8,
    enemy_3 = 9,
    continous_start_point = 10,
    continous_middle_point = 11,
    continous_end_point = 12,
    spikeTrap_1 = 13,
    spikeTrap_2 = 14,
    continuous_point = 15,
    resetPoint = 16,
    bossShowUpTrigger = 17,
    bossLaserTrigger = 18,
    bossBulletTrigger = 19,
}

[CreateAssetMenu(fileName = "BuildableList", menuName = "Buildable/Create BuildableList")]
public class BuildableList : ScriptableObject
{
    [SerializeField] private List<BuildableSO> buildableList;
    
    public GameObject GetPrefab(BuildableType buildableType)
    {
        foreach (var buildable in buildableList)
        {
            if (buildable.BuildableObjectType == buildableType)
            {
                return buildable.Prefab;
            }
        }
    
        return null;
    }
}
