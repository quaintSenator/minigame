using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEndPoint : MonoBehaviour {
    public FlyController flyController;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.TryGetComponent(out PlayerController player))
        {
            flyController.SendVelocity();
            player.SetFlyFinished(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.TryGetComponent(out PlayerController player))
        {
            player.SetFlyFinished(false);
            EventManager.InvokeEvent(EventType.SpacebarUpEvent);
        }
    }
}