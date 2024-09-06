using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : Singleton<DialogManager>
{
    public List<List<string>> levelDialogs1 = new List<List<string>>{
        new List<string>{"我是一个正方形，", "我生活的世界叫作平面国。"},
        new List<string>{"在这里，你得小心女人和士兵。", "士兵的角很锋利，是为了保护贵族和僧侣", "女人的角更尖锐，但她们会藏起来"},
        new List<string>{"你知道东西南北吧？", "我得离开那股向南的引力", "要尽力朝北才行"}
    };

    public List<List<string>> levelDialogs2 = new List<List<string>>{
        new List<string>{"你的视野更开阔了", "怎么样，", "开始理解三次方了吗？"},
        new List<string>{"僧侣只是由无尽的边组成的圆形", "快点跨过他的宫殿吧", "我在最北方等你"},        
    };

    public Dictionary<int, List<List<string>>> dialogs = new Dictionary<int, List<List<string>>>();

    public DialogController dialogController;

    private int levelIndex;
    private int listIndex = 0;
    private int dialogIndex;

    private List<List<int>> resetListIndex =new List<List<int>>{  //神必数字，简单的重置解决办法
        new List<int>{0,0,0,0,0,0},
        new List<int>{0,1,1,2,2,2},
        new List<int>{0,0,0,0,0},
    };

    private List<string> nowDialogList ;

    protected override void OnAwake()
    {
        dialogs.Add(1, levelDialogs1);
        dialogs.Add(2, levelDialogs2);
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.EndLoadMapEvent, OnLoadMap);
        EventManager.AddListener(EventType.PlayerDeadStoryEvent, OnDead);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.EndLoadMapEvent, OnLoadMap);
        EventManager.RemoveListener(EventType.PlayerDeadStoryEvent, OnDead);
    }

    private void OnLoadMap(EventData data)
    {
        var mapData = data as LoadMapDataEvent;
        levelIndex = mapData.index;
    }

    private void OnDead(EventData data)
    {
        LevelEventData levelData = data as LevelEventData;
        int resetIndex = levelData.LevelResetPointIndex;
        Debug.Log("resetIndex"+resetIndex);
        listIndex = resetListIndex[levelIndex - 1][resetIndex];
    }

    private List<string> GetDialogList()
    {
        Debug.Log("dialogLevelIndex"+levelIndex);
        return dialogs[levelIndex][listIndex];
    }

    public void TryShowDialogs()
    {
        Debug.Log("LevelIndex"+levelIndex);
        Debug.Log("listIndex"+listIndex);
        bool hasShown = ProgressManager.Instance.GetDialogShow(levelIndex, listIndex+1);
        if(hasShown){
            listIndex++;
            return;
        }

        nowDialogList = GetDialogList();
        dialogController.ShowDialogs(nowDialogList);
        listIndex++;
        ProgressManager.Instance.UpdateDialogShow(levelIndex, listIndex, true);     //这里使用+1后的listIndex，原因为开场剧情占用index = 0

    }

}
