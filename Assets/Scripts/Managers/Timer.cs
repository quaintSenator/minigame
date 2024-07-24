using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class TimerDieEventData : EventData
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
public class Timer
{
    protected float _timeEverStart;
    protected float _timeSet;
    protected int _timerID;
    protected bool _isTimerDead = false;
    
    public void Init(int timerID, float time)
    {
        _timerID = timerID;
        _timeEverStart = 0.0f;
        _timeSet = time;
    }
    public virtual void UpdateTimer()
    {
        if (!_isTimerDead)
        {
            _timeEverStart += Time.deltaTime;
            if (_timeEverStart > _timeSet)
            {
                Die();
            }
        }
    }
    protected void Die()
    {
        TimerDieEventData eventData = new TimerDieEventData();
        eventData.SetTimerID(_timerID);
        EventManager.InvokeEvent(EventType.TimerDieEvent, eventData);
        this._isTimerDead = true;
    }

    public Boolean isDead()
    {
        return this._isTimerDead;
    }
}
public class FrameTimer : Timer
{
    private float _frameEverStart;

    private float _frameSet;
     public override void UpdateTimer()
    {
        if (!_isTimerDead)
        {
            _timeEverStart ++;
            if (_timeEverStart > _timeSet)
            {
                Die();
            }
        }
    }
}