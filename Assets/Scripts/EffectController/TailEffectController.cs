using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class GameObjectDieEventData : EventData
{
    public GameObject go;

    public GameObjectDieEventData(GameObject gameObject)
    {
        go = gameObject;
    }
}
public class TailEffectController : HoldStillEffectController
{
    private Animator _mAnimator;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _fadingTailEffectPrefab;

    protected override void OnEnable_deprive()
    {
        Debug.LogError(transform.position);
        _mAnimator = gameObject.GetComponent<Animator>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        
        
        EventManager.AddListener(EventType.DecideCanJumpEvent, OnJumpOff);
        EventManager.AddListener(EventType.PlayerHitGroundEvent, OnHitGround);
    }

    protected override void OnDisable_deprive()
    {
        EventManager.RemoveListener(EventType.DecideCanJumpEvent, OnJumpOff);
        EventManager.RemoveListener(EventType.PlayerHitGroundEvent, OnHitGround);
    }
    private void OnJumpOff(EventData eventData)
    {
        if (_mAnimator.enabled)
        {
            _mAnimator.enabled = false;
        }

        _spriteRenderer.color = new Color(1, 1, 1, 0);
        
        var generatedFade = Instantiate(_fadingTailEffectPrefab, null);
        generatedFade.transform.position = transform.position;
        CleverTimerManager.Ask4Timer(0.25, eventData =>
        {
            var go2Die = ((GameObjectDieEventData)eventData).go;
            if(go2Die)
            {
                Destroy(go2Die);
            }
            else
            {
                Debug.Log("TailEffect nothing 2 kill");
            }
        }, new GameObjectDieEventData(generatedFade));
        /*base._initialLocalPosition = transform.localPosition;
        base._initialWorldY = transform.position.y;
         _mAnimator.Play("FrameQueue_fading");
        */
        
    }

    private void OnHitGround(EventData eventData)
    {
        _mAnimator.enabled = true;
        _mAnimator.Play("FrameQueue_play");
        _spriteRenderer.color = new Color(1, 1, 1, 0.607f);
    }
}
