using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBottomLine : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private BoxCollider2D myCollider;

    private void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
    }
    
    //碰撞地面
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            player.SetIsGrounded(true);
            Debug.Log("cubeTransDrop:"+this.transform.position);
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
}