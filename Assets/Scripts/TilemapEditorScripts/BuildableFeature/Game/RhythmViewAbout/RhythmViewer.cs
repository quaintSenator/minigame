using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

enum UpdateMode
{
    OnEnable,
    Update,
}

public class RhythmViewer : Singleton<RhythmViewer>
{
    [Space(20)]
    [InfoBox("当前所使用的音频信息", InfoMessageType.None)]
    [SerializeField] private RhythmDataFile rhythmDataFile;
    
    [InfoBox("是否显示静态节奏区域", InfoMessageType.None)]
    [BoxGroup("静态节奏区域显示", centerLabel:true)]
    [SerializeField] private bool showRhythmZone = true;
    [InfoBox("节奏点的偏移量", InfoMessageType.None)]
    [BoxGroup("静态节奏区域显示", centerLabel:true)]
    [SerializeField]private float rhythmOffset = 0f;
    private float lastRhythmOffset = 0f;
    
    [Space(20)]
    [InfoBox("增加则会在原有基础上增加一个区域，区域按顺序显示\n" +
             "Start Time : 开始时间 （离完美点的距离）\n" +
             " End Time  : 结束时间 （离完美点的距离）", InfoMessageType.None)]
    [BoxGroup("静态节奏区域显示", centerLabel:true)]
    [SerializeField] private List<TimeVisualData> timeVisualDataList;
    
    private static Texture2D squareTexture;
    private List<RhythmZoneVisual> rhythmZoneVisuals = new List<RhythmZoneVisual>();
    private bool noInPlayMode = true;

    [BoxGroup("运行时节奏区域显示", centerLabel:true)] [SerializeField]
    private MusicManager musicController;
    [BoxGroup("运行时节奏区域显示", centerLabel:true)] [SerializeField]
    private MusicCurrentPosLine musicCurrentPosLine;
    [BoxGroup("运行时节奏区域显示", centerLabel:true)] [SerializeField]
    private GameObject dynamicRhythmNodePrefab;
    // <在list中的Index, 对应的VisualizedRhythmNode>
    private Dictionary<int, VisualizedRhythmNode> dynamicRhythmNodes = new Dictionary<int, VisualizedRhythmNode>();
    private int currentDynamicRhythmNodeIndex = 0;
    private int leftDynamicRhythmNodeIndex = 0;
    private int rightDynamicRhythmNodeIndex = 0;
    
    
    
    private static bool currentMusicIsPlaying = false;
    // 确定当前是否没开始播放音乐
    private static bool noStart = true;
    public static bool CurrentMusicIsPlaying => currentMusicIsPlaying;
    private static float currentMusicTime = 0f;
    public static float CurrentMusicTime => currentMusicTime;
    
    private Transform startPoint;
    [SerializeField] private float doubleClickGap = 0.2f;
    private float doubleClickTime = 0f;
    
