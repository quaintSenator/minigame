using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletTrigger : BuildableBase
{
    protected override void TriggerThisBuildable(PlayerController player)
    {
        EventManager.InvokeEvent(EventType.ReleaseBulletEvent);
    }
}
