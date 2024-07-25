using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Unity.Mathematics;
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
    private float speed = 5.0f;
    //角色跳跃的模式
    [SerializeField] private JumpMode jumpMode = JumpMode.Force;
    //角色跳跃的力
    [SerializeField] private float jumpForce = 5.0f;
    //力的作用时间
    [SerializeField] private float jumpTime = 0.5f;
    //角色是否在跳跃
    private bool jumping = false;
    //角色跳跃的初始速度
    private float jumpSpeed = 5.0f;
    //角色是否在地面上
    private bool isGrounded = true;
    //角色是否再跳
    private bool willJump = false;

    //一次跳跃水平移动距离
    [SerializeField] private float horizontalBlockNum = 4.5f;
    //一次跳跃垂直移动距离
    [SerializeField] private float verticalBlockNum = 2.5f;

    //一些初始化
    private Transform cubeSprites;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;

    private void Awake()
    {
        cubeSprites = transform.Find("Visual");
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        if (jumpMode == JumpMode.Speed){
            jumpSpeed = Mathf.Sqrt(2 * GameConsts.GRAVITY * gravityScale * verticalBlockNum * transform.localScale.x);
            jumpTime = jumpSpeed / (GameConsts.GRAVITY * gravityScale) * 2;
            speed = horizontalBlockNum / jumpTime * transform.localScale.x ;
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.GameRestartEvent, OnReset);
        EventManager.AddListener(EventType.MouseRightClickEvent, OnDead);
        EventManager.AddListener(EventType.PlayerHitGroundEvent,OnHitGround);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.GameRestartEvent, OnReset);
        EventManager.RemoveListener(EventType.MouseRightClickEvent, OnDead);
        EventManager.RemoveListener(EventType.PlayerHitGroundEvent,OnHitGround);
    }
    private void Start()
    {
        registerEvents();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Jump();
        }
        if(Input.GetKey(KeyCode.Space) && rigidBody.velocity.y <= 0)
        {
            Debug.Log("willJump");
            willJump = true;
        }
    }

    private void FixedUpdate()
    {
        //角色一直受一个向下的重力，世界坐标系
        rigidBody.AddForce(Vector2.down * gravityScale * GameConsts.GRAVITY);

        //角色自动向右前进，世界坐标系
        transform.Translate(Vector3.right * speed * Time.fixedDeltaTime, Space.World);

        //角色跳跃，如果是Force模式，且在跳跃中，给与一个力
        if (jumpMode == JumpMode.Force && jumping)
        {
            rigidBody.AddForce(Vector2.up * jumpForce);
        }

        //角色旋转
        if (!isGrounded)
        {
            Rotate();
        }
    }

    //注册事件统一函数
    private void registerEvents()
    {

    }

    //角色跳跃
    public void Jump()
    {
        if (isGrounded)
        {
            EventManager.InvokeEvent(EventType.PlayerJumpoffGroundEvent);
            switch (jumpMode)
            {
                case JumpMode.Force:
                    jumping = true;
                    StartCoroutine(JumpForce());
                    break;
                case JumpMode.Speed:
                    rigidBody.velocity = Vector2.up * jumpSpeed;
                    break;
            }
        }
    }

    public void SetIsGrounded(bool value)
    {
        if (!isGrounded && value)
        {
            Rotate(true);
            isGrounded = value;
            EventManager.InvokeEvent(EventType.PlayerHitGroundEvent);
        }
        isGrounded = value;
    }

    public void OnHitGround(EventData data){
        if(willJump){
            willJump = false;
            Jump();
        }
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
        if (end)
        {
            Quaternion spriteRotate = cubeSprites.rotation;
            if (Mathf.Abs(selfAngle - 270) <= 45.0f)
            {
                spriteRotate = Quaternion.Euler(0, 0, 270);
            }
            else if (Mathf.Abs(selfAngle - 180) <= 45.0f)
            {
                spriteRotate = Quaternion.Euler(0, 0, 180);
            }
            else if (Mathf.Abs(selfAngle - 90) <= 45.0f)
            {
                spriteRotate = Quaternion.Euler(0, 0, 90);
            }
            else
            {
                spriteRotate = Quaternion.Euler(0, 0, 0);
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
            time = speed / (rigidBody.gravityScale / GameConsts.GRAVITY) * 2;
        }

        //Debug.Log("Time: " + time);
        float angle = 180 / time * Time.fixedDeltaTime;
        cubeSprites.Rotate(-Vector3.forward, angle);
    }

    public void OnDead(EventData data)
    {
        boxCollider.enabled = false;
        EventManager.InvokeEvent(EventType.GameRestartEvent);
        
    }

    public void OnReset(EventData data)
    {
        transform.position = GameConsts.START_POSITION;
        cubeSprites.rotation = GameConsts.ZERO_ROTATION;
        rigidBody.velocity = GameConsts.START_VELOCITY;
        isGrounded = true;
        boxCollider.enabled = true;
    }

    
}
