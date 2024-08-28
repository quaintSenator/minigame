using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossLaserTrigger : BuildableBase
{
    [SerializeField] private float continueTime = 0.5f;
    [SerializeField] private Transform previewObj;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "TilemapEditorScene")
        {
            previewObj.localPosition = new Vector3((-0.5f + continueTime * GameConsts.SPEED)/2, 0, 0);
            previewObj.localScale = new Vector3(continueTime * GameConsts.SPEED, 0.5f, 0);
            previewObj.gameObject.SetActive(true);
        }
        else
        {
            previewObj.gameObject.SetActive(false);
        }
    }
    protected override void TriggerThisBuildable(PlayerController player)
    {
        EventManager.InvokeEvent(EventType.ReleaseLaserEvent, new ReleaseLaserEvent(transform.position, continueTime));
    }
}

public class ReleaseLaserEvent : EventData
{
    public Vector3 position;
    public float continueTime;
    public ReleaseLaserEvent(Vector3 position, float continueTime)
    {
        this.position = position;
        this.continueTime = continueTime;
    }
}
