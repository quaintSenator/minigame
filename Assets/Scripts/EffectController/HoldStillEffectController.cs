using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldStillEffectController : MonoBehaviour
{
    private Transform playerTransform;
    private void HoldPosition()
    {
        transform.parent = null;
    }

    protected void PickUpPlayer()
    {
        transform.parent = playerTransform;
        ResetLocalPosition();
    }
    protected virtual void OnEnable_deprive()
    {
        
    }

    protected virtual void OnDisable_deprive()
    {
        
    }

    protected virtual void ResetLocalPosition()
    {
        transform.localPosition = new Vector3(0, 0, 0);
    }

    // Update is called once per frame

    private void OnEnable()
    {
        if (!playerTransform)
        {
            playerTransform = GameObject.Find("Player").transform;
        }
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
        HoldPosition();
    }

    private void OnHitGround(EventData ed)
    {
        
    }

}
