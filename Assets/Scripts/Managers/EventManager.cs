using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #endregion

    #region 音乐流程控制事件
    GameStartForAudioEvent=300,
    GameRestartForAudioEvent=301,
    GamePauseForAudioEvent=302, 
    GameResumeForAudioEvent=303,
    #endregion


}

// 事件数据参数基类，具体使用时可以继承该类，添加自己需要的参数
public class EventData
{
    
}

public class GameAudioEventData : EventData
{
    public int LevelMusicIndex=0;
    public int LevelResetPointIndex = 0;
    public GameAudioEventData() : base()
    {
            
    }

}

public class EventManager : Singleton<EventManager>
{
    private Dictionary<EventType, Action<EventData>> eventDictionary;

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
    }

    public static void RemoveListener(EventType eventType, Action<EventData> listener)
    {
        if (Instance == null) return;
        Action<EventData> thisEvent;
        if (Instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent -= listener;
            Instance.eventDictionary[eventType] = thisEvent;
        }
    }

    public static void InvokeEvent(EventType eventType, EventData eventData = null)
    {
        Action<EventData> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent?.Invoke(eventData);
        }
    }
}
