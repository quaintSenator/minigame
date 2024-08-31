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
        transform.position = new Vector3(currentPos.x, currentPos.y, 0);
        if (TilemapCameraController.MoveDirection == Direction.Right)
        {
            lineRenderer.SetPosition(0, new Vector3(currentPos.x, 30 + currentPos.y, 0));
            lineRenderer.SetPosition(1, new Vector3(currentPos.x, -30 + currentPos.y, 0));
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            lineRenderer.SetPosition(0, new Vector3(currentPos.x + 30, currentPos.y, 0));
            lineRenderer.SetPosition(1, new Vector3(currentPos.x - 30, currentPos.y, 0));
        }
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
    
    public void ResumePosLine()
    {
        boxCollider2D.enabled = true;
    }
    
    public void PausePosLine()
    {
        boxCollider2D.enabled = false;
    }
}
