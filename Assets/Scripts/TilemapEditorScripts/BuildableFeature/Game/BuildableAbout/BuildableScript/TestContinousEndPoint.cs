using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestContinousEndPoint : ContinousPoint
{
    public override void Init()
    {
        PointType = ContinousPointType.End;
        LastSpawnPoint.NextPoint = this;
        LastPoint = LastSpawnPoint;
        LastSpawnPoint = this;
        UpdateLineRenderer();
    }

    public override void Dispose()
    {
        if (LastSpawnPoint == this)
        {
            LastPoint.NextPoint = null;
            LastSpawnPoint = LastPoint;
        }
        else
        {
            Debug.LogError("LastSpawnPoint is not EndPoint");
        }
    }

    protected override void DrawContinuousPoint(EventData obj)
    {
        
    }

    protected override void EndDrawContinuousPoint(EventData obj)
    {
        
    }
    
    protected override void EraseContinuousPoint(EventData obj)
    {
        
    }
}
