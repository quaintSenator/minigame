using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestContinousStartPoint : ContinousPoint
{
    public override void Init()
    {
        PointType = ContinousPointType.Start;
        LastSpawnPoint = this;
        EventManager.InvokeEvent(EventType.DrawContinuousPointEvent, new DrawContinuousPointEventData(this));
    }

    public override void Dispose()
    {
        if (LastSpawnPoint == this)
        {
            LastSpawnPoint = null;
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
