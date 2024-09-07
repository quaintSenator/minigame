using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossMoveTrigger : BuildableBase
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
        EventManager.InvokeEvent(EventType.BossMoveEvent, new BossMoveEventData(transform.position));
    }
}


public class BossMoveEventData : EventData
{
    public Vector3 position;

    public BossMoveEventData(Vector3 position)
    {
        this.position = position;

    }
}
