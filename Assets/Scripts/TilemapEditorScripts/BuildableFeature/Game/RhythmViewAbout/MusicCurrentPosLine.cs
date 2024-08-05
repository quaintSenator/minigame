using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicCurrentPosLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private float currentTime = 0f;
    private bool pause = false;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (lineRenderer.enabled && !pause)
        {
            currentTime += Time.deltaTime;
            lineRenderer.SetPosition(0, new Vector3(currentTime * GameConsts.SPEED, 10, 0));
            lineRenderer.SetPosition(1, new Vector3(currentTime * GameConsts.SPEED, -3, 0));
        }
    }
    
    public void StartRecordTime()
    {
        currentTime = 0f;
        lineRenderer.enabled = true;
    }
    
    public void StopRecordTime()
    {
        lineRenderer.enabled = false;
    }
    
    public void PauseRecordTime()
    {
        pause = true;
    }
    
    public void ResumeRecordTime()
    {
        pause = false;
    }
}
