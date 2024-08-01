using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringController : BuildableBase {

    [SerializeField]
    private JumpSettings jumpSettings = GameConsts.SPRING_JUMP;

    protected override void TriggerThisBuildable(PlayerController player)
    {
        player.TryJump(jumpSettings, true);
        Debug.Log("OnSpringTrigger");
    }

}