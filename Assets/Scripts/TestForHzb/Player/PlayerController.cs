using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


enum JumpMode
{
    //给与一个力的跳跃
    Force,
    //给与一个速度的跳跃
    Speed,
}

public class PlayerController : MonoBehaviour
{
    //角色自动前进的速度
    [SerializeField] private float speed = 5.0f;
    //角色跳跃的模式
    [SerializeField] private JumpMode jumpMode = JumpMode.Force;
    //角色跳跃的力
    [SerializeField] private float jumpForce = 5.0f;
    //力的作用时间
    [SerializeField] private float jumpTime = 0.5f;
    //角色是否在跳跃
    private bool jumping = false;
    //角色跳跃的初始速度
    [SerializeField] private float jumpSpeed = 5.0f;
    //角色是否在地面上
    private bool isGrounded = true;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Jump();
        }
        
    }

    private void FixedUpdate()
    {
        //角色自动向右前进
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        
        //角色跳跃，如果是Force模式，且在跳跃中，给与一个力
        if (jumpMode == JumpMode.Force && jumping)
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce);
        }
        
        //角色旋转
        if (Input.GetKey(KeyCode.A))
        {
            Rotate(1);
        }
    }
    
    //角色跳跃
    public void Jump()
    {
        if (isGrounded)
        {
            switch (jumpMode)
            {
                case JumpMode.Force:
                    jumping = true;
                    StartCoroutine(JumpForce());
                    break;
                case JumpMode.Speed:
                    GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpSpeed;
                    break;
            }
        }
    }
    
    public void SetIsGrounded(bool value)
    {
        isGrounded = value;
    }
    
    //协程，在jumpTime时间内持续给与一个力
    private IEnumerator JumpForce()
    {
        yield return new WaitForSeconds(jumpTime);
        jumping = false;
    }
    
    //旋转角色
    public void Rotate(float angle)
    {
        transform.Rotate(Vector3.forward, angle);
    }
}
