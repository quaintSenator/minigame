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
            UpdatePos();
        }
    }

    void UpdatePos()
    {
        Vector3 currentPos = RhythmViewer.Instance.GetCurrentMusicLinePos();
        lineRenderer.SetPosition(0, new Vector3(currentPos.x, 10, 0));
        lineRenderer.SetPosition(1, new Vector3(currentPos.x, -10, 0));
    }
    
    public void ShowPosLine()
    {
        lineRenderer.enabled = true;
        UpdatePos();
    }
    
    public void HidePosLine()
    {
        lineRenderer.enabled = false;
    }
}
