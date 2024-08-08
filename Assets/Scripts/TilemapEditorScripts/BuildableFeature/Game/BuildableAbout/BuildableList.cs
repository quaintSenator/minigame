using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum BuildableType
{
    none,
    floor_1,
    floor_2,
    floor_3,
    spring,
    twoStage,
    flyOrbit,
    enemy
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
