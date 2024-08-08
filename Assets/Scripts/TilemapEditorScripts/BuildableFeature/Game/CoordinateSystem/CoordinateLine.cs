using UnityEngine;

public class CoordinateLine : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public void Init()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = true;
    }

    public void SetLine(Vector3 startPos, Vector3 endPos, Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
}