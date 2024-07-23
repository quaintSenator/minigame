using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;


enum JumpMode
{
    //给与一个力的跳跃
    Force,
    //给与一个速度的跳跃
    Speed,
}

public class PlayerController : MonoBehaviour
{
    //角色的重力系数
    [SerializeField] private float gravityScale = 4.0f;
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
    //角色撞击地面事件

    //一些初始化
    private Transform cubeSprites;

    private void Awake(){
        cubeSprites = transform.Find("Visual");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Jump();
        }
    }
    
    private void FixedUpdate()
    {
        //角色一直受一个向下的重力，世界坐标系
        GetComponent<Rigidbody2D>().AddForce(Vector2.down * gravityScale * GameConsts.GRAVITY);

        //角色自动向右前进，世界坐标系
        transform.Translate(Vector3.right * speed * Time.fixedDeltaTime, Space.World);

        //角色跳跃，如果是Force模式，且在跳跃中，给与一个力
        if (jumpMode == JumpMode.Force && jumping)
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce);
        }

        //角色旋转
        if (!isGrounded)
        {
            Rotate();
        }
        Debug.Log("isGrounded:"+isGrounded);
    }

    //角色跳跃
    public void Jump()
    {
        if (isGrounded)
        {
            switch (jumpMode)
            {
                case JumpMode.Force:
                    EventManager.InvokeEvent(EventType.PlayerJumpoffGroundEvent);
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
        if (!isGrounded && value)
        {
            Rotate(true);
            EventManager.InvokeEvent(EventType.PlayerHitGroundEvent);
        }
        isGrounded = value;
    }

    //协程，在jumpTime时间内持续给与一个力
    private IEnumerator JumpForce()
    {
        yield return new WaitForSeconds(jumpTime);
        jumping = false;
    }

    //旋转角色
    public void Rotate(bool end = false)
    {
        float selfAngle = cubeSprites.eulerAngles.z;
        Quaternion spriteRotate = cubeSprites.rotation;
        if (end)
        {
            //如果此时角色旋转接近-180度，就旋转到-180度，否则旋转到0度
            /*if (Mathf.Abs(transform.rotation.eulerAngles.z) > 5f && Mathf.Abs(transform.rotation.eulerAngles.z) < 185f)
            {
                transform.rotation = Quaternion.Euler(0, 0, -180);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            return;*/
            if (Mathf.Abs(selfAngle - 0) < 45f)
            {
                spriteRotate = Quaternion.Euler(0,0,0);
            }
            else if (Mathf.Abs(selfAngle - (-90)) < 45f){
                spriteRotate = Quaternion.Euler(0,0,-90);
            }
            else if (180 - Mathf.Abs(selfAngle) <= 45.0f){
                spriteRotate = Quaternion.Euler(0,0,-180);
            }
            else {
                spriteRotate = Quaternion.Euler(0,0,90);
            }
            cubeSprites.rotation = spriteRotate;
            return;
        }

        float time = 1.0f;
        if (jumpMode == JumpMode.Speed)
        {
            time = jumpSpeed / (GameConsts.GRAVITY * gravityScale) * 2;
        }
        else if (jumpMode == JumpMode.Force)
        {
            time = speed / (GetComponent<Rigidbody2D>().gravityScale / GameConsts.GRAVITY) * 2;
        }

        //Debug.Log("Time: " + time);
        float angle = 180 / time * Time.fixedDeltaTime;
        cubeSprites.Rotate(-Vector3.forward, angle);
    }
}
