using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBottomLine : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private BoxCollider2D boxCollider;

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
            player.SetIsGrounded(true);
            Debug.Log("cubeTransDrop:"+transform.position);
        }
    }
    
    //离开地面
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            player.SetIsGrounded(false);
            Debug.Log("cubeTransUp:"+transform.position);
        }
    }

    private void OnDead(EventData data=null){
        boxCollider.enabled =false;
    }

    private void OnReset(EventData data = null){
        boxCollider.enabled = true;
    }
}