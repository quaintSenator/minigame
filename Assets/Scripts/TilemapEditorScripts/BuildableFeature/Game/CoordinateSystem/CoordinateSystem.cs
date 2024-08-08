using UnityEngine;
using System.Collections.Generic;

public class CoordinateSystem : MonoBehaviour
{
    [SerializeField] private GameObject linePrefab; // Reference to the CoordinateLine prefab
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private Color gridColor = Color.gray;
    //额外画的区域
    [SerializeField] private float extraDrawArea = 1f;
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
        UpdateGridLines();
    }

    void UpdateGridLines()
    {
        // Get camera boundaries
        Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));
        Debug.Log("bottomLeft: " + bottomLeft);
        Debug.Log("topRight: " + topRight);
        
        float xOffset = startPoint.position.x % tileSize;
        float yOffset = startPoint.position.y % tileSize;
        
        // Compute the range of coordinates to cover a bit beyond the camera view
        float minX = Mathf.Floor(bottomLeft.x / tileSize) * tileSize + xOffset - extraDrawArea;
        float maxX = Mathf.Ceil(topRight.x / tileSize) * tileSize + xOffset + extraDrawArea;
        float minY = Mathf.Floor(bottomLeft.y / tileSize) * tileSize + yOffset - extraDrawArea;
        float maxY = Mathf.Ceil(topRight.y / tileSize) * tileSize + yOffset + extraDrawArea;
        
        // Clear the grid lines
        foreach (GameObject line in gridLines)
        {
            PoolManager.Instance.ReturnToPool("CoordinateLine", line);
        }
        gridLines.Clear();
        
        // Draw the grid lines
        for (float x = minX; x <= maxX; x += tileSize)
        {
            CreateLine(new Vector3(x, minY, 0), new Vector3(x, maxY, 0), gridColor);
        }
        for (float y = minY; y <= maxY; y += tileSize)
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