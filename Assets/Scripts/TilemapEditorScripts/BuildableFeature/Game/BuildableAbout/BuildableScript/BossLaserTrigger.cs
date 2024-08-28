using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLaserTrigger : BuildableBase
{
    protected override void TriggerThisBuildable(PlayerController player)
    {
        EventManager.InvokeEvent(EventType.ReleaseLaserEvent);
    }
}
