using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;
public class CleverTimeDieEventData : EventData
{
    private GameObject go_shall_die;
    public CleverTimeDieEventData(GameObject go)
    {
        go_shall_die = go;
    }
}
public class TimerDieEventData : EventData
{
    public double absoluteTime;
    public TimerDieEventData(double i)
    {
        absoluteTime = i;
    }
}

public class FrameTimerDieEventData : EventData
{
    public int absoluteFrame;
    public FrameTimerDieEventData(int i)
    {
        absoluteFrame = i;
    }
}
public class TimerPElement : PElement
{
    private readonly Action<EventData> _timeupCallback;
    private EventData _timerSetEventData;
    private readonly double _absoluteTime = 0.0f;
    private readonly int _absoluteFrame = 0;
    
    public TimerPElement(Action<EventData> callback, int absoluteFrame, EventData data2Pass = null)
    {
        _timeupCallback = callback;
        _timerSetEventData = data2Pass;
        _absoluteFrame = absoluteFrame;
    }
    public TimerPElement(Action<EventData> callback, double absoluteTime, EventData data2Pass = null)
    {
        _timeupCallback = callback;
        _timerSetEventData = data2Pass;
        _absoluteTime = absoluteTime;
    }
    public void InvokeSelf()
    {
        if (_timeupCallback == null)
        {
            Debug.LogError("One TimerPElement has no callback. Invoke timerEvent failed.");
            return;
        }

        if (_timerSetEventData == null)
        {
            EventData complementedTimerSetEventData;
            if (_absoluteFrame != 0)
            {
                complementedTimerSetEventData = new FrameTimerDieEventData(_absoluteFrame);
            }
            else if (_absoluteTime > 0.0f)
            {
                complementedTimerSetEventData = new TimerDieEventData(_absoluteTime);
            }
            else
            {
                complementedTimerSetEventData = null;
            }
            _timerSetEventData = complementedTimerSetEventData;
        }
        _timeupCallback.Invoke(_timerSetEventData);
    }
}
public class CleverTimerManager : Singleton<CleverTimerManager>
{
    private static PriorityQueue _pq4FloatTimer;
    private static PriorityQueue _pq4FrameTimer;
    [SerializeField] private int frame2Wait_test;
    protected override void OnAwake()
    {
        Init();
    }
    public static double Ask4Timer(double time, Action<EventData> action, EventData data2Pass = null)
    {
        var timeToExplode = Time.timeAsDouble + time;
        var pElement = new TimerPElement(action,timeToExplode, data2Pass);
        var prioElement = new PriorityElement(timeToExplode, pElement);
        _pq4FloatTimer.Enqueue(prioElement);
        return timeToExplode;
    }
    
    public static int Ask4FrameTimer(int frame, Action<EventData> action, EventData data2Pass = null)
    {
        var frame2Explode = Time.frameCount + frame;
        var pElement = new TimerPElement(action, frame2Explode, data2Pass);
        var prioElement = new PriorityElement(frame2Explode, pElement);
        _pq4FrameTimer.Enqueue(prioElement);
        return frame2Explode;
    }
    private void Init()
    {
        _pq4FloatTimer = new PriorityQueue();
        _pq4FrameTimer = new PriorityQueue();
    }
    
    private void Update()
    {
        if (!_pq4FloatTimer.IsEmpty())
        {
            var floatTimerTop = _pq4FloatTimer.Peek();
            var floatTimeCurrent = Time.timeAsDouble;
            while (floatTimeCurrent >= floatTimerTop.priority)
            {
                var timeUpElement = (TimerPElement)floatTimerTop.stuff;
                timeUpElement.InvokeSelf();
                _pq4FloatTimer.Dequeue();
                //Step2Next
                floatTimerTop = _pq4FloatTimer.Peek();
                if (floatTimerTop == null) break;
            }
        }
        if (!_pq4FrameTimer.IsEmpty())
        {
            var frameTop = _pq4FrameTimer.Peek();
            var frameCurrent = Time.frameCount;
            while (frameCurrent >= frameTop.priority)
            {
                var frameCatchElememt = (TimerPElement)frameTop.stuff;
                frameCatchElememt.InvokeSelf();
                _pq4FrameTimer.Dequeue();
                //Step2Next
                frameTop = _pq4FrameTimer.Peek();
                if (frameTop == null) break;
            }
        }
    }
}
