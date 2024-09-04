using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 事件类型
public enum EventType
{
    GameStartEvent = 1,
    GameRestartEvent = 2,
    PlayerHitGroundEvent = 3,
    PlayerJumpoffGroundEvent = 4,
    TimerDieEvent = 5,
    PlayerDeadEvent = 6,
    GravityInverseEvent = 7,
    MusicStartEvent = 8,
    MusicRecordEvent = 9,
    SwitchLevelEvent = 10,
    DecideCanJumpEvent = 11,
    RegisterResetPointEvent = 12,//重生点动态注册事件
    RegisterResetPointCallbackEvent=13, //重生点动态注册回调事件
    PlayerPassRegisterResetPointEvent =14, //玩家经过重生点
    Ask4PauseEvent = 15,

    #region 输入事件在此添加

    MouseMoveEvent = 101,
    MouseLeftClickEvent = 102,
    MouseRightClickEvent = 103,
    MiddleClickEvent = 104,
    MiddleScrollEvent = 105,
    SpacebarDownEvent = 106,
    SpacebarUpEvent = 107,
    JDownEvent = 108,
    
    #endregion

    #region 地图编辑器事件

    CancelCurrentSelectEvent = 201, // 取消当前选择
    DrawOrEraseEvent = 202, // 画或者擦除
    ChangeTileModeEvent = 203, // 切换画笔模式
    SaveMapEvent = 204, // 保存地图
    LoadMapOneEvent = 205, // 加载地图1
    LoadMapTwoEvent = 206, // 加载地图2
    LoadMapThreeEvent = 207, // 加载地图3
    PauseOrResumeMusicEvent = 208, // 暂停或者播放音乐
    StopOrPlayMusicEvent = 209, // 停止或者播放音乐
    OpenOrCloseCoordinateEvent = 210, // 打开或者关闭坐标
    SelectCurrentMusicTimeEvent = 211, // 选择当前音乐时间
    LeftMoveBuildableEvent = 212, // 左移建筑
    RightMoveBuildableEvent = 213, // 右移建筑
    UpMoveBuildableEvent = 214, // 上移建筑
    DownMoveBuildableEvent = 215, // 下移建筑
    EnterSelectModeEvent = 216, // 进入选择模式
    ExitSelectModeEvent = 217, // 退出选择模式
    SelectBuildableEvent = 218, // 选择建筑
    CancelAllSelectEvent = 219, // 取消所有选择
    ResetCameraEvent = 220, // 重置相机
    StartSelectZoneEvent = 221, // 开始选择区域
    EndSelectZoneEvent = 222, // 结束选择区域
    SetSelectZoneStartPosEvent = 223, // 设置选择区域开始位置
    SetSelectZoneEndPosEvent = 224, // 设置选择区域结束位置
    CompleteSelectZoneEvent = 225, // 完成选择区域
    CompleteContinuousPointEvent = 226, // 完成连续点
    RotateBuildableEvent = 227, // 旋转建筑
    ChangeEnemyTypeEvent = 228, // 改变敌人类型
    UGCSaveMapDataEvent = 229, // UGC保存地图数据
    SelectMusicEvent = 230, // 选择音乐

    #endregion

    #region 音乐流程控制事件
    GameStartForAudioEvent=300,
    GameRestartForAudioEvent=301,
    GamePauseForAudioEvent=302, 
    GameResumeForAudioEvent=303,
    #endregion

    #region 开局小剧场事件
    StartStoryStep1 = 400,
    StartStoryStep2 = 401,
    StartStoryStep3 = 402,
    StartStoryStep4 = 403,
    StartStoryStep5 = 404,
    StartStoryStep6 = 405,
    StartStoryStep7 = 406,
    StartStoryStep8 = 407,
    StartStoryStep9 = 408,
    StartStoryStep10 = 409,

    NextStepEvent = 420,
    //StoryStartEvent = 421,    //废弃，用流程控制的
    //StoryEndEvent = 422,

    StartStory2Step1 = 421,
    StartStory2Step2 = 422,
    StartStory2Step3 = 423,
    StartStory2Step4 = 424,
    StartStory2Step5 = 425,
    StartStory2Step6 = 426,
    StartStory2Step7 = 427,
    StartStory2Step8 = 428,
    StartStory2Step9 = 429,
    StartStory2Step10 = 430,
    StartStory2Step11= 431,
    StartStory2Step12 = 432,

    StartStory3Step1 = 441,
    StartStory3Step2 = 442,
    StartStory3Step3 = 443,
    StartStory3Step4 = 444,
    StartStory3Step5 = 445,
    StartStory3Step6 = 446,
    StartStory3Step7 = 447,
    StartStory3Step8 = 448,
    StartStory3Step9 = 449,
    StartStory3Step10 = 450,
    StartStory3Step11 = 451,
    StartStory3Step12 = 452,
    StartStory3Step13 = 453,
    StartStory3Step14 = 454,
    StartStory3Step15 = 455,
    StartStory3Step16 = 456,
    StartStory3Step17 = 457,
    StartStory3Step18 = 458,
    StartStory3Step19 = 459,
    StartStory3Step20 = 460,
    StartStory3Step21 = 461,

