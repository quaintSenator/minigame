using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldStillEffectController : MonoBehaviour
{

    protected virtual void OnEnable_deprive()
    {
        
    }

    protected virtual void OnDisable_deprive()
    {
        
    }

    // Update is called once per frame

    private void OnEnable()
    {
        EventManager.AddListener(EventType.PlayerJumpoffGroundEvent, OnJumpOff);
        EventManager.AddListener(EventType.PlayerHitGroundEvent, OnHitGround);
        OnEnable_deprive();
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.PlayerJumpoffGroundEvent, OnJumpOff);
        EventManager.RemoveListener(EventType.PlayerHitGroundEvent, OnHitGround);
        OnDisable_deprive();
    }
    
    private void OnJumpOff(EventData ed)
    {
        
    }

    private void OnHitGround(EventData ed)
    {
        
    }

}
