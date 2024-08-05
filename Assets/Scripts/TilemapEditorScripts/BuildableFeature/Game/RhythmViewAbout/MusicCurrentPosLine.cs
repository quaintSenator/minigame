using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicCurrentPosLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (lineRenderer.enabled && RhythmViewer.CurrentMusicIsPlaying)
        {
            lineRenderer.SetPosition(0, new Vector3(RhythmViewer.CurrentMusicTime * GameConsts.SPEED, 10, 0));
            lineRenderer.SetPosition(1, new Vector3(RhythmViewer.CurrentMusicTime * GameConsts.SPEED, -3, 0));
        }
    }
    
    public void ShowPosLine()
    {
        lineRenderer.enabled = true;
    }
    
    public void HidePosLine()
    {
        lineRenderer.enabled = false;
    }
}
