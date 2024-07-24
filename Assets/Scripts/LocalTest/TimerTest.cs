using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTest : MonoBehaviour
{
    void myAsk(EventData eventData)
    {
        Debug.Log("My Ask answered after 240 frames.");
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Timer start...");
            TimerManager.Ask4FrameTimer(240, myAsk);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TimerManager.Sweep();
        }
    }
}
