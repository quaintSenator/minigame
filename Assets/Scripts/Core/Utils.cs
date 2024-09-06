using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Utils
{
    public static bool IsBuildableViewportByV3Int(Vector3Int position, Camera camera)
    {
        Vector3 realPosition = GetRealPostion(position);
        return IsBuildableViewportByV3(realPosition, camera);
    }
    
    public static bool IsBuildableViewportByV3(Vector3 position, Camera camera)
    {
        Vector3 viewportPos = camera.WorldToViewportPoint(position);
        if (viewportPos.x < -0.5f || viewportPos.x > 1.5f || viewportPos.y < -0.5f || viewportPos.y > 1.5f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    
    public static Vector3 GetRealPostion(Vector3Int position)
    {
        Vector3 offset = BuildableCreator.GetStartPositionOffset();
        Vector3 realPosition = new Vector3(position.x * GameConsts.TILE_SIZE + offset.x, position.y * GameConsts.TILE_SIZE + offset.y, 0);
        return realPosition;
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
    
    public static bool IsAlwaysVisible(BuildableType type)
    {
        if (type == BuildableType.continuous_point || 
            type == BuildableType.change_direction_trigger)
        {
            return true;
        }
        
        return false;
    }

    public static void AddMaskAndLoadScene(Transform parent, string sceneName)
    {
        GameObject exitPage = Resources.Load<GameObject>("mask_page");
        if(exitPage == null)
        {
            Debug.LogError("mask_page is null");
            return;
        }
        exitPage = GameObject.Instantiate(exitPage, parent);
        exitPage.SetActive(true);
        exitPage.GetComponent<Image>().DOColor(new Color(0, 0, 0, 1f), 0.5f).onComplete = () =>
        {
            SceneManager.LoadScene(sceneName);
        };
    }
}
