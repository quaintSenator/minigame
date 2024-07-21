using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 事件类型
public enum EventType
{
    GameStartEvent,
}

// 事件数据参数基类，具体使用时可以继承该类，添加自己需要的参数
public class EventData
{
}

public class EventManager : MonoBehaviour
{
    private Dictionary<EventType, Action<EventData>> eventDictionary;

    public static EventManager Instance;

    private void Awake()
    {
        Debug.Log("Awake");
        Instance = this;
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
            thisEvent.Invoke(eventData);
        }
        Debug.Log("InvokeEvent");
    }
}
