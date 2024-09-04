using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PathMover : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();
    private List<Transform> pointTransforms = new List<Transform>();
    public bool EditorMode = false;
    private GameObject pointPrefab; // 用于生成路径点的预制体
    public string savekey = "__pathData__";

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
            pointTransforms.Add(newPoint.transform);
            points.Add(mousePos);
        }
    }

    [Button]
    public void MoveByPath()
    {
        Vector3[] positions = new Vector3[points.Count + 1];
        for (int i = 0; i < points.Count; i++)
        {
            positions[i] = points[i];
        }
        positions[points.Count] = points[0];
        transform.DOPath(positions, 5f, PathType.Linear, PathMode.TopDown2D).SetEase( Ease.Linear);
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
