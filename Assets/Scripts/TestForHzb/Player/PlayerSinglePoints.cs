using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//此代码已被废弃***********************************************************************************
public class PlayerSinglePoints : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private CircleCollider2D myCollider;

    private void Start()
    {
        myCollider = GetComponent<CircleCollider2D>();
    }
    
    //碰撞地面
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            player.SetIsGrounded(true);
        }
    }
    
    //离开地面
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            player.SetIsGrounded(false);
        }
    }
}
