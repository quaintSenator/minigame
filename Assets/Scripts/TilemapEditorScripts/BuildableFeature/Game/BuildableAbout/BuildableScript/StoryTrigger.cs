using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTrigger : BuildableBase
{
    public override void Init()
    {

    }
    protected override void TriggerThisBuildable(PlayerController player)
    {
        DialogManager.Instance.TryShowDialogs();
    }

    protected override void TriggerOffThisBuildable(PlayerController player)
    {

    }
}