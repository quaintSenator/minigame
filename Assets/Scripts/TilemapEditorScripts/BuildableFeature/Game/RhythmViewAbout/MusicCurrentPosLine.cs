using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicCurrentPosLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private BoxCollider2D boxCollider2D;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        lineRenderer.enabled = false;
        boxCollider2D.enabled = false;
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
        transform.position = new Vector3(currentPos.x, 0, 0);
        lineRenderer.SetPosition(0, new Vector3(currentPos.x, 30, 0));
        lineRenderer.SetPosition(1, new Vector3(currentPos.x, -30, 0));
    }
    
    public void ShowPosLine()
    {
        lineRenderer.enabled = true;
        boxCollider2D.enabled = true;
        UpdatePos();
    }
    
    public void HidePosLine()
    {
        lineRenderer.enabled = false;
        boxCollider2D.enabled = false;
    }
}
