using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 针对场景地图加载流程和同步音乐的流程管理类
/// </summary>
public class FlowManager : Singleton<FlowManager>
{
    //全局
    private bool ifFirstEnterLevel = false;
    private bool ifPlayStortEveryTime = false;

    private bool shouldPlayStory = false;

    //单次进入Level的
    private bool ifStartLevelEvent = false;

    
    private int DeadCount = 0;


    [SerializeField]
    private bool debugSkipStoryEvent = false;

    [SerializeField]
    private bool debugSkipPlayerDeadStoryEvent = false;

    [SerializeField]
    private bool debugSkipPlayerDeadEvent = false;


    //用于每次流程控制
    private LevelEventData lastLevelEventData = null;

    //EnterGameEvent
    //StartStoryEvent
    //EndStoryEvent
    //StartLoadBankEvent:for audio load
    //EndLoadBankEvent
    //StartLoadMapEvent
    //EndLoadMapEvent
    protected override void OnAwake()
    {
        InitFlowSetting();
    }
	
	protected override bool NeedDestory()
    {
        return true;
    }

    private void Start()
    {
        StartFlow();
    }

    private void OnEnable()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }


    private void RegisterEvents()
    {
        EventManager.AddListener(EventType.EndLoadBankEvent, OnEndLoadBankEvent);
        EventManager.AddListener(EventType.EndStoryEvent, OnEndStoryEvent);
        
        EventManager.AddListener(EventType.PlayerDeadStoryEvent, OnPlayerDeadStoryEvent);
        EventManager.AddListener(EventType.EndPlayerDeadStoryEvent, OnEndPlayerDeadStoryEvent);
        EventManager.AddListener(EventType.EndRespawnEvent, OnEndRespawnEvent);

    }

    private void UnregisterEvents()
    {
        EventManager.RemoveListener(EventType.EndLoadBankEvent, OnEndLoadBankEvent);
        EventManager.RemoveListener(EventType.EndStoryEvent, OnEndStoryEvent);

        //EventManager.RemoveListener(EventType.EndLoadMapEvent, OnEndLoadMapEvent);

        EventManager.RemoveListener(EventType.PlayerDeadStoryEvent, OnPlayerDeadStoryEvent);
        EventManager.RemoveListener(EventType.EndPlayerDeadStoryEvent, OnEndPlayerDeadStoryEvent);
        EventManager.RemoveListener(EventType.EndRespawnEvent, OnEndRespawnEvent);

    }

    private void InitFlowSetting()
    {
        //TODO: now seem nothing will init here
    }

    private void StartFlow()
    {
        //TODO：从其他地方获取到额外信息
        //额外信息：是否是第一次打开关卡
        if(ProgressManager.Instance != null)
        {
            ifFirstEnterLevel = true;//TODO
        }

        shouldPlayStory = ifFirstEnterLevel;
        shouldPlayStory |= (!ifFirstEnterLevel && ifFirstEnterLevel);


        if (shouldPlayStory && !debugSkipStoryEvent)
        {
            EventManager.InvokeEvent(EventType.StartStoryEvent);
            return;
        }
        else
        {
            EventManager.InvokeEvent(EventType.StartLevelEvent);
            //EventManager.InvokeEvent(EventType.StartLoadBankEvent);
            return;
        }

    }

    private void OnEndLoadBankEvent(EventData eventData)
    {
        EventManager.InvokeEvent(EventType.StartLevelEvent);
    }

    private void OnEndStoryEvent(EventData eventData)
    {
        EventManager.InvokeEvent(EventType.StartLevelEvent);
    }

    private void OnEndRespawnEvent(EventData eventData)
    {
        EventManager.InvokeEvent(EventType.RestartLevelEvent, lastLevelEventData);

    }

    private void OnPlayerDeadStoryEvent(EventData eventData)
    {
        LevelEventData levelEventData= eventData as LevelEventData;
        if (levelEventData ==null)
        {
            Debug.LogWarning("FlowManager :: OnPlayerDeadStoryEvent : can not get right event data ");
            //return;
        }
        lastLevelEventData = levelEventData;
        
        if (DeadCount == 0 || DeadCount >= 20)
        {
            DeadCount %= 20;
            WindowManager.Instance.CallDeadPage(null);
        }
        else//直接复活
        {
            if(debugSkipPlayerDeadStoryEvent)
            {
                EventManager.InvokeEvent(EventType.StartPlayerDeadEvent);
            }
        }
        DeadCount++;
        /*if(debugSkipPlayerDeadStoryEvent)
        {
            EventManager.InvokeEvent(EventType.StartPlayerDeadEvent);
        }*/
    }

    private void OnEndPlayerDeadStoryEvent(EventData eventData)
    {
        EventManager.InvokeEvent(EventType.StartPlayerDeadEvent);
    }


    private void OnEndPlayerDeadEvent(EventData eventData)
    {

    }



}
