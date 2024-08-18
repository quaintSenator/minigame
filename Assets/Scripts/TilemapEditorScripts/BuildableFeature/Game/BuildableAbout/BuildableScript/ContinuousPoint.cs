using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousPoint : BuildableBase
{
    public ContinuousPoint LastPoint;
    public ContinuousPoint NextPoint;
    private LineRenderer lineRenderer;
    
    public override void Init()
    {
        ContinuousPointInit();
    }
    
    public override void Dispose()
    {
        UnLinkPoint();
        ContinuousPointDispose();
    }

    protected virtual void ContinuousPointInit()
    {
        
    }

    protected virtual void ContinuousPointDispose()
    {
        
    }
    
    public void LinkPoint()
    {
        int groupIndex = GetBuildableGroupIndex(Index);
        LastPoint = GetLastBuildableInGroup(groupIndex, Index) as ContinuousPoint;
        NextPoint = GetNextBuildableInGroup(groupIndex, Index) as ContinuousPoint;
        if(LastPoint != null)
        {
            UpdateLineRenderer();
        }
        else
        {
            HideLineRenderer();
        }
    }
    
    private void UnLinkPoint()
    {
        if(LastPoint != null)
        {
            LastPoint.NextPoint = NextPoint;
        }
        if(NextPoint != null)
        {
            NextPoint.LastPoint = LastPoint;
        }
        LastPoint = null;
        NextPoint = null;
    }
    
    public void UpdateLineRenderer()
    {
        //生成LineRenderer,并设置起始点为LastPoint的RealPosition、终点为this的RealPosition
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Utils.GetRealPostion(LastPoint.Position));
        lineRenderer.SetPosition(1, Utils.GetRealPostion(Position));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.green;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.sortingOrder = -1;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.enabled = true;
    }
    
    public void HideLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }
}
