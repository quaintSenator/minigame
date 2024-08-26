using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : Singleton<ProgressManager>
{
    [SerializeField] List<int> levelMusicMaxTimeList;
    [SerializeField] private List<LevelProgressData> levelProgressDataList;
    private Dictionary<int, LevelProgressData> levelProgressDataDic;
    
    private float currentGameTime = 0;
    private bool gameStart = false;
    private int currentLevelIndex = 1;

    protected override void OnAwake()
    {
        string data = PlayerPrefs.GetString(GameConsts.PROGRESS_DATA_LIST);
        if (data != "")
        {
            levelProgressDataList = JsonUtility.FromJson<SerializeBridge<LevelProgressData>>(data).list;
        }
        else
        {
            levelProgressDataList = new List<LevelProgressData>();
        }
        
        levelProgressDataDic = new Dictionary<int, LevelProgressData>();
        foreach (var levelProgressData in levelProgressDataList)
        {
            levelProgressDataDic.Add(levelProgressData.levelIndex, levelProgressData);
        }
    }
    
    private void OnEnable()
    {
        EventManager.AddListener(EventType.EndLoadMapEvent, OnLoadMap);
        EventManager.AddListener(EventType.StartLevelEvent, StartLevel);
        EventManager.AddListener(EventType.RestartLevelEvent, StartLevel);
        EventManager.AddListener(EventType.GamePauseEvent, GamePause);
        EventManager.AddListener(EventType.EndPlayerDeadEvent, OnPlayerDead);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.EndLoadMapEvent, OnLoadMap);
        EventManager.RemoveListener(EventType.StartLevelEvent, StartLevel);
        EventManager.RemoveListener(EventType.RestartLevelEvent, StartLevel);
        EventManager.RemoveListener(EventType.GamePauseEvent, GamePause);
        EventManager.RemoveListener(EventType.EndPlayerDeadEvent, OnPlayerDead);
    }

    private void OnPlayerDead(EventData obj)
    {
        float progress = currentGameTime / levelMusicMaxTimeList[currentLevelIndex-1];
        UpdateLevelProgress(currentLevelIndex, progress);
    }

    private void OnLoadMap(EventData obj)
    {
        var data = obj as LoadMapDataEvent;
        currentLevelIndex = data.index;
    }

    private void GamePause(EventData obj)
    {
        gameStart = false;
    }

    private void StartLevel(EventData obj)
    {
        currentGameTime = 0;
        gameStart = true;
    }
    
    public void UpdateLevelProgress(int levelIndex, float progress)
    {
        if (!levelProgressDataDic.ContainsKey(levelIndex))
        {
            InitLevelData(levelIndex);
        }
        if(progress > levelProgressDataDic[levelIndex].levelProgress)
        {
            levelProgressDataDic[levelIndex].levelProgress = progress;
            levelProgressDataList.Find(data => data.levelIndex == levelIndex).levelProgress = progress;
        }
        SaveLevelData();
    }
    
    public void UpdateLevelComplete(int levelIndex, bool isComplete)
    {
        if (!levelProgressDataDic.ContainsKey(levelIndex))
        {
            InitLevelData(levelIndex);
        }
        levelProgressDataDic[levelIndex].isLevelComplete = isComplete;
        levelProgressDataList.Find(data => data.levelIndex == levelIndex).isLevelComplete = isComplete;
        SaveLevelData();
    }
    
    public void UpdateLevelLocked(int levelIndex, bool isLocked)
    {
        if (!levelProgressDataDic.ContainsKey(levelIndex))
        {
            InitLevelData(levelIndex);
        }
        levelProgressDataDic[levelIndex].isLevelLocked = isLocked;
        levelProgressDataList.Find(data => data.levelIndex == levelIndex).isLevelLocked = isLocked;
        SaveLevelData();
    }

    public void UpdateDialogShow(int levelIndex, int dialogIndex, bool isShown)
    {
        if (!levelProgressDataDic.ContainsKey(levelIndex))
        {
            InitLevelData(levelIndex);
        }
        levelProgressDataDic[levelIndex].dialogsShows[dialogIndex] = isShown;
        levelProgressDataList.Find(data => data.levelIndex == levelIndex).dialogsShows[dialogIndex] = isShown;
        SaveLevelData();       
    }

    public void InitLevelData(int levelIndex)
    {
        bool isLocked = levelIndex > 1;
        LevelProgressData levelProgressData = new LevelProgressData("level_" + levelIndex, levelIndex, 0, false, isLocked);
        levelProgressDataDic.Add(levelIndex, levelProgressData);
        levelProgressDataList.Add(levelProgressData);
    }
    
    /// <summary>
    /// 获取当前关卡进度
    /// </summary>
    /// <returns>返回 当前时间 / 总音乐时间</returns>
    public float GetCurrentProgress()
    {
        return currentGameTime / levelMusicMaxTimeList[currentLevelIndex-1];
    }
    
    /// <summary>
    /// 返回指定关卡进度
    /// </summary>
    /// <param name="levelIndex">关卡Index，从1开始</param>
    /// <returns>返回 当前时间 / 总音乐时间</returns>
    public float GetLevelProgress(int levelIndex)
    {
        if (!levelProgressDataDic.ContainsKey(levelIndex))
        {
            InitLevelData(levelIndex);
        }
        return levelProgressDataDic[levelIndex].levelProgress;
    }
    
    /// <summary>
    /// 返回关卡是否完成
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <returns></returns>
    public bool GetLevelComplete(int levelIndex)
    {
        if (!levelProgressDataDic.ContainsKey(levelIndex))
        {
            InitLevelData(levelIndex);
        }
        return levelProgressDataDic[levelIndex].isLevelComplete;
    }
    
    /// <summary>
    /// 返回关卡是否锁定
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <returns></returns>
    public bool GetLevelLocked(int levelIndex)
    {
        if (!levelProgressDataDic.ContainsKey(levelIndex))
        {
            InitLevelData(levelIndex);
        }
        return levelProgressDataDic[levelIndex].isLevelLocked;
    }

    public bool GetDialogShow(int levelIndex, int dialogIndex)
    {
        if (!levelProgressDataDic.ContainsKey(levelIndex))
        {
            InitLevelData(levelIndex);
        }
        return levelProgressDataDic[levelIndex].dialogsShows[dialogIndex];        
    }

    private void SaveLevelData()
    {
        string data = JsonUtility.ToJson(new SerializeBridge<LevelProgressData>(levelProgressDataList));
        PlayerPrefs.SetString(GameConsts.PROGRESS_DATA_LIST, data);
    }

    private void Update()
    {
        if (gameStart)
        {
            currentGameTime += Time.deltaTime;
        }
    }
}


