using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameSerial
{
    private List<int> framesToCall;
    private Action<EventData> _frameHitCallback;
    public FrameSerial(string buildSerialString, Action<EventData> FrameHitCallback)
    {
        framesToCall = new List<int>();
        buildframeListByString(buildSerialString);
        _frameHitCallback = FrameHitCallback;
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
    public void callBySerial()
    {
        foreach (var bit in framesToCall)
        {
            TimerManager.Ask4FrameTimer(bit, _frameHitCallback);
        }
    }
}
