using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum ContinousPointType
{
    Start = 1,
    Middle = 2,
    End = 3,
}

public class ContinousPoint : BuildableBase
{
    public static ContinousPoint LastSpawnPoint = null; //上一个生成的连续点 
    public ContinousPointType PointType; //连续点类型
    public ContinousPoint LastPoint; //上一个连续点
    public ContinousPoint NextPoint; //下一个连续点
    
    protected LineRenderer lineRenderer;
    
    public override void RegisterEvent()
    {
        EventManager.AddListener(EventType.DrawContinuousPointEvent, DrawContinuousPoint);
        EventManager.AddListener(EventType.EndDrawContinuousPointEvent, EndDrawContinuousPoint);
        EventManager.AddListener(EventType.EraseContinuousPointEvent, EraseContinuousPoint);
    }
    
    public override void UnRegisterEvent()
    {
        EventManager.RemoveListener(EventType.DrawContinuousPointEvent, DrawContinuousPoint);
        EventManager.RemoveListener(EventType.EndDrawContinuousPointEvent, EndDrawContinuousPoint);
        EventManager.RemoveListener(EventType.EraseContinuousPointEvent, EraseContinuousPoint);
    }

    protected virtual void DrawContinuousPoint(EventData obj)
    {
    }

    protected virtual void EndDrawContinuousPoint(EventData obj)
    {
    }

    protected virtual void EraseContinuousPoint(EventData obj)
    {
    }

    protected override void TriggerThisBuildable(PlayerController player)
    {
        //TODO 触发功能
    }
    
    public ContinousPointType GetPointType()
    {
        return PointType;
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
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }
}

public class DrawContinuousPointEventData : EventData
{
    public ContinousPoint Point;
    public DrawContinuousPointEventData(ContinousPoint point)
    {
        Point = point;
    }
}

public class EraseContinuousPointEventData : EventData
{
    public ContinousPoint Point;

    public EraseContinuousPointEventData(ContinousPoint point)
    {
        Point = point;
    }
}
