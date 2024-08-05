
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;



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
    public float jumpTime;
    public float horizontalBlockNum;
    public float verticalBlockNum;

    public JumpSettings()
    {
        jumpTime = 0.25f;
        horizontalBlockNum = 4.5f;
        verticalBlockNum = 3.0f;
    }
    public JumpSettings(float jumpTime, float horizontalBlockNum, float verticalBlockNum)
    {
        this.jumpTime = jumpTime;
        this.horizontalBlockNum = horizontalBlockNum;
        this.verticalBlockNum = verticalBlockNum;
    }
}

public class PlayerController : MonoBehaviour
{
    private double JUDGE_ZERO = 0.000001f;

    [SerializeField]
    [Tooltip("移动、跳跃的基础单位（格子的具体大小），根据场景设置，后续可能会根据变速产生改变")]
    private float worldScale = 1.0f;

    [SerializeField]
    [Tooltip("一次节拍时间，默认BPM为120，即一拍0.5s")]
    private float beatTime = 0.5f;

    [SerializeField]
    [Tooltip("角色按键缓冲时长，玩家过早输入后，在一定时间内仍然可以触发动作")]
    private double bufferTime = 0.1f;

    [SerializeField] public JumpSettings jumpSettings;

    [SerializeField] 
    [Tooltip("角色自动前进的速度,默认为8格/s,即BPM120，每拍前进4格")] 
    private float speed = 8.0f;

    [SerializeField]
    [Tooltip("跳跃实现方式，默认为给予初速实现")]
    private JumpMode jumpMode = JumpMode.Force;

    [SerializeField]
    [ReadOnly]
    [Tooltip("重力的大小，目前根据另外两个参数自动调整")]
    private float gravityScale = 50.0f;

    [SerializeField]
    [ReadOnly]
    [Tooltip("跳跃实现方式->给予初速的速度大小，目前根据另外两个参数自动调整")]
    private float jumpSpeed = 5.0f;

    [SerializeField]
    [Tooltip("一次跳跃最高上升的高度，需要调整的参数之一，影响重力和跳跃初速")]
    private float jumpHeight = 2.6f;

    [SerializeField]
    [Tooltip("一次节拍循环完成时（从起跳到下一次起跳）的此时和最高高度的高度差，需要调整的参数之一，影响重力和跳跃初速，同时为了保证关卡的搭建，需要比1大一点")]
    private float jumpDeltaHeight = 1.1f;

    [SerializeField]
    [Tooltip("跳跃实现方式->给予一段持续时间力中力的大小")]
    private float jumpForce = 25.0f;



    //角色是否在跳跃
    private bool jumping = false;

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
    //跳跃计时器
    private float jumpTimer;

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
    //死亡判定余量
    private float deadZone = 0.5f;
    //运动时长
    private float moveTimer = 0;

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
        /*        if (jumpMode == JumpMode.Speed)
                {
                    CalSettings();
                }*/


        _forceManager = ForceManager.Instance;
        playerHeadingDir = Vector3.right;
        CalSettings();
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

        if(isBufferActive)
        {
            jumpTimer += Time.deltaTime;
        }
        if(jumpTimer >= bufferTime)
        {
            jumpTimer = 0;
            isBufferActive = false;
            willJump = false;
        }

