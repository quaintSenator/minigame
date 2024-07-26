using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerEntireCollider : MonoBehaviour{
    private BoxCollider2D boxCollider;

    public PlayerController player;

    private void Awake(){
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.PlayerDeadEvent,OnDead);
        EventManager.AddListener(EventType.GameRestartEvent,OnReset);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.PlayerDeadEvent,OnDead);
        EventManager.RemoveListener(EventType.GameRestartEvent,OnReset);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Safe")){

        }
        else if(other.gameObject.CompareTag("Unsafe")){
            player.SetIsDead(true);
        }
        Debug.Log("isCollidsion"+other.gameObject.tag);
    }
    private void OnTriggerExit2D(Collider2D other) {
        
    }

    
    private void OnDead(EventData data=null){
        boxCollider.enabled =false;
    }

    private void OnReset(EventData data = null){
        boxCollider.enabled = true;
    }

}