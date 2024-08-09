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
            jumpSettings = new JumpSettings(JumpSettings.settings[JumpType.Spring]); 
        }
    }

    private void Start()
    {
        if (useDefault)
        { 
            jumpSettings = new JumpSettings(JumpSettings.settings[JumpType.Spring]);
        }       
    }

    protected override void TriggerThisBuildable(PlayerController player)
    {
        player.TryJump(JumpType.Spring, jumpSettings);
        Debug.Log("OnSpringTrigger");
    }

}