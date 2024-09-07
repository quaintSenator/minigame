using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProgressManager : Singleton<ProgressManager>
{
    private List<int> levelMusicMaxTimeList;
    private List<LevelProgressData> levelProgressDataList;
    private Dictionary<int, LevelProgressData> levelProgressDataDic;

    //储存台词触发点个数，初始化ProgressData中的List用, 0用来占位
    static public int[] dialogNums = {0, 4, 3, 1};
    
    private float currentGameTime = 0;
    private bool gameStart = false;
    private int currentLevelIndex = 1;
    private float lastRecordTime = 0;

    protected override void OnAwake()
    {
        levelMusicMaxTimeList = new List<int>(GameConsts.MUSIC_TIME_LIST);
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
        EventManager.AddListener(EventType.RestartLevelEvent, RestartLevel);
        EventManager.AddListener(EventType.GamePauseEvent, GamePause);
        EventManager.AddListener(EventType.GameResumeEvent, GameResume);
        EventManager.AddListener(EventType.PlayerDeadStoryEvent, OnPlayerDead);
        EventManager.AddListener(EventType.PlayerPassRegisterResetPointEvent, OnPlayerPassRegisterResetPoint);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.EndLoadMapEvent, OnLoadMap);
        EventManager.RemoveListener(EventType.StartLevelEvent, StartLevel);
        EventManager.RemoveListener(EventType.RestartLevelEvent, RestartLevel);
        EventManager.RemoveListener(EventType.GamePauseEvent, GamePause);
        EventManager.RemoveListener(EventType.GameResumeEvent, GameResume);
        EventManager.RemoveListener(EventType.PlayerDeadStoryEvent, OnPlayerDead);
        EventManager.RemoveListener(EventType.PlayerPassRegisterResetPointEvent, OnPlayerPassRegisterResetPoint);
    }

    private void GameResume(EventData obj)
    {
        gameStart = true;
    }

    private void OnPlayerPassRegisterResetPoint(EventData obj)
    {
        lastRecordTime = currentGameTime;
    }

    private void RestartLevel(EventData obj)
    {
        currentGameTime = lastRecordTime;
        gameStart = true;
    }

    private void OnPlayerDead(EventData obj)
    {
        gameStart = false;
        currentGameTime = lastRecordTime;
        float progress = currentGameTime / levelMusicMaxTimeList[currentLevelIndex - 1];
        UpdateLevelProgress(currentLevelIndex, progress);
    }

    private void OnLoadMap(EventData obj)
    {
        Debug.Log("currentLevelIndex" + currentLevelIndex);
        var data = obj as LoadMapDataEvent;
        currentGameTime = 0;
        currentLevelIndex = data.index;
    }

    private void GamePause(EventData obj)
    {
        UpdateLevelProgress(currentLevelIndex);
        gameStart = false;
    }

    private void StartLevel(EventData obj)
    {
        currentGameTime = 0;
        lastRecordTime = 0;
        gameStart = true;
    }
    
    public void UpdateLevelProgress(int levelIndex, float progress)
    {
        progress = Mathf.Clamp(progress, 0, 1);
        Debug.Log($"UpdateLevelComplete {levelIndex} : {progress} ");
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

    public void UpdateLevelProgress(int levelIndex)
    {
        float progress = currentGameTime / levelMusicMaxTimeList[currentLevelIndex - 1];
        UpdateLevelProgress(levelIndex, progress);
    }
    public void UpdateLevelComplete(int levelIndex, bool isComplete)
    {
        Debug.Log($"UpdateLevelComplete {levelIndex} : {isComplete} ");
        if (!levelProgressDataDic.ContainsKey(levelIndex))
        {
            InitLevelData(levelIndex);
        }
        levelProgressDataDic[levelIndex].isLevelComplete = isComplete;
        levelProgressDataList.Find(data => data.levelIndex == levelIndex).isLevelComplete = isComplete;
        if (isComplete)
        {
            UpdateLevelProgress(levelIndex, 1);
        }
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

    [Button]
    public void DeletePlayerPrefsData()
    {
        PlayerPrefs.DeleteKey(GameConsts.PROGRESS_DATA_LIST);
        levelProgressDataDic.Clear();
        levelProgressDataList.Clear();
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
        Debug.Log($"GetLevelProgress {levelIndex} : {levelProgressDataDic[levelIndex].levelProgress} ");
        return Mathf.Clamp(levelProgressDataDic[levelIndex].levelProgress, 0, 1);
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

    #region HelperFunction
    public int GetCurrentLevelIndex()
    {
        return this.currentLevelIndex;
    }
    #endregion
}


[Serializable]
public class LevelProgressData
{
    public string levelName;
    public int levelIndex;  //使用说明：从1开始计数，通过LoadMapEndEvent由LoadMapDataEvent赋值来
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

        if(levelIndex >= 0 && levelIndex <= 3)
        {
            int length = ProgressManager.dialogNums[levelIndex];
            Debug.Log("length"+length);
            for(int i = 0; i < length; i++){
                dialogsShows.Add(false);
            }
        }
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

