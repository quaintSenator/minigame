using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameSerial
{
    private List<int> framesToCall;
    private Action<EventData> _frameHitCallback;
    private List<EventData> _eventArgs;

    public FrameSerial(string buildSerialString, Action<EventData> frameHitCallback)
    {
        framesToCall = new List<int>();
        buildframeListByString(buildSerialString);
        _frameHitCallback = frameHitCallback;
        _eventArgs = new List<EventData>();
    }
    public FrameSerial(string buildSerialString, Action<EventData> frameHitCallback, List<EventData> callbackParams)
    :this(buildSerialString,frameHitCallback)
    {
        _eventArgs = callbackParams;
    }
    public FrameSerial(string buildSerialString, Action<EventData> frameHitCallback, EventData callbackParam)
        :this(buildSerialString, frameHitCallback)
    {
        _eventArgs.Add(callbackParam);
    }

    public void setSerialEventDataList(List<EventData> list)
    {
        _eventArgs = new List<EventData>();
        foreach (var eventData in list)
        {
            _eventArgs.Add(eventData);
        }
    }
    private void buildframeListByString(string buildSerialString)
    {
        var words = buildSerialString.Split(new char[] { ' ' });
        foreach (var word in words)
        {
            var parseNum = Int32.Parse(word);
            if (parseNum >= 1)
            {
                framesToCall.Add(parseNum);
            }
        }
    }
    public void CallBySerial()
    {
        if (_eventArgs == null)
        {
            Debug.LogError("FrameSerial._eventArgs = null");
            return;
        }
        for (var i = 0; i < framesToCall.Count; i++)
        {
            var frame = framesToCall[i];
            var id = CleverTimerManager.Instance.Ask4FrameTimer(frame, _frameHitCallback, null);
        }
    }
}
