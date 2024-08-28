using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShowUpTrigger : BuildableBase
{
    protected override void TriggerThisBuildable(PlayerController player)
    {
        BossController.InitBoss();
    }
}
