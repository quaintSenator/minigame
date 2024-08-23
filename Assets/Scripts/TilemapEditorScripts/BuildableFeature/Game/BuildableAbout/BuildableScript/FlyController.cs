using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//代码已废弃*************************************************

public class FlyController : BuildableBase {
    private Transform startPoint;
    private Transform endPoint;
    private float vVelocity;

    private static FlyController firstNode;

    public PlayerController player;
    private void Awake() {
        Init();
    }

    public override void Init() {
        startPoint = transform.Find("beginning");
        endPoint = transform.Find("destination");
        vVelocity = CalVerticalVelocity();
    }

    private float CalVerticalVelocity()
    {
        Vector3 startPos = startPoint.position;
        Vector3 endPos = endPoint.position;
        float hDistance = endPos.x - startPos.x;
        float vDistance = endPos.y - startPos.y;
        float time = hDistance / GameConsts.SPEED;
        float vSpeed = vDistance / time;
        Debug.Log("vSpeed"+vSpeed);
        return vSpeed;
    }

    public void SendVelocity()
    {
        player.SetVerticalVelocity(vVelocity);
    }
    
}