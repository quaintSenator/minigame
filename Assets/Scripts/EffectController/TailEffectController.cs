using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class GameObjectDieEventData : EventData
{
    public GameObject sender;
    public GameObject go2kill;
    
    public GameObjectDieEventData(GameObject sdor, GameObject gameObject)
    {
        go2kill = gameObject;
        sender = sdor;
    }
}

public class KeepSlidingEventData : EventData
{
    public TailEffectController self;

    public KeepSlidingEventData(TailEffectController s)
    {
        self = s;
    }
}
public class TailEffectController : HoldStillEffectController
{
    private Animator _mAnimator;
    private SpriteRenderer _spriteRenderer;
    private bool _isHiding;
    [SerializeField] private float _delayBe4Show;
    [SerializeField] private GameObject _fadingTailEffectPrefab;

    public double lastOffTime;
    public double lastHitTime;

    private void HideSprite()
    {
        _isHiding = true;
        _mAnimator.enabled = false;
        _spriteRenderer.color = new Color(1, 1, 1, 0);
    }

    private void ShowSprite()
    {
        _isHiding = false;
        _mAnimator.enabled = true;
        _spriteRenderer.color = new Color(1, 1, 1, 0.607f);
        _mAnimator.Play("FrameQueue_play");
    }

    private void ShowSpriteInTime(float t)
    {
        CleverTimerManager.Ask4Timer(t, OnShowSpriteTimeup, new KeepSlidingEventData(this));
    }

    private void OnShowSpriteTimeup(EventData eventData)
    {
        var s = ((KeepSlidingEventData)eventData).self;
        if (Time.timeAsDouble - lastOffTime > _delayBe4Show)
        {
            s.ShowSprite();
        }
    }

    private void OnGameRestart(EventData eventData)
    {
        ShowSprite();
    }
    protected override void OnEnable_deprive()
    {
        _mAnimator = gameObject.GetComponent<Animator>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _isHiding = false;
        EventManager.AddListener(EventType.DecideCanJumpEvent, OnJump);
        EventManager.AddListener(EventType.PlayerJumpoffGroundEvent, OnOff);
        EventManager.AddListener(EventType.PlayerHitGroundEvent, OnHitGround);
        EventManager.AddListener(EventType.GameRestartEvent, OnGameRestart);
    }

    protected override void OnDisable_deprive()
    {
        EventManager.RemoveListener(EventType.DecideCanJumpEvent, OnJump);
        EventManager.RemoveListener(EventType.PlayerJumpoffGroundEvent, OnOff);
        EventManager.RemoveListener(EventType.PlayerHitGroundEvent, OnHitGround);
        EventManager.RemoveListener(EventType.GameRestartEvent, OnGameRestart);
    }

    private void OnOff(EventData eventData)
    {
        lastOffTime = Time.timeAsDouble;
        HideSprite();
    }
    private void OnJump(EventData eventData)
    {
        if (!_isHiding)
        {
            var generatedFade = Instantiate(_fadingTailEffectPrefab, null);
            generatedFade.transform.position = transform.position;
            CleverTimerManager.Ask4Timer(1.2, eventData =>
            {
                var go2Die = ((GameObjectDieEventData)eventData).go2kill;
                if(go2Die)
                {
                    Destroy(go2Die);
                }
                else
                {
                    Debug.Log("TailEffect nothing 2 kill");
                }
            }, new GameObjectDieEventData(gameObject, generatedFade));
        }
    }

    private void OnHitGround(EventData eventData)
    {
        var hitGroundED = (HitGroundEventData)eventData;
        if (hitGroundED.other.gameObject.name.Contains("floor_1"))
        {
            ShowSprite();
        }
        //lastHitTime = Time.timeAsDouble;
        //ShowSpriteInTime(_delayBe4Show);
    }
}
