using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sirenix.Utilities.Editor;
using UnityEngine;
/*TimerManager的使用前提：
 在场景中任意空gameObject上附加TimerManager；这个gameObject必须同时还有TimerTick组件
 
 发起计时：
 TimerManager.Ask4Timer(3.0f, myAsk);
 其中3.0f是秒数；myAsk是一个回调函数，其声明为：
 ```
 void myAsk(EventData eventData){}
 ```
 只经过了初步测试，并不robust
 */
public class TimerManager : Singleton<TimerManager>
{
    private static Timer[] _timers;
    private static readonly int TIMER_LIST_INITIAL_CAPACITY = 10;
    private static int _timerListCapacity = TIMER_LIST_INITIAL_CAPACITY;
    private readonly int SWEEP_FREQUENCY = 3;
    private static Action<EventData>[] _timerCallBacks;

    [SerializeField] private static TimerTick _timerTick;
    
    private static int sweepCount = 0;
    protected override void OnAwake()
    {
        Init();
    }
    private void Init()
    {
        if (_timers == null)
        {
            _timers = new Timer[TIMER_LIST_INITIAL_CAPACITY];
            _timerCallBacks = new Action<EventData>[TIMER_LIST_INITIAL_CAPACITY];
        }
        _timerTick = _timerTick == null ? gameObject.GetComponent<TimerTick>():_timerTick;
        if (_timerTick == null)
        {
            Debug.LogError("TimerManager must have brother component TimerTick.");
        }
        else
        {
            _timerTick.timers = _timers;
        }
        EventManager.AddListener(EventType.TimerDieEvent, OnTimerDie);
    }

    private void OnTimerDie(EventData eventData)
    {
        var timerDieEventData = (TimerDieEventData)eventData;
        var timerID = timerDieEventData.GetTimerID();
        var timerCallBack = _timerCallBacks[timerID];
        timerCallBack?.Invoke(eventData);
        sweepCount++;
        if (sweepCount >= SWEEP_FREQUENCY)
        {
            Sweep();
        }
    }

    //After GetBstFit, a Timer must be fit in at once.
    private static int GetBestFit()
    {
        var firstHole = -1;
        for (var i = 0; i < TimerTick._timerListTail; i++)
        {
            if (_timers[i].isDead())
            {
                firstHole = i;
                break;
            }
        }

        if (firstHole == -1)
        {
            if (TimerTick._timerListTail + 1 >= _timerListCapacity)
            {
                ResetCapacity();
            }
            TimerTick._timerListTail++;
            return TimerTick._timerListTail - 1;
        }
        else
        {
            return firstHole;
        }
    }
    public static int Ask4Timer(float time, Action<EventData> action)
    {
        var bestFitIndex = GetBestFit();
        var generatedTimer = new Timer();
        generatedTimer.Init(bestFitIndex, time);
        _timers[bestFitIndex] = generatedTimer;
        _timerCallBacks[bestFitIndex] = action;
        //_timers[0] 的 id 就是 0
        //即便0计时器死亡，此后0这个id也在也不会分配
        //因此_timers会一直增长
        return bestFitIndex;
    }

    public static int Ask4FrameTimer(int frames, Action<EventData> action)
    {
        var bestFitIndex = GetBestFit();
        FrameTimer generatedTimer = new FrameTimer();
        generatedTimer.Init(bestFitIndex, frames);
        _timers[bestFitIndex] = (FrameTimer)generatedTimer;
        _timerCallBacks[bestFitIndex] = action;
        //_timers[0] 的 id 就是 0
        //即便0计时器死亡，此后0这个id也在也不会分配
        //因此_timers会一直增长
        return bestFitIndex;
    }
    private static void ResetCapacity()
    {
        _timerListCapacity = (int)(_timerListCapacity * 1.5);
        var afterResize = new Timer[_timerListCapacity];
        Array.Copy(_timers, afterResize, _timers.Length);
        _timers = afterResize;

        var afterResizeCallback = new Action<EventData>[_timerListCapacity];
        Array.Copy(_timerCallBacks, afterResizeCallback, _timerCallBacks.Length);
        _timerCallBacks = afterResizeCallback;
    }
    public static void Sweep()
    {
        var lastLivingIndex = -1;
        //每有四个定时器死亡事件触发，将进行一次Sweep，重排数组
        for (int i = 0; i < TimerTick._timerListTail; i++)
        {
            if (!_timers[i].isDead())
            {
                lastLivingIndex = i;
            }
        }

        if (lastLivingIndex != -1)
        {
            TimerTick._timerListTail = lastLivingIndex + 1;
        }
        else
        {
            //all timers are dead
            TimerTick._timerListTail = 0;
        }
#if UNITY_EDITOR
        Debug.Log("After Sweep, timer list = ");
        var sb = new StringBuilder("");
        for (int i = 0; i < TimerTick._timerListTail; i++)
        {
            sb.Append(i);
            sb.Append(" ");
            sb.Append(_timers[i].isDead() ? "Dead" : "Alive");
        }
        Debug.Log(sb.ToString());
#endif
        sweepCount = 0;
    }
}
