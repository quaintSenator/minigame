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

public enum JumpType
{
    //默认跳跃
    Default,
    //弹簧跳跃
    Spring,
    //飞行到终点时跳跃
    Fly,
}

[System.Serializable]
public class JumpSettings : System.Object
{
    public static readonly Dictionary<JumpType, Vector2> settings = new Dictionary<JumpType, Vector2>(){
        {JumpType.Default, new Vector2(2.6f, 1.1f)}, {JumpType.Spring, new Vector2(4.8f, 3.0f)}, {JumpType.Fly, new Vector2(2.6f, 1.1f)}
    };

    public float jumpHeight;
    public float jumpDeltaHeight;

    public JumpSettings()
    {
        jumpHeight = settings[JumpType.Default].x;
        jumpDeltaHeight = settings[JumpType.Default].y;
    }
    public JumpSettings(JumpType jumpType)
    {
        jumpHeight = settings[jumpType].x;
        jumpDeltaHeight = settings[jumpType].y;
    }
    public JumpSettings(Vector2 setting)
    {
        jumpHeight = setting.x;
        jumpDeltaHeight = setting.y;
    }
    public JumpSettings(float jumpHeight, float jumpDeltaHeight)
    {
        this.jumpHeight = jumpHeight;
        this.jumpDeltaHeight = jumpDeltaHeight;
    }
    public static implicit operator JumpSettings(Vector2 setting)
    {
        return new JumpSettings(setting);
    }
}

public class PlayerController : MonoBehaviour
{
    public AK.Wwise.Event ActionJumpEvent=null;

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

    [SerializeField]
    [ReadOnly]
    [Tooltip("弹簧上升初速,由参数计算得到")]
    private float jumpSpringSpeed= 5.0f;

    [SerializeField]
    [Tooltip("一次弹簧跳跃最高上升的高度，需要调整的参数，影响弹簧上升初速")]
    private float jumpSpringHeight = 4.8f;


    //角色是否在跳跃
    private bool jumping = false;

    //角色是否在地面上
    private bool isGrounded = true;
    //角色是否已经死亡
    private bool isDead = false;
    //角色是否可以进入飞行模式
    private bool canFly = false;
    //角色是否在飞行模式
    private bool isFlying = false;
    //飞行是否到终点
    private bool isFlyFinished = false;
    //角色是否再跳
    private bool willJump = false;
    //缓冲计时器是否生效
    private bool isBufferActive = false;
    //缓冲计时器计数    ***********已废弃
    private int bufferTimerCount = 0;
    //废弃计时器个数    ***********已废弃
    private int disableTimerCount = 0;
    //跳跃计时器
    private float jumpTimer = 0;

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
    //飞行时竖直速度
    private float verticalVelocity = 0;

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
        InitSettings();
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

        if (isBufferActive)
        {
            jumpTimer += Time.deltaTime;
        }
        if (jumpTimer >= bufferTime)
        {
            jumpTimer = 0;
            isBufferActive = false;
            willJump = false;
        }

