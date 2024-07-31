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
    public bool test_is_moving_right;
    void SelfPSInit()
    {
        var myPSMain = myParticleSystem.main;
        ParticleSystem.MinMaxGradient startColorRange = new ParticleSystem.MinMaxGradient(particleRandomColorRangeL, particleRandomColorRangeR);
        myPSMain.startColor = startColorRange;
    }
    
    private void OnGravityInverse(EventData ed)
    {
        setPSGravity(ForceManager.Instance.getGravityDir());
    }
    
    //根据重力和摩擦力计算抛洒例子方向
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
        Debug.Log("SF::OnPlayerJumpOff");
        var emit = myParticleSystem.emission;
        emit.enabled = false;
    }
    private void OnPlayerHitGround(EventData ed)
    {
        var fmanager = ForceManager.Instance;
        HitGroundEventData hitGroundEventData = (HitGroundEventData)ed;
        float rotationX = GetFrictionThrowingRotationAngle(
            hitGroundEventData.velocityDir, fmanager.getGravityDir(), fricThrowAngle);
        transform.rotation = Quaternion.Euler(rotationX, -90.0f, 0);
        
        //开喷
        var emit = myParticleSystem.emission;
        emit.enabled = true;
    }
    
    private void setPSGravity(Vector2 gravity)
    {
        var forceModule = myParticleSystem.forceOverLifetime;
        forceModule.y = gravity.y < 0 ? -20.0f : 20.0f;
    }
    private void OnEnable()
    {
        SelfPSInit();
        test_is_moving_right = true;
        EventManager.AddListener(EventType.PlayerJumpoffGroundEvent, OnPlayerJumpOff);
        EventManager.AddListener(EventType.PlayerHitGroundEvent, OnPlayerHitGround);
        EventManager.AddListener(EventType.GravityInverseEvent, OnGravityInverse);
    }
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.PlayerJumpoffGroundEvent, OnPlayerJumpOff);
        EventManager.RemoveListener(EventType.PlayerHitGroundEvent, OnPlayerHitGround);
        EventManager.RemoveListener(EventType.GravityInverseEvent, OnGravityInverse);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            test_is_moving_right = !test_is_moving_right;
            EventManager.InvokeEvent(EventType.PlayerHitGroundEvent, 
                new HitGroundEventData(test_is_moving_right? Vector2.right : Vector2.left));
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            EventManager.InvokeEvent(EventType.GravityInverseEvent, null);
        }
    }
}
