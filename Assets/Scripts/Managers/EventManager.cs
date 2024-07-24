using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 事件类型
public enum EventType
{
    //输入事件
    OnMouseMove,
    OnMouseLeftClick,
    OnMouseRightClick,
    OnMiddleClick,
    OnHorizontalInput,
    OnSpacebarDown,
    OnEscDown,
    
    GameStartEvent,
    PlayerHitGroundEvent,
    PlayerJumpoffGroundEvent,
    TimerDieEvent
}

// 事件数据参数基类，具体使用时可以继承该类，添加自己需要的参数
public class EventData
{
    private int TimerID;

    public void SetTimerID(int i)
    {
        TimerID = i;
    }

    public int GetTimerID()
    {
        return TimerID;
    }
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
        Debug.Log("AddListener");
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
        Debug.Log("RemoveListener");
    }

    public static void InvokeEvent(EventType eventType, EventData eventData = null)
    {
        Action<EventData> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent?.Invoke(eventData);
        }
        Debug.Log("InvokeEvent");
    }
}