        Rotate();
        //CheckDead();
    }

    private void FixedUpdate()
    {
        //角色一直受一个向下的重力，世界坐标系
        if(!isFlying){
            rigidBody.AddForce(ForceManager.Instance.GetGravityDir() * gravityScale);//* GameConsts.GRAVITY);
        }
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
        if (canFly)
        {
            Fly();
        }
        else
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
        Debug.Log("OnSpaceDown");
    }

    private void OnSpacebarUp(EventData DATA = null)
    {
        Debug.Log("OnSpacebarUp");
        //CleverTimerManager.Ask4Timer(bufferTime, OnBufferTimeEnd);
        //bufferTimerCount++;
        if (isFlying)
        {
            EndFly();
        }
        else
        {
            jumpTimer = 0;
            isBufferActive = true;
            isContinueJump = false;
        }

    }

    private void OnMouseLeftClick(EventData data = null)
    {
        //Jump();
    }

    private void OnBufferTimeEnd(EventData data)
    {
/*        if (disableTimerCount > 0)
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
        }*/
    }

    //注册事件统一函数
    private void registerEvents()
    {

    }


    private void PlayNormalJumpAudio()
    {
        if(ActionJumpEvent !=null)
        {
            ActionJumpEvent.Post(gameObject);
        }

    }
    //角色跳跃
    private void Jump()
    {
        EventManager.InvokeEvent(EventType.DecideCanJumpEvent, null);
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
                jumping = false;
                rigidBody.velocity = Vector2.up * jumpSpeed;
                //rigidBody.AddForce(Vector2.up * jumpForce);
                // Impulse
                //rigidBody.impluse
                break;
        }
    }

    private void Fly()
    {
        isFlying = true;
        rigidBody.velocity = Vector2.up * verticalVelocity;
    }

    private void EndFly()
    {
        isFlying = false;
        if(isFlyFinished)
        {
            TryJump(JumpType.Fly);
        }
        else{
            rigidBody.velocity = new Vector2();
        }
    }

    //对外跳跃接口，设置跳跃参数，不传为默认参数
    public void TryJump(JumpType jumpType = JumpType.Default, JumpSettings value = null)
    {
        if(value == null)
        {
            CalSettings(JumpSettings.settings[jumpType]);
        }
        else
        {
            CalSettings(value);
        }

        if (jumpType == JumpType.Default && isGrounded)
        {
            PlayNormalJumpAudio();
            Jump();
        }
        else if (jumpType == JumpType.Spring || jumpType == JumpType.Fly)
        {
            Jump();
        }
    }

    private void InitSettings()
    {
        worldScale = transform.localScale.x;

        speed = speed * worldScale;

        jumpForce = jumpForce * worldScale;


    }

    private void CalSettings(JumpSettings data = null)
    {
        if (data == null)
        {
            jumpSettings = GameConsts.DEFAULT_JUMP;
        }

        CalNormalJumpParameter(data);

        //TODO:这个值应该是从别处拿到或者直接赋的

        jumpSpeed = jumpSpeed * worldScale;
        gravityScale = gravityScale * worldScale;
    }

    private void CalNormalJumpParameter(JumpSettings data = null)
    {
        if(data == null) return;

        float jumpHeight = data.jumpHeight;
        float jumpDeltaHeight = data.jumpDeltaHeight;

        //跳跃一次的上升初速度和最高点位置解析出来会满足一个数量关系
        //jumpHeight = jumpHeight * worldScale;
        if (jumpHeight < JUDGE_ZERO && jumpDeltaHeight < JUDGE_ZERO)
        {
            Debug.LogError("Error jumpHeight！");
            return;
        }

        //一次跳跃从起跳到最高点的时间
        float jumptime = 0;

        jumptime = (float)(Math.Sqrt(jumpHeight / jumpDeltaHeight)
            * jumpDeltaHeight
            / (jumpDeltaHeight + Math.Sqrt(jumpDeltaHeight * jumpHeight))
            * beatTime );

        if (jumptime < JUDGE_ZERO)
        {
            Debug.LogError("Error jumptime！");
            return;
        }

        gravityScale = 2 * jumpHeight / jumptime / jumptime;

        jumpSpeed = jumptime * gravityScale;


        jumpSpringSpeed = (float)Math.Sqrt(2 * jumpSpringHeight * gravityScale);

        /*        jumpSpeed = Math.Sqrt(jumpHeight / jumpHeight - 1) 
                    * (jumpHeight - 1)
                    / ((jumpHeight - 1) + Math.Sqrt(jumpHeight* jumpHeight - jumpHeight) )
                    * gravityScale
                    * beatTime;*/
    }

    private void CheckDead()
    {
        moveTimer += Time.deltaTime;
        if (Mathf.Abs(transform.position.x - GameConsts.START_POSITION.x - moveTimer * speed) >= deadZone)
        {
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

    public void SetCanFly(bool value)
    {
        canFly = value;
    }

    public void SetFlyFinished(bool value)
    {
        isFlyFinished = value;
    }

    public void OnHitGround(EventData data = null)
    {
        if (!isGrounded)
        {
            HitGroundEventData hitdata = new HitGroundEventData(playerHeadingDir);
            EventManager.InvokeEvent(EventType.PlayerHitGroundEvent, hitdata);
        }
        isGrounded = true;
        isReturn = true;
        selfAngle = cubeSprites.eulerAngles.z;
       
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
        yield return new WaitForSeconds(1);
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

        if (!isGrounded && !isFlying)
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
        ResetState();
        ResetJump();
        ResetFly();
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

    public void SetVerticalVelocity(float value)
    {
        verticalVelocity = value * worldScale;
    }

    private void ResetFly()
    {
        isFlying = false;
        canFly = false;
        isFlyFinished = false;
    }

    private void ResetJump()
    {
        isGrounded = true;
        isReturn = false;
        returnTimer = 0;
        willJump = false;
        isContinueJump = false;
        jumpTimer = 0;
        jumpSettings = GameConsts.DEFAULT_JUMP;
        isBufferActive = false;
    }

    private void ResetState()
    {
        transform.position = GameConsts.START_POSITION;
        cubeSprites.rotation = GameConsts.ZERO_ROTATION;
        rigidBody.velocity = GameConsts.START_VELOCITY;

        isDead = false;
        boxCollider.enabled = true;
        moveTimer = 0;
    }
}
