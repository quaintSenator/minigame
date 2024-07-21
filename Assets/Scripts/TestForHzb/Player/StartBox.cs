using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EventManager.InvokeEvent(EventType.GameStartEvent);
        }
    }
}