[Serializable]
public class LevelProgressData
{
    public string levelName;
    public int levelIndex;
    public float levelProgress;
    public bool isLevelComplete;
    public bool isLevelLocked;

    public List<bool> dialogsShows = new List<bool>();
    
    public LevelProgressData(string levelName, int levelIndex, float levelProgress, bool isLevelComplete, bool isLevelLocked)
    {
        this.levelName = levelName;
        this.levelIndex = levelIndex;
        this.levelProgress = levelProgress;
        this.isLevelComplete = isLevelComplete;
        this.isLevelLocked = isLevelLocked;
    }
    
    public LevelProgressData(LevelProgressData levelProgressData)
    {
        levelName = levelProgressData.levelName;
        levelIndex = levelProgressData.levelIndex;
        levelProgress = levelProgressData.levelProgress;
        isLevelComplete = levelProgressData.isLevelComplete;
        isLevelLocked = levelProgressData.isLevelLocked;
        dialogsShows = levelProgressData.dialogsShows;
    }
    
    public void CopyData(LevelProgressData levelProgressData)
    {
        levelName = levelProgressData.levelName;
        levelIndex = levelProgressData.levelIndex;
        levelProgress = levelProgressData.levelProgress;
        isLevelComplete = levelProgressData.isLevelComplete;
        isLevelLocked = levelProgressData.isLevelLocked;
        dialogsShows = levelProgressData.dialogsShows;
    }
}

