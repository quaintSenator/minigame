using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static bool IsBuildableViewport(Vector3Int position, Camera camera)
    {
        Vector3 offset = BuildableCreator.GetStartPositionOffset();
        Vector3 realPosition = new Vector3(position.x * GameConsts.TILE_SIZE + offset.x, position.y * GameConsts.TILE_SIZE + offset.y, 0);
        Vector3 viewportPos = camera.WorldToViewportPoint(realPosition);
        if (viewportPos.x < -0.5f || viewportPos.x > 1.5f || viewportPos.y < -0.5f || viewportPos.y > 1.5f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static Transform GetStartPointPostion()
    {
        if(GameObject.Find("start_point") != null)
        {
            return GameObject.Find("start_point").transform;
        }
        else
        {
            GameObject startPoint = new GameObject("start_point");
            return startPoint.transform;
        }
    }
}