        Rotate();
        CheckDead();
    }

    private void FixedUpdate()
    {
        //角色一直受一个向下的重力，世界坐标系
        rigidBody.AddForce(ForceManager.Instance.GetGravityDir() * gravityScale);//* GameConsts.GRAVITY);

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
        Debug.Log("OnSpaceDown");
    }

    private void OnSpacebarUp(EventData DATA = null)
    {
        Debug.Log("OnSpacebarUp");
        //CleverTimerManager.Ask4Timer(bufferTime, OnBufferTimeEnd);
        //bufferTimerCount++;

        jumpTimer = 0;
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
        isBufferActive = false;
        jumpTimer = 0;
        switch (jumpMode)
        {
            case JumpMode.Force:
                jumping = true;
                StartCoroutine(JumpForce());
                break;
            case JumpMode.Speed:
                rigidBody.velocity = Vector2.up * jumpSpeed;

                //rigidBody.AddForce(Vector2.up * jumpForce);
                // Impulse
                //rigidBody.impluse
                break;
        }
    }

    //对外跳跃接口，设置跳跃参数，不传为默认参数
    public void TryJump(JumpSettings data = null, bool mustJump = false)
    {
        if (isGrounded)
        {
           // CalSettings(data);
            Jump();
        }
        else if (mustJump)
        {
           // CalSettings(data);
            Jump();
        }
    }

    private void CalSettings(JumpSettings data = null)
    {
        /*        if (data == null)
                {
                    jumpSettings = GameConsts.DEFAULT_JUMP;
                }
                else
                {
                    jumpSettings = data;
                }

                gravityScale = 2 * jumpSettings.verticalBlockNum * transform.localScale.x / Mathf.Pow(jumpSettings.jumpTime,2.0f) / GameConsts.GRAVITY;
                jumpSpeed = Mathf.Sqrt(2 * GameConsts.GRAVITY * gravityScale * jumpSettings.verticalBlockNum * transform.localScale.x);
                speed = (jumpSettings.horizontalBlockNum * transform.localScale.x) / (jumpSettings.jumpTime * 2) ;*/


        if (data == null)
        {
            jumpSettings = GameConsts.DEFAULT_JUMP;
        }

        CalNormalJumpParameter();

        //TODO:这个值应该是从别处拿到或者直接赋的
        worldScale = transform.localScale.x;

        speed = speed  * worldScale;

        gravityScale = gravityScale * worldScale;

        jumpForce = jumpForce * worldScale;

        jumpSpeed = jumpSpeed * worldScale;








    }

    private void CalNormalJumpParameter()
    {

        //跳跃一次的上升初速度和最高点位置解析出来会满足一个数量关系
        //jumpHeight = jumpHeight * worldScale;
        if (jumpHeight  < JUDGE_ZERO && jumpDeltaHeight < JUDGE_ZERO)
        {
            Debug.LogError("Error jumpHeight！");
            return;
        }

        //一次跳跃从起跳到最高点的时间
        float jumptime = 0;

        jumptime = (float)(Math.Sqrt(jumpHeight / jumpDeltaHeight) 
            * jumpDeltaHeight
            / (jumpDeltaHeight + Math.Sqrt(jumpDeltaHeight + jumpHeight))
            * beatTime );

        if (jumptime < JUDGE_ZERO )
        {
            Debug.LogError("Error jumptime！");
            return;
        }

        gravityScale = 2 * jumpHeight / jumptime / jumptime;

        jumpSpeed = jumptime * gravityScale;


        /*        jumpSpeed = Math.Sqrt(jumpHeight / jumpHeight - 1) 
                    * (jumpHeight - 1)
                    / ((jumpHeight - 1) + Math.Sqrt(jumpHeight* jumpHeight - jumpHeight) )
                    * gravityScale
                    * beatTime;*/
    }

    private void CheckDead()
    {
        moveTimer+=Time.deltaTime;
        if(Mathf.Abs(transform.position.x - GameConsts.START_POSITION.x - moveTimer * speed) >= deadZone){
            OnDead();
        }
    }

    public void SetIsGrounded(bool value)
    {
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
        Debug.Log("OnOffGround");
        EventManager.InvokeEvent(EventType.PlayerJumpoffGroundEvent);
    }

    //协程，在jumpTime时间内持续给与一个力
    private IEnumerator JumpForce()
    {
        yield return new WaitForSeconds(jumpSettings.jumpTime);
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
                time = jumpSpeed / gravityScale * 2;
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
        jumpTimer = 0;
        moveTimer = 0;
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
