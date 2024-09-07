using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossShowUpTrigger : BuildableBase
{
    
    [SerializeField] private Transform triggerObj;


    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "TilemapEditorScene")
        {
            triggerObj.gameObject.SetActive(true);
        }
        else
        {
            triggerObj.gameObject.SetActive(false);
        }
    }

    protected override void TriggerThisBuildable(PlayerController player)
    {
        if (SceneManager.GetActiveScene().name == "TilemapEditorScene")
        {
            TilemapCameraController.Instance.ChangeCamera(true);
            triggerObj.gameObject.SetActive(true);
        }
        else
        {
            triggerObj.gameObject.SetActive(false);
        }
        BossController.InitBoss();
    }
}
