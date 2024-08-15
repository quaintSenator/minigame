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
    
    public bool test_is_moving_right;
    void SelfPSInit()
    {
        var myPSMain = myParticleSystem.main;
        ParticleSystem.MinMaxGradient startColorRange = new ParticleSystem.MinMaxGradient(particleRandomColorRangeL, particleRandomColorRangeR);
        myPSMain.startColor = startColorRange;
    }
    
    private void OnGravityInverse(EventData ed)
    {
        setPSGravity(ForceManager.Instance.GetGravityDir());
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
        //开喷
        var emit = myParticleSystem.emission;
        emit.enabled = true;
    }
    private void setPSGravity(Vector2 gravity)
    {
        var forceModule = myParticleSystem.forceOverLifetime;
        forceModule.y = gravity.y < 0 ? -20.0f : 20.0f;
    }

    protected override void OnEnable_deprive()
    {
        SelfPSInit();
        test_is_moving_right = true;
        EventManager.AddListener(EventType.PlayerJumpoffGroundEvent, OnPlayerJumpOff);
        EventManager.AddListener(EventType.PlayerHitGroundEvent, OnPlayerHitGround);
        EventManager.AddListener(EventType.GravityInverseEvent, OnGravityInverse);
    }
    protected override void OnDisable_deprive()
    {
        EventManager.RemoveListener(EventType.PlayerJumpoffGroundEvent, OnPlayerJumpOff);
        EventManager.RemoveListener(EventType.PlayerHitGroundEvent, OnPlayerHitGround);
        EventManager.RemoveListener(EventType.GravityInverseEvent, OnGravityInverse);
    }
}
