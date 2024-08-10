using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 事件类型
public enum EventType
{
    GameStartEvent = 1,
    GameRestartEvent = 2,
    PlayerHitGroundEvent = 3,
    PlayerJumpoffGroundEvent = 4,
    TimerDieEvent = 5,
    PlayerDeadEvent = 6,
    GravityInverseEvent = 7,
    MusicStartEvent = 8,
    MusicRecordEvent = 9,
    SwitchLevelEvent = 10,
    DecideCanJumpEvent = 11,


    #region 输入事件在此添加
    
    MouseMoveEvent = 101,
    MouseLeftClickEvent = 102,
    MouseRightClickEvent = 103,
    MiddleClickEvent = 104,
    MiddleScrollEvent = 105,
    SpacebarDownEvent = 106,
    SpacebarUpEvent = 107,
    JDownEvent = 108,
    
    #endregion

    #region 地图编辑器输入事件

    CancelCurrentSelectEvent = 201,
    DrawOrEraseEvent = 202,
    ChangeTileModeEvent = 203,
    SaveMapEvent = 204,
    LoadMapOneEvent = 205,
    LoadMapTwoEvent = 206,
    LoadMapThreeEvent = 207,
    PauseOrResumeMusicEvent = 208,
    StopOrPlayMusicEvent = 209,
    OpenOrCloseCoordinateEvent = 210,
    SelectCurrentMusicTimeEvent = 211,

    #endregion
}

// 事件数据参数基类，具体使用时可以继承该类，添加自己需要的参数
public class EventData
{
    
}

public class EventManager : Singleton<EventManager>
{
    private Dictionary<EventType, Action<EventData>> eventDictionary;

    protected override void OnAwake()
    {
        Init();
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<EventType, Action<EventData>>();
        }
    }
    public static void AddListener(EventType eventType, Action<EventData> listener)
    {
        Action<EventData> myEvent;
        if (Instance.eventDictionary.TryGetValue(eventType, out myEvent))
        {
            myEvent += listener;
            Instance.eventDictionary[eventType] = myEvent;
        }
        else
        {
            myEvent += listener;
            Instance.eventDictionary.Add(eventType, myEvent);
        }
    }

    public static void RemoveListener(EventType eventType, Action<EventData> listener)
    {
        if (Instance == null) return;
        Action<EventData> thisEvent;
        if (Instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent -= listener;
            Instance.eventDictionary[eventType] = thisEvent;
        }
    }

    public static void InvokeEvent(EventType eventType, EventData eventData = null)
    {
        Action<EventData> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent?.Invoke(eventData);
        }
    }
}
