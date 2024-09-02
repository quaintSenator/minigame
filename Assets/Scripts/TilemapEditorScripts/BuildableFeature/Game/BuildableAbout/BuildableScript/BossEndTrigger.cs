using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossEndTrigger : BuildableBase
{
    [SerializeField] private Transform triggerObj;
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
        EventManager.InvokeEvent(EventType.BossEndEvent);
    }
}
