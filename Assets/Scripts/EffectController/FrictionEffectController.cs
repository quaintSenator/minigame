using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HitGroundEventData : EventData
{
    public Collider2D other;
    public string hitType;
    public HitGroundEventData(Collider2D o)
    {
        other = o;
        hitType = "";
    }

    public HitGroundEventData(Collider2D o, string tp)
    {
        other = o;
        hitType = tp;
    }
}
public class FrictionEffectController : HoldStillEffectController
{
    [SerializeField]
    private ParticleSystem myParticleSystem;
    [SerializeField] public Transform playerTransform;
    [SerializeField] private float minimumPlayerYDecidedAsOnGround;
    [SerializeField] private Color particleRandomColorRangeL;
    [SerializeField] private Color particleRandomColorRangeR;
    [SerializeField] private float fricThrowAngle = 15.0f;
    [SerializeField] private float cubeEdgeLen = 0.45f;
    [SerializeField] private float FrictionTimeLen = 0.4f;
    [SerializeField] private Transform player;
    
    void SelfPSInit()
    {
        var myPSMain = myParticleSystem.main;
        ParticleSystem.MinMaxGradient startColorRange = new ParticleSystem.MinMaxGradient(particleRandomColorRangeL, particleRandomColorRangeR);
        myPSMain.startColor = startColorRange;
    }
    //根据重力和摩擦力计算抛洒粒子方向
    private float GetFrictionThrowingRotationAngle(Vector2 v, Vector2 g, float realAngle)
    {
        if (v.x > 0)
        {
            if (g.y < 0)
            {
                return -realAngle;
            }
            else
            {
                return realAngle;
            }
        }
        else
        {
            if (g.y > 0)
            {
                return 180.0f - realAngle;
            }
            else
            {
                return -(180.0f - realAngle);
            }
        }
    }
    
    private void OnPlayerJumpOff(EventData ed)
    {
        StopSpit();
    }

    protected override void ResetLocalPosition()
    {
        transform.localPosition = new Vector3(-0.45f, -0.31f, -0.419f);
        transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    private void Spit()
    {
        //开喷
        var emit = myParticleSystem.emission;
        emit.enabled = true;
    }
    private void StopSpit()
    {
        //断喷
        var emit = myParticleSystem.emission;
        emit.enabled = false;
    }
    private void OnPlayerHitGround(EventData ed)
    {
        //设置transform.rotation, 用于调整喷射方向
        var fmanager = ForceManager.Instance;
        var hitGroundEventData = (HitGroundEventData)ed;
        /*var rotationX = GetFrictionThrowingRotationAngle(
            hitGroundEventData.velocityDir, fmanager.GetGravityDir(), fricThrowAngle);
        transform.rotation = Quaternion.Euler(rotationX, -90.0f, 0);*/
        
        //调节相对顶层节点位置
        Vector3 localPosition2Set = new Vector3();
        localPosition2Set.x = -cubeEdgeLen;
        localPosition2Set.y = -cubeEdgeLen;
        transform.localPosition = localPosition2Set;
        Spit();
        base.PickUpPlayer();
    }
    private void setPSGravity(Vector2 gravity)
    {
        var forceModule = myParticleSystem.forceOverLifetime;
        forceModule.y = gravity.y < 0 ? -20.0f : 20.0f;
    }

    private void OnRestart(EventData eventData)
    {
        Spit();
    }
    protected override void OnEnable_deprive()
    {
        SelfPSInit();
        EventManager.AddListener(EventType.PlayerJumpoffGroundEvent, OnPlayerJumpOff);
        EventManager.AddListener(EventType.PlayerHitGroundEvent, OnPlayerHitGround);
        EventManager.AddListener(EventType.GameRestartEvent, OnRestart);
    }
    protected override void OnDisable_deprive()
    {
        EventManager.RemoveListener(EventType.PlayerJumpoffGroundEvent, OnPlayerJumpOff);
        EventManager.RemoveListener(EventType.PlayerHitGroundEvent, OnPlayerHitGround);
        EventManager.RemoveListener(EventType.GameRestartEvent, OnRestart);
    }
}
