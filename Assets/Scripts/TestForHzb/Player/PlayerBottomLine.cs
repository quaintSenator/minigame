using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBottomLine : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private BoxCollider2D boxCollider;

    private int enterCount = 0;

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


    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    
    //碰撞地面
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            enterCount++;
            player.OnHitGround();
        }
    }
    
    //离开地面
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            if(enterCount == 1){
                player.OnOffGround();
            }
            enterCount = enterCount - 1 > 0 ? enterCount - 1 : 0;
        }

    }

    private void OnDead(EventData data=null){
        boxCollider.enabled =false;
    }

    private void OnReset(EventData data = null){
        boxCollider.enabled = true;
    }
}