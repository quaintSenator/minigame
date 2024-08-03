using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicCurrentPosLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    float currentTime = 0f;
    
    private void OnEnable()
    {
        EventManager.AddListener(EventType.MusicStartEvent, StartRecordTime);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.MusicStartEvent, StartRecordTime);
    }
    
    public void StartRecordTime(EventData data)
    {
        lineRenderer.enabled = true;
    }
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (lineRenderer.enabled)
        {
            currentTime += Time.deltaTime;
            lineRenderer.SetPosition(0, new Vector3(currentTime * GameConsts.SPEED, 10, 0));
            lineRenderer.SetPosition(1, new Vector3(currentTime * GameConsts.SPEED, -3, 0));
        }
    }
}
