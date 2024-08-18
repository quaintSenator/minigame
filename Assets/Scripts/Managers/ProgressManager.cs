using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : Singleton<ProgressManager>
{
    private List<LevelProgressData> levelProgressDataList;
    private Dictionary<int, LevelProgressData> levelProgressDataDic;
    
    private float currentGameTime;
    private bool gameStart;

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
        EventManager.AddListener(EventType.StartLevelEvent, StartLevel);
        EventManager.AddListener(EventType.RestartLevelEvent, StartLevel);
        EventManager.AddListener(EventType.GamePauseEvent, GamePause);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.StartLevelEvent, StartLevel);
        EventManager.RemoveListener(EventType.RestartLevelEvent, StartLevel);
        EventManager.RemoveListener(EventType.GamePauseEvent, GamePause);
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
    
    public void UpdateLevelProgress(LevelProgressData levelProgressData)
    {
        if (levelProgressDataDic.ContainsKey(levelProgressData.levelIndex))
        {
            levelProgressDataDic[levelProgressData.levelIndex] = new LevelProgressData(levelProgressData);
            levelProgressDataList.Find(data => data.levelIndex == levelProgressData.levelIndex).CopyData(levelProgressData);
        }
        else
        {
            levelProgressDataDic.Add(levelProgressData.levelIndex, levelProgressData);
            levelProgressDataList.Add(new LevelProgressData(levelProgressData));
        }
    }

    private void Update()
    {
        if (!gameStart)
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
    }
    
    public void CopyData(LevelProgressData levelProgressData)
    {
        levelName = levelProgressData.levelName;
        levelIndex = levelProgressData.levelIndex;
        levelProgress = levelProgressData.levelProgress;
        isLevelComplete = levelProgressData.isLevelComplete;
        isLevelLocked = levelProgressData.isLevelLocked;
    }
}
