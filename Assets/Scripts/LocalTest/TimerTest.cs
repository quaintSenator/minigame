using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTest : MonoBehaviour
{
    void myAsk(EventData eventData)
    {
        Debug.Log("My Ask answered after 3 secs.");
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Timer start...");
            TimerManager.Ask4Timer(3.0f, myAsk);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TimerManager.Sweep();
        }
    }
}
