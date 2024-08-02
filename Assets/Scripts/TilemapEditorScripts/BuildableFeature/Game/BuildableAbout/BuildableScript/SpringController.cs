using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringController : BuildableBase
{

    [SerializeField]
    private JumpSettings jumpSettings = new JumpSettings();
    [SerializeField] private bool useDefault = true;

    public override void Init()
    {
        if (useDefault)
        { 
            jumpSettings = GameConsts.SPRING_JUMP; 
        }
    }

    private void Start()
    {
        if (useDefault)
        { 
            jumpSettings = GameConsts.SPRING_JUMP; 
        }       
    }

    protected override void TriggerThisBuildable(PlayerController player)
    {
        player.TryJump(jumpSettings, true);
        Debug.Log("OnSpringTrigger");
    }

}