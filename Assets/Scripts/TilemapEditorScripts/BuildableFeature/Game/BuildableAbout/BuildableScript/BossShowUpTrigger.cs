using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossShowUpTrigger : BuildableBase
{
    protected override void TriggerThisBuildable(PlayerController player)
    {
        if (SceneManager.GetActiveScene().name == "TilemapEditorScene")
        {
            TilemapCameraController.Instance.ChangeCamera(true);
        }
        BossController.InitBoss();
    }
}
