using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HitGroundEventData : EventData
{
    public Vector2 velocityDir;

    public HitGroundEventData(Vector2 vec)
    {
        velocityDir = vec;
    }
}
public class FrictionEffectController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem myParticleSystem;
    [SerializeField] public Transform playerTransform;
    [SerializeField] private float minimumPlayerYDecidedAsOnGround;
    [SerializeField] private Color particleRandomColorRangeL;
    [SerializeField] private Color particleRandomColorRangeR;
    [SerializeField] private float fricThrowAngle = 15.0f;
    void SelfPSInit()
    {
        var myPSMain = myParticleSystem.main;
        ParticleSystem.MinMaxGradient startColorRange = new ParticleSystem.MinMaxGradient(particleRandomColorRangeL, particleRandomColorRangeR);
        myPSMain.startColor = startColorRange;
    }
    private void OnRegister()
    {
        EventManager.AddListener(EventType.PlayerJumpoffGroundEvent, OnPlayerJumpOff);
        EventManager.AddListener(EventType.PlayerHitGroundEvent, OnPlayerHitGround);
    }

    private void OnPlayerJumpOff(EventData ed)
    {
        Debug.Log("SF::OnPlayerJumpOff");
        var emit = myParticleSystem.emission;
        emit.enabled = false;
    }
    private void OnPlayerHitGround(EventData ed)
    {
        var fmanager = ForceManager.Instance;
        HitGroundEventData hitGroundEventData = (HitGroundEventData)ed;
        float rotationX = fmanager.getFrictionThrowingRotationAngle(
            hitGroundEventData.velocityDir, fmanager.getGravityDir(), fricThrowAngle);
        transform.rotation = Quaternion.Euler(rotationX, -90.0f, 0);
        var emit = myParticleSystem.emission;
        emit.enabled = true;
    }
    private void OnEnable()
    {
        OnRegister();
        SelfPSInit();
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.PlayerJumpoffGroundEvent, OnPlayerJumpOff);
        EventManager.RemoveListener(EventType.PlayerHitGroundEvent, OnPlayerHitGround);
    }

    //在重力改变、跳跃或飞行落地等情况下，通过设定发射体的rotation调整抛射角度
    private void setEmittorShapeRotation()
    {
        
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            
            EventManager.InvokeEvent(EventType.PlayerHitGroundEvent, new HitGroundEventData(Vector2.right));
        }

        if (Input.GetKey(KeyCode.A))
        {
            ForceManager.Instance.switchGravityDir();
            EventManager.InvokeEvent(EventType.PlayerHitGroundEvent, new HitGroundEventData(Vector2.right));
        }
    }
}
