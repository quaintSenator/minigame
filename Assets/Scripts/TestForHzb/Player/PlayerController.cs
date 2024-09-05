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


public class PlayerController : MonoBehaviour
{
    public AK.Wwise.Event ActionJumpEvent = null;

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


    [SerializeField]
    [Tooltip("角色最大下落速度")]
    private float maxFallSpeed = 16.0f;

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
    private float jumpSpringSpeed = 5.0f;

    [SerializeField]
    [Tooltip("一次弹簧跳跃最高上升的高度，需要调整的参数，影响弹簧上升初速")]
    private float jumpSpringHeight = 4.8f;

    [SerializeField]
    [Tooltip("一次弹簧跳跃最高上升的高度，需要调整的参数，影响弹簧上升初速")]
    private List<Vector3> resetpoints = null;

    private Hashtable resetpointIndexHashTable = new Hashtable();

    [SerializeField]
    [Tooltip("下落死亡检测的Y轴坐标")]
    private Transform deadCheckYAxis = null;
    
    //重生点位，影响复活点位和歌曲播放
    //*
    //TODO: 关于关卡的流程，需要考虑设置重生点位的重置
    private int resetPointIndex = 0;

    private float expectedDisplacementXAxis = 0f;

    private float practicalDisplacementXAxis = 0f;

    private bool isFirstStart = true;

    [SerializeField]
    [Tooltip("是否开启下落速度检测")]
    private bool ifSwitchOnFallSpeedLimit = false;

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
    //方向翻转时 水平速度
    private float horizonVelocity = 0;

    //眨眼回正动画触发相关
    private bool isBlink = false;
    private readonly float BLINK_TIME = 0.3f;
    private float blinkTimer;
    private readonly float BLINK_CHANGE_TIME = 0.1f;

    //表情切换相关
    private bool checkVelocity = false;
    private readonly float SPRING_CHANGE_TIME = 0.1f;

    //一些初始化
    private Transform cubeSprites;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;
    private PlayerSpriteController spriteController;

    //是否能进行移动,封装了Getter和Setter
    public bool ifCanMove = false;


    //重生相关
    [SerializeField]
    [Tooltip("一次重生过程的时间长度")]
    private float respawnDuration = 0.5f;
    private bool isRespawning = false;
    private Vector3 nextRespawnPosition= Vector3.zero;
    private Vector3 laseDeadPosition = Vector3.zero;
    private float lastDeadTime = 0.0f;

    [SerializeField]
    [Tooltip("一次重生过程的时间长度")]
    private GameObject respawnMask = null;

    //重生相关
    [SerializeField]
    [Tooltip("TODO")]
    private Vector3 respawnMaskNormalScale = new Vector3(0.1f, 0.1f, 0);

    //重生相关
    [SerializeField]
    [Tooltip("TODO")]
    private Vector3 respawnMaskTargetScale = new Vector3(0.003f, 0.003f, 0);

    private bool isMaskReducing = false;
	
	
	//通关相关
	private bool bHasPassLevel=false;
	
	private bool bHasPassEpilogueLevel=false;

    //public CinemachineVirtualCamera cinemachine=null;


    //切换方向相关
    //是否正在向上
    private bool isMovingTowardUp = false;

    private float nextCanTriggerChangeDirectionTime = 0;
    private float triggerChangeDirectionCD = 10;


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
        spriteController = cubeSprites.Find("cube").GetComponent<PlayerSpriteController>();


