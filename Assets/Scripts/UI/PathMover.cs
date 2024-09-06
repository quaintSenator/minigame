using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using UnityEngine;

public class PathMover : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();
    private List<Transform> pointTransforms = new List<Transform>();
    public bool EditorMode = false;
    private GameObject pointPrefab; // 用于生成路径点的预制体
    public string savekey = "__pathData__";
    private TweenerCore<Vector3, Path, PathOptions> tween;
    
    private void Awake()
    {
        pointPrefab = Resources.Load<GameObject>("PathPoint");
    }

    void Update()
    {
        if (EditorMode && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // 确保路径点在2D平面上
            GameObject newPoint = Instantiate(pointPrefab, mousePos, Quaternion.identity);
            // 转换到父物体的局部坐标
            Transform parent = transform.parent;
            if (parent != null)
            {
                mousePos = parent.InverseTransformPoint(mousePos);
            }
            pointTransforms.Add(newPoint.transform);
            points.Add(mousePos);
        }
    }
    
    [Button]
    public void GetTween(float duration)
    {
        Vector3[] positions = new Vector3[points.Count + 1];
        for (int i = 0; i < points.Count; i++)
        {
            positions[i] = points[i];
        }
        positions[points.Count] = points[0];
        bool doneOnce = true;
        
        tween = transform.DOLocalPath(positions, duration, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).Pause();
    }
    

    [Button]
    public void StopMove()
    {
        tween.Pause();
    }
    
    [Button]
    public void PlayMove()
    {
        tween.Play();
    }

    
    [Button]
    public void SavePath()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (Vector3 point in points)
        {
            positions.Add(point);
        }

        PathData pathData = new PathData { positions = positions };
        string json = JsonUtility.ToJson(pathData);

        PlayerPrefs.SetString(savekey, json);
    }

    
    [Button]
    public void LoadPath()
    {
        string json = PlayerPrefs.GetString(savekey);
        PathData pathData = JsonUtility.FromJson<PathData>(json);

        // 清理现有的路径点
        foreach (Transform pointTrans in pointTransforms)
        {
            Destroy(pointTrans.gameObject);
        }
        points.Clear();

        // 加载路径点
        foreach (Vector3 position in pathData.positions)
        {
            points.Add(position);
        }
    }
    
    [Button]
    public void ClearPath()
    {
        foreach (Transform pointTrans in pointTransforms)
        {
            Destroy(pointTrans.gameObject);
        }
        points.Clear();
    }
}

[System.Serializable]
public class PathData
{
    public List<Vector3> positions;
}
