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

public class RhythmViewer : MonoBehaviour
{
    [Space(20)]
    [InfoBox("当前所使用的音频信息", InfoMessageType.None)]
    [SerializeField] private RhythmDataFile rhythmDataFile;
    
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
    private MusicVisualization musicController;
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
    public static bool CurrentMusicIsPlaying => currentMusicIsPlaying;
    private static float currentMusicTime = 0f;
    public static float CurrentMusicTime => currentMusicTime;
    
    private Transform startPoint;
    
    private void Start()
    {
        currentMusicTime = 0f;
        startPoint = GameObject.Find("start_point").transform;
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
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.PauseOrResumeMusicEvent, PauseOrResumeMusic);
        EventManager.RemoveListener(EventType.StopOrPlayMusicEvent, StopOrPlayMusic);
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
    }

    #region 静态节奏区域显示

    private void SpawnRhythmZoneVisual()
    {
        int count = 0;
        foreach (var rhythmData in rhythmDataFile.rhythmDataList)
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.name = "RhythmZoneVisual_" + count;
            go.transform.position = new Vector3(rhythmData.perfectTime * GameConsts.SPEED + startPoint.position.x, -2.72f, 0);
            RhythmZoneVisual rhythmZoneVisual = go.AddComponent<RhythmZoneVisual>();
            rhythmZoneVisual.Init(timeVisualDataList);
            count++;
        }
    }
    
    [DisableIf("noInPlayMode"),Button(ButtonSizes.Large)]
    public void UpdateVisual()
    {
        for (int i = 0; i < rhythmZoneVisuals.Count; i++)
        {
            rhythmZoneVisuals[i].UpdateVisual(timeVisualDataList);
        }
    }

    public static Sprite GetSprite()
    {
        return Sprite.Create(squareTexture, new Rect(0.0f, 0.0f, 1.0f, 1.0f), new Vector2(0.5f, 0.5f), 1.0f);
    }

    #endregion

    #region 运行时节奏区域显示

    private void PauseOrResumeMusic(EventData data)
    {
        if (musicController == null) return;
        if(currentMusicIsPlaying)
        {
            currentMusicIsPlaying = false;
            musicController.PauseLevelMusic();
        }
        else
        {
            currentMusicIsPlaying = true;
            musicController.ResumeLevelMusic();
            musicCurrentPosLine.ShowPosLine();
        }
    }
    
    private void StopOrPlayMusic(EventData data)
    {
        if (musicController == null) return;
        if(currentMusicIsPlaying)
        {
            currentMusicTime = 0f;
            currentMusicIsPlaying = false;
            musicController.StopLevelMusic();
            musicCurrentPosLine.HidePosLine();
            ClearDynamicRhythmNode();
        }
        else
        {
            currentMusicTime = 0f;
            currentMusicIsPlaying = true;
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
        dynamicRhythmNodes.Clear();
    }
        
    private void CheckDynamicRhythmNode()
    {
        if (musicController == null) return;
        if (currentMusicIsPlaying)
        {
            while (leftDynamicRhythmNodeIndex < currentDynamicRhythmNodeIndex)
            {
                RhythmData rhythmData = rhythmDataFile.rhythmDataList[leftDynamicRhythmNodeIndex];
                Vector3 realPosition = new Vector3(rhythmData.perfectTime * GameConsts.SPEED + startPoint.position.x, 0, 0);
                if (!IsPositionInViewport(realPosition))
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
            
            while (rightDynamicRhythmNodeIndex < rhythmDataFile.rhythmDataList.Count)
            {
                RhythmData rhythmData = rhythmDataFile.rhythmDataList[rightDynamicRhythmNodeIndex];
                Vector3 realPosition = new Vector3(rhythmData.perfectTime * GameConsts.SPEED + startPoint.position.x, 0, 0);
                if (IsPositionInViewport(realPosition))
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