        playerHeadingDir = Vector3.right;
        CalSettings();
        InitSettings();
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.StartLevelEvent, OnStartLevelEvent);
        EventManager.AddListener(EventType.RestartLevelEvent, OnRestartLevelEvent);

        EventManager.AddListener(EventType.StartPlayerDeadEvent, OnStartPlayerDeadEvent);

        //EventManager.AddListener(EventType.MouseRightClickEvent, OnDead);
        EventManager.AddListener(EventType.SpacebarDownEvent, OnSpacebarDown);
        EventManager.AddListener(EventType.SpacebarUpEvent, OnSpacebarUp);
        EventManager.AddListener(EventType.MouseLeftClickEvent, OnMouseLeftClick);

        //EventManager.AddListener(EventType.GameStartEvent, OnReset);

        EventManager.AddListener(EventType.RegisterResetPointEvent, OnRegisterResetPoint);
        EventManager.AddListener(EventType.PlayerPassRegisterResetPointEvent, OnPlayerPassRegisterResetPoint);
		
		EventManager.AddListener(EventType.EndPassLevelEvent, OnEndPassLevelEvent);
		EventManager.AddListener(EventType.EpilogueEvent, OnEpilogueEvent);
        EventManager.AddListener(EventType.ChangeDirectionEvent, OnChangeDirectionEvent);

    }

    private void OnDisable()
    {
        //EventManager.RemoveListener(EventType.MouseRightClickEvent, OnDead);

        EventManager.RemoveListener(EventType.SpacebarDownEvent, OnSpacebarDown);
        EventManager.RemoveListener(EventType.SpacebarUpEvent, OnSpacebarUp);
        EventManager.RemoveListener(EventType.MouseLeftClickEvent, OnMouseLeftClick);

        //EventManager.RemoveListener(EventType.GameStartEvent, OnReset);

        EventManager.RemoveListener(EventType.RegisterResetPointEvent, OnRegisterResetPoint);
        EventManager.RemoveListener(EventType.PlayerPassRegisterResetPointEvent, OnPlayerPassRegisterResetPoint);
		
		EventManager.RemoveListener(EventType.EndPassLevelEvent, OnEndPassLevelEvent);
		
		EventManager.RemoveListener(EventType.EpilogueEvent, OnEpilogueEvent);
    }
    private void Start()
    {
        //ResetJump中包含了这项
        //returnTimer = 0;

        //OnReset();
    }

    private void Update()
    {
        if(isRespawning)
        {
            Respawning();

            return;
        }
        if(!isRespawning)
        {
            //处于暂停或者其他状态不进行更新
            if (!ifCanMove)
            {
                return;
            }


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

            if (isBlink)
            {
                blinkTimer += Time.deltaTime;
            }
            if (blinkTimer >= BLINK_TIME)
            {
                blinkTimer = 0;
                isBlink = false;
                DoBlink();
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
            if(checkVelocity){
                CheckSpringVelocity();
            }
            Rotate();
        }
        /* if (Input.GetKey(KeyCode.Space) && rigidBody.velocity.y <= 0 && !isGrounded)
         {
             //Debug.Log("willJump");
             willJump = true;
         }*/



    }


    private void FixedUpdate()
    {

        //处于暂停或者其他状态不进行更新
        if (!ifCanMove)
        {
            return;
        }

        if(!isMovingTowardUp)
        {
			//角色一直受一个向下的重力，世界坐标系
			if (!isFlying) {
				//添加最大下落速度限制
				if (!ifSwitchOnFallSpeedLimit || rigidBody.velocity.y > -maxFallSpeed)
				{
					rigidBody.AddForce(Vector2.down * gravityScale);//* GameConsts.GRAVITY);
				}

			}



            //角色自动向右前进，世界坐标系
            transform.Translate(playerHeadingDir * speed * Time.fixedDeltaTime, Space.World);
            expectedDisplacementXAxis += (playerHeadingDir * speed * Time.fixedDeltaTime).x;
        }
		//if(!isMovingTowardUp)
		else
		{
			transform.Translate(Vector3.up * speed * Time.fixedDeltaTime, Space.World);
		}


        if (!isMovingTowardUp)
        {
            CheckDead();
        }



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
    }

    private void OnSpacebarUp(EventData DATA = null)
    {
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

    public void PassLastFlyPoint(float positionX)
    {
        if (isFlying)
        {
            EndFly(positionX);
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
        if (ActionJumpEvent != null)
        {
            ActionJumpEvent.Post(gameObject);
        }

    }
    //角色跳跃
    private void Jump(JumpType jumpType = JumpType.Default)
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
                if (jumpType == JumpType.Default || jumpType == JumpType.Fly)
                {
                    rigidBody.velocity = Vector2.up * jumpSpeed;
                    ChangeJumpExpression();
                }
                else if (jumpType == JumpType.Spring)
                {
                    rigidBody.velocity = Vector2.up * jumpSpringSpeed;
                    SetCheckVelocity(true);
                }

                //rigidBody.AddForce(Vector2.up * jumpForce);
                // Impulse
                //rigidBody.impluse
                break;
        }
    }

    private void Fly()
    {
        isFlying = true;
        if(!isMovingTowardUp)
        {
            rigidBody.velocity = Vector2.up * verticalVelocity;
        }
        else
        {
            rigidBody.velocity = Vector2.right * horizonVelocity;
        }

    }

    public void ChangeFlyDir(float positionX)
    {
        if(isFlying)
        {
            if (!isMovingTowardUp)
            {
                rigidBody.velocity = Vector2.up * verticalVelocity;
            }
            else
            {
                transform.position = new Vector3(positionX, transform.position.y, transform.position.z);
                rigidBody.velocity = Vector2.right * horizonVelocity;
            }
        }
    }

    public void TrySetPositionXWhenFlyAndDirectionUp(float positionX)
    {
        if(isFlying && !isFlyFinished && isMovingTowardUp)
        {
            transform.position = new Vector3(positionX, transform.position.y, transform.position.z);
            rigidBody.velocity = new Vector2();
        }

    }

    private void EndFly(float positionX = 0)
    {
        isFlying = false;
        if (isFlyFinished && !isMovingTowardUp)
        {
            TryJump(JumpType.Fly);
        }
        else if(isMovingTowardUp)
        {

            rigidBody.velocity = new Vector2();
            //写这段代码的诗人我直接吃，嚯嚯嚯 夸张哦
            if(positionX !=0)
            {
                transform.position = new Vector3(positionX, transform.position.y, transform.position.z);
            }

        }
        else {
            rigidBody.velocity = new Vector2();


        }
    }

    //对外跳跃接口，设置跳跃参数，不传为默认参数
    public void TryJump(JumpType jumpType = JumpType.Default)
    {
        if(!ifCanMove)
        {
            return;
        }

        //CalSettings(null);

        if (jumpType == JumpType.Default && isGrounded)
        {
            PlayNormalJumpAudio();
            Jump();
        }
        else if (jumpType == JumpType.Spring || jumpType == JumpType.Fly)
        {
            Jump(jumpType);
        }
    }

    private void InitSettings()
    {
        worldScale = transform.localScale.x;

        speed = speed * worldScale;

        jumpForce = jumpForce * worldScale;


    }

    private void CalSettings()
    {


        CalNormalJumpParameter();

        //TODO:这个值应该是从别处拿到或者直接赋的
        //根据WorldScale进行缩放
        jumpSpeed = jumpSpeed * worldScale;
        gravityScale = gravityScale * worldScale;
        jumpSpringSpeed = jumpSpringSpeed * worldScale;
    }

    private void CalNormalJumpParameter()
    {

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
            * beatTime);

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
        if(isRespawning  || !ifCanMove)
        {
            return;
        }
        //Check X position 
        //moveTimer += Time.fixedDeltaTime;

        // playerHeadingDir* speed *Time.fixedDeltaTime
        /*
                if (resetPointIndex >=0 && resetPointIndex < resetpoints.Count)
                {*/
        //if (Mathf.Abs(transform.position.x - resetpoints[resetPointIndex].position.x - moveTimer * speed) >= deadZone)
        if (expectedDisplacementXAxis - transform.position.x >= deadZone && !isMovingTowardUp)
        {
            OnDead();
        }
        //}
        /*        else
                {
                    Debug.LogError("wrong resetPointIndex or wrong resetpoints");
                }*/


        //Check Y position
        if (transform.position.y <= deadCheckYAxis.position.y)
        {
            OnDead();
        }


    }

    public void SetIsGrounded(bool value)
    {
        isGrounded = value;
    }

    //没看懂这个函数拿来干嘛的，先注释了
    /*  public void SetIsDead(bool value)
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
        }*/

    public void SetCanFly(bool value)
    {
        canFly = value;
    }

    public void SetFlyFinished(bool value)
    {
        isFlyFinished = value;
    }

    public void SetCheckVelocity(bool value)
    {
        checkVelocity = value;
        SetSprite(new ExpressionTypeData(ExpressionType.CLOSE, 20));
    }

    public void OnHitGround(EventData data = null)
    {
        var d = (HitGroundEventData)data;
        if (!isGrounded)
        {
            HitGroundEventData hitdata = new HitGroundEventData(d.other, d.hitType);
            EventManager.InvokeEvent(EventType.PlayerHitGroundEvent, hitdata);
        }
        isGrounded = true;
        isReturn = true;
        isBlink = true;
        SetSprite(new ExpressionTypeData(ExpressionType.IDLE, 99));
        selfAngle = cubeSprites.eulerAngles.z;

        if (willJump || isContinueJump)
        {
            if (!isContinueJump)
                willJump = false;
            TryJump();
        }
    }

    public void OnOffGround(EventData data = null)
    {
        isGrounded = false;
        isReturn = false;
        isBlink = false;
        returnTimer = 0;
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

    private void CheckSpringVelocity()
    {
        if(rigidBody.velocity.y < 1f)
        {
            checkVelocity = false;
            SetSprite(new ExpressionTypeData(ExpressionType.AFRAID, 20));
            CleverTimerManager.Ask4Timer(SPRING_CHANGE_TIME, SetSprite, new ExpressionTypeData(ExpressionType.IDLE, 20));
        }
    }

    private void ChangeJumpExpression()
    {
        SetSprite(new ExpressionTypeData(ExpressionType.CLOSE, 10));
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
        Debug.Log("setCorrect");
    }

    private void DoBlink()
    {
        CleverTimerManager.Ask4Timer(BLINK_CHANGE_TIME, SetSprite, new ExpressionTypeData(ExpressionType.CLOSE, 5));
        CleverTimerManager.Ask4Timer(BLINK_CHANGE_TIME * 2, SetSpriteCorrect);
    }

    private void SetSprite(EventData data = null)
    {
        spriteController.SetSprite(data);
    }

    private void SetSpriteCorrect(EventData data = null)
    {
        spriteController.SetCorrect();
    }

    public void OnDead(EventData data = null)
    {
        
        SetIfCanMove(false);
        rigidBody.velocity = GameConsts.START_VELOCITY;

        if (resetPointIndex >= resetpoints.Count || resetPointIndex < 0)
        {
            Debug.LogError("PlayerController::OnDead :get wrong resetPointIndex in PlayerController::ResetAudio");
            return;
        }

        if (speed == 0)
        {
            Debug.LogError("PlayerController::OnDead :get wrong speed in PlayerController::ResetAudio");
            return;
        }

        boxCollider.enabled = false;


        //TODO :死亡动画、音效相关补充,添加到OnStartPlayerDead也可

        LevelEventData levelEventData = new LevelEventData();
        
        //计算当前复活点到最初复活点的距离，根据速度换算为毫秒
        int seekTime = (int)Mathf.Ceil((resetpoints[resetPointIndex].x - resetpoints[0].x) / speed * 1000);
        levelEventData.LevelMusicTimeInMS = seekTime;
        levelEventData.LevelResetPointIndex = resetPointIndex;
        //现在死亡流程走Flow，这里只触发事件
        EventManager.InvokeEvent(EventType.PlayerDeadStoryEvent, levelEventData);
         // OnReset();
    }

    public void OnReset(EventData data = null)
    {
        laseDeadPosition = transform.position;
        lastDeadTime = Time.time;
        ResetState();
        ResetJump();
        ResetFly();
        ResetDeacCheck();
        //ResetAudio();
        isRespawning = true;
        isMaskReducing = true;
        //ResetPositionAndDeacCheck();
        //EventManager.InvokeEvent(EventType.EndRespawnEvent);
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

    public void SetHorizonVelocity(float value)
    {
        horizonVelocity = value * worldScale;
    }

    private void Respawning()
    {
        /*        if(NearlyEqualsVector3(transform.position,nextRespawnPosition))
                {
                    isRespawning = false;
                    EventManager.InvokeEvent(EventType.EndRespawnEvent);

                    return;
                }
                else
                {

                    float elapsedTime = Time.time - lastDeadTime; // 已经经过的时间
                    float t = Mathf.Clamp01(elapsedTime / respawnDuration); // 计算插值因子，确保在0和1之间

                    transform.position = Vector3.Lerp(laseDeadPosition, nextRespawnPosition, t);
                }*/
        //ResetPositionAndDeacCheck();

        if( respawnMask ==null)
        {
            Debug.LogError("can not get the right mask");
            return;

        }
        if(isMaskReducing)
        {
            if (NearlyEqualsVector3(respawnMask.transform.localScale, respawnMaskTargetScale,0.000001f))
            {
                //isRespawning = false;
                //EventManager.InvokeEvent(EventType.EndRespawnEvent);
                isMaskReducing = false;
                transform.position = nextRespawnPosition;
                return;
            }
            else
            {

                float elapsedTime = Time.time - lastDeadTime; // 已经经过的时间
                float t = Mathf.Clamp01(elapsedTime / respawnDuration *2); // 计算插值因子，确保在0和1之间

                respawnMask.transform.localScale = Vector3.Lerp(respawnMaskNormalScale, respawnMaskTargetScale, t);
            }
        }
        else
        {
            if (NearlyEqualsVector3(respawnMask.transform.localScale, respawnMaskNormalScale, 0.000001f))
            {
                isRespawning = false;
                EventManager.InvokeEvent(EventType.EndRespawnEvent);
                //isMaskReducing = false;
                return;
            }
            else
            {

                float elapsedTime = Time.time - lastDeadTime - respawnDuration / 2; // 已经经过的时间
                float t = Mathf.Clamp01(elapsedTime / respawnDuration * 2); // 计算插值因子，确保在0和1之间

                respawnMask.transform.localScale = Vector3.Lerp(respawnMaskTargetScale, respawnMaskNormalScale, t);
            }
        }


    }

    private void ResetLevelState()
    {
        laseDeadPosition = transform.position;
        lastDeadTime = Time.time;
        ResetState();
        ResetJump();
        ResetFly();
        ResetDeacCheck();
        resetPointIndex = 0;
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
        isBlink = false;
        returnTimer = 0;
        willJump = false;
        isContinueJump = false;
        jumpTimer = 0;
        isBufferActive = false;
    }

    private void ResetState()
    {
        //重生在重生点 清空死亡坐标计算累计
        //位置相关重设单独抽离到ResetPositionAndDeacCheck()
        cubeSprites.rotation = GameConsts.ZERO_ROTATION;
        rigidBody.velocity = GameConsts.START_VELOCITY;

        isDead = false;
        boxCollider.enabled = true;
        moveTimer = 0;

        isMovingTowardUp = false;
        nextCanTriggerChangeDirectionTime = -1;
    }

    private void ResetDeacCheck()
    {
        //May be check the reset point if valid position
        //Vector3 resetPosition = new Vector3();

        if (resetPointIndex < resetpoints.Count && resetPointIndex >= 0)
        {
            nextRespawnPosition = resetpoints[resetPointIndex];
        }
        else
        {
            if (resetpoints.Count >= 1)
            {
                nextRespawnPosition = resetpoints[0];

            }
            else
            {
                Debug.LogError("Error ResetPoints");
                return;
            }

        }

        //transform.position = nextRespawnPosition;
        expectedDisplacementXAxis = nextRespawnPosition.x;
        practicalDisplacementXAxis = 0f;


    }

    private void ResetPosition()
    {
        if(resetpoints.Count<=0)
        {
            Debug.LogError("Can not get right reset point");
            return;
        }
        transform.position = resetpoints[0];
    }
    private void ResetAudio()
    {
        LevelEventData gameAudioEventData = new LevelEventData();
        //暂时一个Scene对应一个LevelMusic
        //gameAudioEventData.LevelMusicIndex=
        gameAudioEventData.LevelResetPointIndex = resetPointIndex;
        if (resetPointIndex >= resetpoints.Count || resetPointIndex <0)
        {
            Debug.LogError("get wrong resetPointIndex in PlayerController::ResetAudio");
            return;
        }

        if (speed  == 0)
        {
            Debug.LogError("get wrong speed in PlayerController::ResetAudio");
            return;
        }

        //计算当前复活点到最初复活点的距离，根据速度换算为毫秒
        float seekTime = (resetpoints[resetPointIndex].x - resetpoints[0].x) / speed *1000;

        gameAudioEventData.LevelMusicTimeInMS = (int)seekTime;
        //TODO :这里逻辑有问题 之后再改
        if (resetPointIndex == 0)
        {
            EventManager.InvokeEvent(EventType.GameStartForAudioEvent, gameAudioEventData);
        }
        else
        {
            EventManager.InvokeEvent(EventType.GameRestartEvent, gameAudioEventData);
        }

    }



    private void  OnRegisterResetPoint(EventData eventData)
    {
        RegisterResetPointEventData registerResetPointEventData= eventData as RegisterResetPointEventData;

        if (registerResetPointEventData != null)
        {
            RegisterResetPointCallbackEventData registerResetPointCallbackEventData = new RegisterResetPointCallbackEventData();
            Vector3 resetpointPosition = registerResetPointEventData.position;
            //Transform resetpointPosition = registerResetPointEventData.resetpointPosition;

            //第一次注册(X坐标一定是唯一的，直接作为键值）
            if(!resetpointIndexHashTable.Contains(resetpointPosition))
            {
                resetpoints.Add(resetpointPosition);

                registerResetPointCallbackEventData.state = false;
                resetpointIndexHashTable.Add(resetpointPosition, resetpoints.Count - 1);

                registerResetPointCallbackEventData.index = resetpoints.Count - 1;

            }
            //if(!resetpointIndexHashTable.Contains(resetpointPosition))
            else
            {
                int registerResetPointIndex = (int)resetpointIndexHashTable[resetpointPosition];
                registerResetPointCallbackEventData.index = registerResetPointIndex;
                registerResetPointCallbackEventData.state = registerResetPointIndex <= resetPointIndex;

            }

            EventManager.InvokeEvent(EventType.RegisterResetPointCallbackEvent, registerResetPointCallbackEventData);


        }
          
    }

    private void OnPlayerPassRegisterResetPoint(EventData eventData)
    {
        PlayerPassRegisterResetPointEvent playerPassRegisterResetPointEvent= eventData as PlayerPassRegisterResetPointEvent;

        if (playerPassRegisterResetPointEvent != null)
        {
            resetPointIndex = playerPassRegisterResetPointEvent.index;

        }
    }
	
	private void OnEndPassLevelEvent(EventData eventData)
	{
		//TODO,控制相机 和持续移动
		SetIfCanMove(false);
	}
	
	
	private void OnEpilogueEvent(EventData eventData)
	{
		SetIfCanMove(false);
	}


    private void OnChangeDirectionEvent(EventData eventData)
    {
        if (Time.time >nextCanTriggerChangeDirectionTime )
        {
            nextCanTriggerChangeDirectionTime = triggerChangeDirectionCD + Time.time;
            Debug.Log("pass change");
            isMovingTowardUp = !isMovingTowardUp;
            expectedDisplacementXAxis = transform.position.x;
            rigidBody.velocity = new Vector2();
        }


       
    }

    private void OnStartLevelEvent(EventData eventData)
    {
        //OnReset();
        ResetPosition();
        ResetLevelState();
        SetIfCanMove(true);


    }

    private void OnRestartLevelEvent(EventData eventData)
    {
        SetIfCanMove(true);
        //OnReset();
    }


    private void OnStartDeadEvent(EventData eventData)
    {
        OnReset();
    }


    private void OnStartPlayerDeadEvent(EventData eventData)
    {
        //TODO :死亡动画、音效相关补充,添加到OnDead也可
        Debug.LogWarning("Player:OnStartPlayerDeadEvent");
        //如果没有额外的其他类的处理，这里就
        EventManager.InvokeEvent(EventType.EndPlayerDeadEvent);

        OnReset();
    }

    #region HelperFunctions
    private void SetIfCanMove(bool newIfCanMove)
    {
        this.ifCanMove = newIfCanMove;
    }

    private bool GetIfCanMove()
    {
        return this.ifCanMove;
    }


    public static bool NearlyEqualsVector3(Vector3 vector1 , Vector3 vector2, float tolerance=0.5f)
    {
        return (vector1 - vector2).sqrMagnitude < tolerance;
    }
    #endregion
}
