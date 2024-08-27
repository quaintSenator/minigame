using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShowUpTrigger : BuildableBase
{
    protected override void TriggerThisBuildable(PlayerController player)
    {
        Debug.Log("BossShowUpTrigger");
        BossController.InitBoss();
    }
}
