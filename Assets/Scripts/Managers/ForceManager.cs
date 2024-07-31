using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChangeEventData : EventData
{
    private Vector2 afterGravityDir;
}
public class ForceManager : Singleton<ForceManager>
{
    private Vector2 gravityDir;
    private Boolean isheadingRight = true;
    void Start()
    {
        gravityDir = Vector2.down;
        isheadingRight = true;
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.GravityInverseEvent, OnGravityInversed);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.GravityInverseEvent, OnGravityInversed);
    }

    private void OnGravityInversed(EventData ed)
    {
        gravityDir = -gravityDir;
    }
    public Vector2 getGravityDir()
    {
        return gravityDir;
    }
    private void switchGravityDir()
    {
        gravityDir = -gravityDir;
    }
    public void InverseGravityManually()
    {
        EventManager.InvokeEvent(EventType.GravityInverseEvent);
    }
}
