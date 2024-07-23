using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrictionEffectController : MonoBehaviour, ICloneable
{
    [SerializeField]
    private ParticleSystem myParticleSystem;
    [SerializeField] private float minimumPlayerYDecidedAsOnGround;
    [SerializeField] private Color particleRandomColorRangeL;
    [SerializeField] private Color particleRandomColorRangeR;
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

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    private void OnPlayerJumpOff(EventData ed)
    {
        Debug.Log("SF::OnPlayerJumpOff");
        var emit = myParticleSystem.emission;
        emit.enabled = false;
    }
    private void OnPlayerHitGround(EventData ed)
    {
        Debug.Log("SF::OnPlayerHitGround");
        var emit = myParticleSystem.emission;
        emit.enabled = true;
    }
    private void OnEnable()
    {
        if (typeof(FrictionEffectController).IsAssignableFrom(typeof(ICloneable)))
        {
            Debug.Log("SF::FrictionEffectController is ICloneable.");
        }
        else
        {
            Debug.Log("SF::FrictionEffectController is not                                                                    ICloneable.");
        }
        if (typeof(FrictionEffectController).IsAssignableFrom(typeof(MonoBehaviour)))
        {
            Debug.Log("SF::FrictionEffectController is MonoBehaviour.");
        }
        
        OnRegister();
        SelfPSInit();
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.PlayerJumpoffGroundEvent, OnPlayerJumpOff);
        EventManager.RemoveListener(EventType.PlayerHitGroundEvent, OnPlayerHitGround);
    }

    void Update()
    {
        
    }
}
