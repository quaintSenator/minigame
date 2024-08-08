using UnityEngine;
using System.Collections.Generic;

public class CoordinateSystem : MonoBehaviour
{
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private bool drawGrid = true; // 是否绘制网格
    [SerializeField] private float extraDrawArea = 1f; // 额外绘制区域大小
    [SerializeField] private Color gridColor = Color.gray;
    private Transform startPoint;

    private Camera mainCamera;
    private List<GameObject> gridLines = new List<GameObject>();
    

    void Start()
    {
        mainCamera = Camera.main;
        startPoint = Utils.GetStartPointPostion();
        UpdateGridLines();
    }

    void Update()
    {
        // 每帧更新网格线
        // 但是应该也有优化空间，比如只有在摄像机移动时才更新网格线 或者只有在摄像机缩放时才更新网格线
        // 或者只更新 新进入的区域 和 离开的区域 的网格线 
        // 或者减小更新频率，比如每0.1s更新一次 额外绘制区域就是用于这个东西
        // 目前是每帧更新，不过使用了对象池，所以性能应该还可以
        if( drawGrid )
        {
            UpdateGridLines();
        }
        else if (gridLines.Count > 0)
        {
            foreach (GameObject line in gridLines)
            {
                PoolManager.Instance.ReturnToPool("CoordinateLine", line);
            }
            gridLines.Clear();
        }
    }

    void UpdateGridLines()
    {
        // 获取摄像机的左下角和右上角的世界坐标
        Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));
        
        float xOffset = startPoint.position.x % GameConsts.TILE_SIZE;
        float yOffset = startPoint.position.y % GameConsts.TILE_SIZE;
        
        // 计算需要绘制的区域
        float minX = Mathf.Floor(bottomLeft.x / GameConsts.TILE_SIZE) * GameConsts.TILE_SIZE + xOffset - extraDrawArea;
        float maxX = Mathf.Ceil(topRight.x / GameConsts.TILE_SIZE) * GameConsts.TILE_SIZE + xOffset + extraDrawArea;
        float minY = Mathf.Floor(bottomLeft.y / GameConsts.TILE_SIZE) * GameConsts.TILE_SIZE + yOffset - extraDrawArea;
        float maxY = Mathf.Ceil(topRight.y / GameConsts.TILE_SIZE) * GameConsts.TILE_SIZE + yOffset + extraDrawArea;
        
        // 清除之前的网格线
        foreach (GameObject line in gridLines)
        {
            PoolManager.Instance.ReturnToPool("CoordinateLine", line);
        }
        gridLines.Clear();
        
        // 绘制网格线
        for (float x = minX; x <= maxX; x += GameConsts.TILE_SIZE)
        {
            CreateLine(new Vector3(x, minY, 0), new Vector3(x, maxY, 0), gridColor);
        }
        for (float y = minY; y <= maxY; y += GameConsts.TILE_SIZE)
        {
            CreateLine(new Vector3(minX, y, 0), new Vector3(maxX, y, 0), gridColor);
        }
    }

    void CreateLine(Vector3 startPos, Vector3 endPos, Color color)
    {
        GameObject line = PoolManager.Instance.SpawnFromPool("CoordinateLine", linePrefab, transform);
        CoordinateLine coordinateLine = line.GetComponent<CoordinateLine>();
        coordinateLine.Init();
        coordinateLine.SetLine(startPos, endPos, color);
        gridLines.Add(line);
    }
}