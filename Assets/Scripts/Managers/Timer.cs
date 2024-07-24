using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Timer
{
    private float _timeEverStart;

    private float _timeSet;

    private int _timerID;
    private bool _isTimerDead = false;
    
    public void Init(int timerID, float time)
    {
        _timerID = timerID;
        _timeEverStart = 0.0f;
        _timeSet = time;
    }
    public void UpdateTimer()
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
    void Die()
    {
        EventData eventData = new EventData();
        eventData.SetTimerID(_timerID);
        EventManager.InvokeEvent(EventType.TimerDieEvent, eventData);
        this._isTimerDead = true;
    }

    public Boolean isDead()
    {
        return this._isTimerDead;
    }
}