    #endregion

    #region 正式游戏关卡进程时间点事件

    StartLoadBankEvent = 500, // 开始加载音乐
    EndLoadBankEvent = 501, // 结束加载音乐
    EndLoadMapEvent = 502, // 结束加载地图
    EndRespawnEvent = 503, // 结束重生
    StartLevelEvent = 504, // 开始关卡
    RestartLevelEvent = 505, // 重玩关卡
    PlayerDeadStoryEvent = 506, // 玩家死亡剧情
    StartPlayerDeadEvent = 507, // 开始玩家死亡
    EndPlayerDeadEvent = 508, // 结束玩家死亡
    GamePauseEvent = 509, // 游戏暂停
    GameResumeEvent = 510, // 游戏继续
    StartStoryEvent=511,//剧情开始播放
    EndStoryEvent = 512,//剧情开始播放
    //StartLoadMapEvent=513,// 弃置：开始加载地图
    EndPlayerDeadStoryEvent = 514,
    ChangeDirectionEvent = 515,// 改变方向
	
	StartPassLevelEvent=516,//除了Flow以外都别监听
	EndPassLevelEvent=517,//过关（非终章即第三关）需要监听的事件
	EpilogueEvent=518,//终章(即第三关）过关需要监听的事件
    #endregion

    #region Boss事件

    ReleaseLaserEvent = 600,
    ReleaseBulletEvent = 601,
    ReleaseEnemyEvent = 602,
    BossEndEvent = 603,

    #endregion

    #region UI事件
    
    SwitchLevelAnimEndEvent = 1001,
    #endregion
}

// 事件数据参数基类，具体使用时可以继承该类，添加自己需要的参数
public class EventData
{
    
}

public class LevelEventData : EventData
{
    public int LevelIndex=0;
    public int LevelResetPointIndex = 0;
    public int LevelMusicTimeInMS= 0;
    public LevelEventData() : base()
    {
            
    }

}



public class RegisterResetPointEventData : EventData
{
    public Vector3 position = Vector3.zero;
    //public Transform resetpointPosition = null;
    //public int LevelResetPointIndex = 0;
    public RegisterResetPointEventData() : base()
    {

    }

}

public class RegisterResetPointCallbackEventData : EventData
{
    public int index  = 0;
    public bool state = false;
    //public int LevelResetPointIndex = 0;
    public RegisterResetPointCallbackEventData() : base()
    {

    }

}



 public class PlayerPassRegisterResetPointEvent : EventData
{
    public int index = 0;
    //public int LevelResetPointIndex = 0;
    public PlayerPassRegisterResetPointEvent() : base()
    {

    }

}


public class EventManager : Singleton<EventManager>
{
    private Dictionary<EventType, Action<EventData>> eventDictionary;
    private string sceneName = "";
    protected override void OnAwake()
    {
        Init();
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<EventType, Action<EventData>>();
        }
    }
    public static void AddListener(EventType eventType, Action<EventData> listener)
    {
        Instance.CheckNeedClear();
        Action<EventData> myEvent;
        if (Instance.eventDictionary.TryGetValue(eventType, out myEvent))
        {
            myEvent += listener;
            Instance.eventDictionary[eventType] = myEvent;
        }
        else
        {
            myEvent += listener;
            Instance.eventDictionary.Add(eventType, myEvent);
        }

        if (eventType == EventType.UGCSaveMapDataEvent)
        {
            Debug.Log("UGCSaveMapDataEvent AddListener");
        }
    }

    public static void RemoveListener(EventType eventType, Action<EventData> listener)
    {
        if (Instance == null) return;
        Instance.CheckNeedClear();
        Action<EventData> thisEvent;
        if (Instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent -= listener;
            Instance.eventDictionary[eventType] = thisEvent;
        }
    }

    public static void InvokeEvent(EventType eventType, EventData eventData = null)
    {
        Instance.CheckNeedClear();
        if (eventType.GetHashCode() >= 500 && eventType.GetHashCode() < 600)
        {
            Debug.Log("Flow Event:" + eventType.ToString());
        }
        Action<EventData> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent?.Invoke(eventData);
        }
        
        if(eventType == EventType.UGCSaveMapDataEvent)
        {
            Debug.Log("UGCSaveMapDataEvent");
        }
    }

    private void CheckNeedClear()
    {
        if (sceneName != SceneManager.GetActiveScene().name)
        {
            eventDictionary.Clear();
            sceneName = SceneManager.GetActiveScene().name;
        }
    }

    protected override bool NeedDestory()
    {
        return false;
    }
}
