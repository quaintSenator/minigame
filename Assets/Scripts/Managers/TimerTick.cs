using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTick : MonoBehaviour
{
    public Timer[] timers;
    public static int _timerListTail = 0;//tail is the first null position
    void Update()
    {
        for(var i = 0; i < _timerListTail; i++)
        {
            timers[i].UpdateTimer();
        }
    }
}
