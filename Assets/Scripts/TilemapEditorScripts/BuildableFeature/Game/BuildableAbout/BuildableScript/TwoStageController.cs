using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoStageController : BuildableBase
{
    protected override void TriggerThisBuildable(PlayerController player)
    {
        player.SetIsGrounded(true);
    }

    protected override void TriggerOffThisBuildable(PlayerController player)
    {
        player.SetIsGrounded(false);
    }
}