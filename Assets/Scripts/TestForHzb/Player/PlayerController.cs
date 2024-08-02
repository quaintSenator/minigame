using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


enum JumpMode
{
    //给与一个力的跳跃
    Force,
    //给与一个速度的跳跃
    Speed,
}

[System.Serializable]
public class JumpSettings : System.Object
{
    public float gravityScale;
    public float horizontalBlockNum;
    public float verticalBlockNum;

    public JumpSettings()
    {
        gravityScale = 12.0f;
        horizontalBlockNum = 4.5f;
        verticalBlockNum = 3.0f;
    }
    public JumpSettings(float gravityScale, float horizontalBlockNum, float verticalBlockNum)
    {
        this.gravityScale = gravityScale;
        this.horizontalBlockNum = horizontalBlockNum;
        this.verticalBlockNum = verticalBlockNum;
    }
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] public JumpSettings jumpSettings;
    //角色自动前进的速度
    private float speed = 5.0f;
    //角色跳跃的模式
    [SerializeField] private JumpMode jumpMode = JumpMode.Force;
    //角色跳跃的力
    [SerializeField] private float jumpForce = 5.0f;
    //角色按键缓冲时长
    [SerializeField] private double bufferTime = 0.1f;
    //力的作用时间
    private float jumpTime = 0.5f;
    //角色是否在跳跃
    private bool jumping = false;
    //角色跳跃的初始速度
    private float jumpSpeed = 5.0f;
    //角色是否在地面上
    private bool isGrounded = true;
    //角色是否已经死亡
    private bool isDead = false;
    //角色是否再跳
    private bool willJump = false;
    //缓冲计时器是否生效
    private bool isBufferActive = false;
    //缓冲计时器计数
    private int bufferTimerCount = 0;
    //废弃计时器个数（起跳后废弃）
    private int disableTimerCount = 0;
    //角色是否可以连跳
    private bool isContinueJump = false;
    //角色是否进行回正
    private bool isReturn = false;
    //回正时间
    private readonly float RETURN_TIME = 0.1f;
    //回正计时器
    private float returnTimer;
    //回正角度
    private float selfAngle;

    //一些初始化
    private Transform cubeSprites;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;

    //为减少FixUp开销保存ForceManager引用
    private ForceManager _forceManager;
    //为减少FixUp开销保存HeadingDir的常态，仅在事件下切换
    private Vector3 playerHeadingDir;
    private void Awake()
    {
        cubeSprites = transform.Find("Visual");
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        if (jumpMode == JumpMode.Speed)
        {
            CalSettings();
        }
        _forceManager = ForceManager.Instance;
        playerHeadingDir = Vector3.right;
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.MouseRightClickEvent, OnDead);
        EventManager.AddListener(EventType.SpacebarDownEvent, OnSpacebarDown);
        EventManager.AddListener(EventType.SpacebarUpEvent, OnSpacebarUp);
        EventManager.AddListener(EventType.MouseLeftClickEvent, OnMouseLeftClick);

    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.MouseRightClickEvent, OnDead);

        EventManager.RemoveListener(EventType.SpacebarDownEvent, OnSpacebarDown);
        EventManager.RemoveListener(EventType.SpacebarUpEvent, OnSpacebarUp);
        EventManager.RemoveListener(EventType.MouseLeftClickEvent, OnMouseLeftClick);

    }
    private void Start()
    {
        returnTimer = 0;
        registerEvents();
    }

    private void Update()
    {
        /* if (Input.GetKey(KeyCode.Space) && rigidBody.velocity.y <= 0 && !isGrounded)
         {
             //Debug.Log("willJump");
             willJump = true;
         }*/
        if (isReturn)
        {
            returnTimer += Time.deltaTime;
        }
        if (returnTimer >= RETURN_TIME)
        {
            returnTimer = 0;
            isReturn = false;
            SetCorrect();
        }
        Rotate();
    }

    private void FixedUpdate()
    {
        //角色一直受一个向下的重力，世界坐标系
        rigidBody.AddForce(ForceManager.Instance.GetGravityDir() * jumpSettings.gravityScale * GameConsts.GRAVITY);

        //角色自动向右前进，世界坐标系
        transform.Translate(playerHeadingDir * speed * Time.fixedDeltaTime, Space.World);

        //角色跳跃，如果是Force模式，且在跳跃中，给与一个力
        if (jumpMode == JumpMode.Force && jumping)
        {
            rigidBody.AddForce(Vector2.up * jumpForce);
        }
    }

    private void OnSpacebarDown(EventData data = null)
    {
        if (isGrounded)
        {
            TryJump();
        }
        else
        {
            willJump = true;
        }
        isContinueJump = true;
    }

    private void OnSpacebarUp(EventData DATA = null)
    {
        CleverTimerManager.Ask4Timer(bufferTime, OnBufferTimeEnd);
        bufferTimerCount++;
        isBufferActive = true;
        isContinueJump = false;
    }

    private void OnMouseLeftClick(EventData data = null)
    {
        //Jump();
    }

    private void OnBufferTimeEnd(EventData data)
    {
        if (disableTimerCount > 0)
        {
            disableTimerCount--;
            return;
        }

        bufferTimerCount--;
        if (bufferTimerCount == 1)
        {
            isBufferActive = true;
        }
        else if (isBufferActive)
        {
            willJump = false;
            isBufferActive = false;
        }
    }

    //注册事件统一函数
    private void registerEvents()
    {

    }

    //角色跳跃
    private void Jump()
    {
        disableTimerCount = bufferTimerCount > 0 ? bufferTimerCount : 0;
        bufferTimerCount = 0;
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

    //对外跳跃接口，设置跳跃参数，不传为默认参数
    public void TryJump(JumpSettings data = null, bool mustJump = false)
    {
        if (isGrounded)
        {
            CalSettings(data);
            Jump();
        }
        else if (mustJump)
        {
            CalSettings(data);
            Jump();
        }
    }

    private void CalSettings(JumpSettings data = null)
    {
        if (data == null)
        {
            jumpSettings = GameConsts.DEFAULT_JUMP;
        }
        else
        {
            jumpSettings = data;
        }

        jumpSpeed = Mathf.Sqrt(2 * GameConsts.GRAVITY * jumpSettings.gravityScale * jumpSettings.verticalBlockNum * transform.localScale.x);
        jumpTime = jumpSpeed / (GameConsts.GRAVITY * jumpSettings.gravityScale) * 2;
        speed = jumpSettings.horizontalBlockNum / jumpTime * transform.localScale.x;
    }

    public void SetIsGrounded(bool value)
    {
        if (!isGrounded && value)
        {
            isGrounded = value;
            OnHitGround();
        }
        isGrounded = value;
    }

    public void SetIsDead(bool value)
    {
        if (!isDead && value)
        {
            isDead = value;
            OnDead();
        }
        else
        {
            isDead = value;
        }
    }

    public void OnHitGround(EventData data = null)
    {
        isGrounded = true;
        isReturn = true;
        selfAngle = cubeSprites.eulerAngles.z;
        HitGroundEventData hitdata = new HitGroundEventData(playerHeadingDir);
        EventManager.InvokeEvent(EventType.PlayerHitGroundEvent, hitdata);
        if (willJump || isContinueJump)
        {
            if (!isContinueJump)
                willJump = false;
            TryJump();
        }
        Debug.Log("OnHitGround");
    }

    public void OnOffGround(EventData data = null)
    {
        isGrounded = false;
        isReturn = false;
        returnTimer = 0;
        EventManager.InvokeEvent(EventType.PlayerJumpoffGroundEvent);
    }

    //协程，在jumpTime时间内持续给与一个力
    private IEnumerator JumpForce()
    {
        yield return new WaitForSeconds(jumpTime);
        jumping = false;
    }

    //旋转角色
    public void Rotate()
    {
        if (isReturn)
        {
            if (Mathf.Abs(selfAngle - 270) <= 45.0f)
            {
                cubeSprites.Rotate(-Vector3.forward, (selfAngle - 270) / RETURN_TIME * Time.deltaTime);
            }
            else if (Mathf.Abs(selfAngle - 180) <= 45.0f)
            {
                cubeSprites.Rotate(-Vector3.forward, (selfAngle - 180) / RETURN_TIME * Time.deltaTime);
            }
            else if (Mathf.Abs(selfAngle - 90) <= 45.0f)
            {
                cubeSprites.Rotate(-Vector3.forward, (selfAngle - 90) / RETURN_TIME * Time.deltaTime);
            }
            else
            {
                if (selfAngle < 180)
                    cubeSprites.Rotate(-Vector3.forward, (selfAngle - 0) / RETURN_TIME * Time.deltaTime);
                else
                    cubeSprites.Rotate(-Vector3.forward, (selfAngle - 360) / RETURN_TIME * Time.deltaTime);
            }
            return;
        }

        if (!isGrounded)
        {
            float time = 1.0f;
            if (jumpMode == JumpMode.Speed)
            {
                time = jumpSpeed / (GameConsts.GRAVITY * jumpSettings.gravityScale) * 2;
            }
            else if (jumpMode == JumpMode.Force)
            {
                time = speed / (rigidBody.gravityScale / GameConsts.GRAVITY) * 2;
            }

            //Debug.Log("Time: " + time);
            float angle = 180 / time * Time.deltaTime;
            cubeSprites.Rotate(-Vector3.forward, angle);
        }
    }

    private void SetCorrect()
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
    }

    public void OnDead(EventData data = null)
    {
        boxCollider.enabled = false;
        EventManager.InvokeEvent(EventType.PlayerDeadEvent);
        OnReset();
    }

    public void OnReset(EventData data = null)
    {
        isGrounded = true;
        transform.position = GameConsts.START_POSITION;
        cubeSprites.rotation = GameConsts.ZERO_ROTATION;
        rigidBody.velocity = GameConsts.START_VELOCITY;

        isDead = false;
        isReturn = false;
        returnTimer = 0;
        boxCollider.enabled = true;
        willJump = false;
        isContinueJump = false;
        disableTimerCount = bufferTimerCount > 0 ? bufferTimerCount : 0;
        bufferTimerCount = 0;
        jumpSettings = GameConsts.DEFAULT_JUMP;
        EventManager.InvokeEvent(EventType.GameRestartEvent);
    }

    public Vector3 getPlayerVelocity()
    {
        return rigidBody.velocity;
    }

    public float GetSpeed()
    {
        return speed;
    }
}
