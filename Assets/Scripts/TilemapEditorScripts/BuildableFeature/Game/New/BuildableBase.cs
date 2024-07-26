using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableBase : MonoBehaviour
{
    public string buildableName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            TriggerThisBuildable();
        }
    }
    
    protected virtual void TriggerThisBuildable()
    {
        Debug.Log("Triggered " + buildableName);
    }
    
    public void SetPosition(Vector3Int position)
    {
        //计算TILE_SIZE 和 起始位置偏移 和 以左下角为锚点 得到实际位置
        Vector3 offset = BuildableCreator.Instance.GetStartPositionOffset();
        Vector3 realPosition = new Vector3(position.x * GameConsts.TILE_SIZE + offset.x, position.y * GameConsts.TILE_SIZE + offset.y, 0);
        transform.position = realPosition;
    }
}
