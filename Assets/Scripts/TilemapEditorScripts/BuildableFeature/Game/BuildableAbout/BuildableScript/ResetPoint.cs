using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 执行流程
/// 1、初始化向Player或者流程Manager动态注册重生点，之后再通过返回自身状态
/// 2、玩家经过检查点，向Player或者流程Manager，通知经过事件，永久在管理类中修改状态。
/// 
/// 关于重生点可能要考虑在地图中预先把index存一下
/// </summary>
public class ResetPoint : BuildableBase
{
    int index = -1;
    bool hasBeenChecked = false;
    
    [SerializeField]
    [Tooltip("未激活Render实例")]
    SpriteRenderer resetPointsOff = null;

    [SerializeField]
    [Tooltip("激活Render实例")]
    SpriteRenderer resetPointsOn = null;

    [SerializeField]
    [Tooltip("未激活Render实例")]
    Sprite resetPointsOffSpecial = null;

    [SerializeField]
    [Tooltip("激活Render实例")]
    Sprite resetPointsOnSpecial = null;

    public bool debugReplaceSprite = false;
    public HashSet<int> needSpacialProcessLevelIndexs = new HashSet<int>() { 3};

    public override void Init()
    {
        int currentLevelIndex = ProgressManager.Instance.GetCurrentLevelIndex();
        if(needSpacialProcessLevelIndexs.Contains(currentLevelIndex) || debugReplaceSprite)
        {
            resetPointsOff.sprite = resetPointsOffSpecial;
            resetPointsOn.sprite = resetPointsOnSpecial;
        }
        EventManager.AddListener(EventType.RegisterResetPointCallbackEvent, OnRegisterResetPointCallback);
        RegisterResetPoint();
        //RegisterResetPoint()
    }

    public void OnDisable()
    {
        EventManager.RemoveListener(EventType.RegisterResetPointCallbackEvent, OnRegisterResetPointCallback);
    }

    private void Start()
    {

    }

    protected override void TriggerThisBuildable(PlayerController player)
    {
        /*
                player.TryJump(JumpType.Spring);
                Debug.Log("OnSpringTrigger");
                hasBeenUsedForJump = true;*/
        //EventManager.InvokeEvent(EventType.RegisterResetPointEvent, registerResetPointEventData);

        PlayerPassRegisterResetPointEvent playerPassRegisterResetPointEvent = new PlayerPassRegisterResetPointEvent();
        playerPassRegisterResetPointEvent.index = index;
        EventManager.InvokeEvent(EventType.PlayerPassRegisterResetPointEvent, playerPassRegisterResetPointEvent);

        ChangeCheckState(true);
    }



    //复活点是随着地图加载出的，所有初始化需要注册一下
    //TODO:目前是在Player中注册，之后又统一的管理类之后，在这个管理类中注册
    private void RegisterResetPoint()
    {
        RegisterResetPointEventData registerResetPointEventData= new RegisterResetPointEventData();

        Transform resetPointTranform= transform;
        Vector3 resetPointPosition = resetPointTranform.position;
       // resetPointPosition.x = resetPointPosition.x ;


        registerResetPointEventData.position = resetPointPosition;

        EventManager.InvokeEvent(EventType.RegisterResetPointEvent, registerResetPointEventData);
    }

    private void OnRegisterResetPointCallback(EventData eventData)
    {
        RegisterResetPointCallbackEventData registerResetPointCallbackEventData= eventData as RegisterResetPointCallbackEventData;
        if (registerResetPointCallbackEventData != null)
        {
            index = registerResetPointCallbackEventData.index;

            ChangeCheckState(registerResetPointCallbackEventData.state);
            //hasBeenChecked = registerResetPointCallbackEventData.state;

        }
    }


    private void ChangeCheckState(bool newState)
    {
        if (resetPointsOff == null || resetPointsOn == null)
        {
            Debug.LogError("Can not get right render in resetPoint ,please check");
            return;
        }

        if (newState && !hasBeenChecked)
        {
            hasBeenChecked = true;
            resetPointsOff.enabled = false;
            resetPointsOn.enabled = true;
        }

/*        //应该不会出现这种状态
        if (!newState && hasBeenChecked)
        {
            hasBeenChecked = false;
            resetPointsOff.enabled = true;
            resetPointsOn.enabled = false;
        }*/
    }

}