    private void Start()
    {
        currentMusicTime = 0f;
        startPoint = Utils.GetStartPointPostion();
        // 创建一个1x1纹理
        squareTexture = new Texture2D(1, 1);
        squareTexture.SetPixel(0, 0, Color.white); // 设置纹理的颜色
        squareTexture.Apply(); // 应用纹理的改变
        SpawnRhythmZoneVisual();
        noInPlayMode = false;
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.PauseOrResumeMusicEvent, PauseOrResumeMusic);
        EventManager.AddListener(EventType.StopOrPlayMusicEvent, StopOrPlayMusic);
        EventManager.AddListener(EventType.SelectCurrentMusicTimeEvent, SelectCurrentMusicTime);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.PauseOrResumeMusicEvent, PauseOrResumeMusic);
        EventManager.RemoveListener(EventType.StopOrPlayMusicEvent, StopOrPlayMusic);
        EventManager.RemoveListener(EventType.SelectCurrentMusicTimeEvent, SelectCurrentMusicTime);
    }

    private void SelectCurrentMusicTime(EventData obj)
    {
        if(BuildableCreator.Instance.GetTileMode() != TileMode.None || BuildableCreator.Instance.GetInSelectMode())
        {
            return;
        }
        if(doubleClickTime > 0)
        {
            //获取鼠标点击位置
            Vector3 mousePos = Input.mousePosition;
            //将鼠标点击位置转换为世界坐标
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            //计算鼠标点击位置与起始点的距离
            float currentMusicLinePos = worldPos.x - startPoint.position.x;
            Debug.Log("currentMusicLinePos : "+currentMusicLinePos);
            //计算当前音乐播放时间
            currentMusicTime = (float)Math.Round(currentMusicLinePos / GameConsts.SPEED, 3);
            musicCurrentPosLine.ShowPosLine();
        }
        else
        {
            doubleClickTime = doubleClickGap;
        }
    }

    private void Update()
    {
        if (rhythmOffset != lastRhythmOffset)
        {
            lastRhythmOffset = rhythmOffset;
            transform.position = new Vector3(rhythmOffset * GameConsts.SPEED, 0, 0);
        }
        if(currentMusicIsPlaying)
        {
            currentMusicTime += Time.deltaTime;
            CheckDynamicRhythmNode();
        }
        
        doubleClickTime -= Time.deltaTime;
    }

    #region 静态节奏区域显示

    private void SpawnRhythmZoneVisual()
    {
        if(!showRhythmZone)
        {
            return;
        }
        
        int count = 0;
        foreach (var rhythmData in rhythmDataFile.rhythmDataList)
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.name = "RhythmZoneVisual_" + count;
            go.transform.position = new Vector3(rhythmData.perfectTime * GameConsts.SPEED + startPoint.position.x, -2.72f, 0);
            RhythmZoneVisual rhythmZoneVisual = go.AddComponent<RhythmZoneVisual>();
            rhythmZoneVisual.Init(timeVisualDataList);
            rhythmZoneVisuals.Add(rhythmZoneVisual);
            count++;
        }
    }

    public static Sprite GetSprite()
    {
        return Sprite.Create(squareTexture, new Rect(0.0f, 0.0f, 1.0f, 1.0f), new Vector2(0.5f, 0.5f), 1.0f);
    }
    
    [DisableIf("noInPlayMode"),Button(ButtonSizes.Large)]
    public void UpdateVisual()
    {
        if (showRhythmZone)
        {
            foreach (var rhythmZoneVisual in rhythmZoneVisuals)
            {
                Destroy(rhythmZoneVisual.gameObject);
            }
            rhythmZoneVisuals.Clear();
            SpawnRhythmZoneVisual();
        }
        else
        {
            foreach (var rhythmZoneVisual in rhythmZoneVisuals)
            {
                Destroy(rhythmZoneVisual.gameObject);
            }
            rhythmZoneVisuals.Clear();
        }
    }

    #endregion

    #region 运行时节奏区域显示

    private void PauseOrResumeMusic(EventData data)
    {
        if (musicController == null) return;
        ClearDynamicRhythmNode();
        if(currentMusicIsPlaying)
        {
            currentMusicIsPlaying = false;
            musicController.PauseLevelMusic();
        }
        else if (noStart)
        {
            currentMusicIsPlaying = true;
            noStart = false;
            musicController.PlayLevelMusic();
            musicController.SeekLevelMusicByTimeMS((int)(currentMusicTime * 1000));
            musicCurrentPosLine.ShowPosLine();
        }
        else
        {
            currentMusicIsPlaying = true;
            musicController.ResumeLevelMusic();
            musicController.SeekLevelMusicByTimeMS((int)(currentMusicTime * 1000));
            musicCurrentPosLine.ShowPosLine();
        }
    }
    
    private void StopOrPlayMusic(EventData data)
    {
        if (musicController == null) return;
        ClearDynamicRhythmNode();
        if(currentMusicIsPlaying)
        {
            currentMusicTime = 0f;
            currentMusicIsPlaying = false;
            noStart = true;
            musicController.StopLevelMusic();
            musicCurrentPosLine.HidePosLine();
            ClearDynamicRhythmNode();
        }
        else
        {
            currentMusicTime = 0f;
            currentMusicIsPlaying = true;
            noStart = false;
            musicController.PlayLevelMusic();
            musicCurrentPosLine.ShowPosLine();
            InitCurrentDynamicRhythmNode();
        }
    }

    private void InitCurrentDynamicRhythmNode()
    {
        currentDynamicRhythmNodeIndex = 0;
        leftDynamicRhythmNodeIndex = 0;
        rightDynamicRhythmNodeIndex = 0;
        for(int i = 0; i < rhythmDataFile.rhythmDataList.Count; i++)
        {
            RhythmData rhythmData = rhythmDataFile.rhythmDataList[i];
            Vector3 realPosition = new Vector3(rhythmData.perfectTime * GameConsts.SPEED + startPoint.position.x, 0, 0);
            if (IsPositionInViewport(realPosition))
            {
                GameObject go = PoolManager.Instance.SpawnFromPool("DynamicRhythmNode", dynamicRhythmNodePrefab, transform);
                go.transform.position = realPosition;
                VisualizedRhythmNode visualizedRhythmNode = go.GetComponent<VisualizedRhythmNode>();
                dynamicRhythmNodes.Add(i, visualizedRhythmNode);
                currentDynamicRhythmNodeIndex = i;
                rightDynamicRhythmNodeIndex = i + 1;
            }
            else
            {
                break;
            }
        }
    }
    
    private void ClearDynamicRhythmNode()
    {
        foreach (var dynamicRhythmNode in dynamicRhythmNodes)
        {
            PoolManager.Instance.ReturnToPool("DynamicRhythmNode", dynamicRhythmNode.Value.gameObject);
        }
        leftDynamicRhythmNodeIndex = 0;
        rightDynamicRhythmNodeIndex = 0;
        currentDynamicRhythmNodeIndex = 0;
        dynamicRhythmNodes.Clear();
    }
        
    private void CheckDynamicRhythmNode()
    {
        if (musicController == null) return;
        if (currentMusicIsPlaying)
        {
            while (rightDynamicRhythmNodeIndex < rhythmDataFile.rhythmDataList.Count)
            {
                RhythmData rhythmData = rhythmDataFile.rhythmDataList[rightDynamicRhythmNodeIndex];
                Vector3 realPosition = new Vector3(rhythmData.perfectTime * GameConsts.SPEED + startPoint.position.x, 0, 0);
                
                //存在Bug，先全部显示，后续在考虑优化
                if (IsPositionInViewport(realPosition) || true)
                {
                    GameObject go = PoolManager.Instance.SpawnFromPool("DynamicRhythmNode", dynamicRhythmNodePrefab, transform);
                    go.transform.position = realPosition;
                    VisualizedRhythmNode visualizedRhythmNode = go.GetComponent<VisualizedRhythmNode>();
                    dynamicRhythmNodes.Add(rightDynamicRhythmNodeIndex, visualizedRhythmNode);
                    rightDynamicRhythmNodeIndex++;
                }
                else
                {
                    break;
                }
            }
            
            while (currentDynamicRhythmNodeIndex < rightDynamicRhythmNodeIndex)
            {
                RhythmData rhythmData = rhythmDataFile.rhythmDataList[currentDynamicRhythmNodeIndex];
                float startTimeOffset = dynamicRhythmNodes[currentDynamicRhythmNodeIndex].GetStartTimeOffset();
                if (currentMusicTime >= rhythmData.perfectTime - startTimeOffset)
                {
                    dynamicRhythmNodes[currentDynamicRhythmNodeIndex].Play();
                    currentDynamicRhythmNodeIndex++;
                }
                else
                {
                    break;
                }
            }
            
            while (leftDynamicRhythmNodeIndex < currentDynamicRhythmNodeIndex)
            {
                RhythmData rhythmData = rhythmDataFile.rhythmDataList[leftDynamicRhythmNodeIndex];
                Vector3 realPosition = new Vector3(rhythmData.perfectTime * GameConsts.SPEED + startPoint.position.x, 0, 0);
                
                //存在Bug，先不删除，后续在考虑优化
                if (!IsPositionInViewport(realPosition) && false)
                {
                    PoolManager.Instance.ReturnToPool("DynamicRhythmNode", dynamicRhythmNodes[leftDynamicRhythmNodeIndex].gameObject);
                    dynamicRhythmNodes.Remove(leftDynamicRhythmNodeIndex);
                    leftDynamicRhythmNodeIndex++;
                }
                else
                {
                    break;
                }
            }
        }
    }
    
    private bool IsPositionInViewport(Vector3 position)
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(position);
        if (viewportPos.x < -0.5f || viewportPos.x > 1.5f || viewportPos.y < -0.5f || viewportPos.y > 1.5f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion
    
    public Vector3 GetCurrentMusicLinePos()
    {
        return new Vector3(currentMusicTime * GameConsts.SPEED + startPoint.position.x, 0, 0);
    }
}


[Serializable]
public class TimeVisualData
{
    public string name;
    public float startTime;
    public float endTime;
    public Color color;
    public float height;
}
