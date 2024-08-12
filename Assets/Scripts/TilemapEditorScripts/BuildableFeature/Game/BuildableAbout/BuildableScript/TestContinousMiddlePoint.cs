using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestContinousMiddlePoint : ContinousPoint
{
    
    public override void Init()
    {
        PointType = ContinousPointType.Middle;
        if (LastSpawnPoint == null)
        {
            Debug.LogError("LastSpawnPoint is null");
        } 
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
            if (NextPoint != null)
            {
                LastPoint.NextPoint = NextPoint;
                NextPoint.LastPoint = LastPoint;
                NextPoint.UpdateLineRenderer();
            }
            else
            {
                Debug.LogError("NextPoint is null");
            }
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
