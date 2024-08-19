using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringController : BuildableBase
{

    bool hasBeenUsedForJump = false;

    public override void Init()
    {

    }

    private void Start()
    {

    }

    protected override void TriggerThisBuildable(PlayerController player)
    {
/*        if(!hasBeenUsedForJump)
        {*/
            player.TryJump(JumpType.Spring);
            hasBeenUsedForJump = true;
       // }

    }

}