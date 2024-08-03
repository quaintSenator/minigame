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
    
    [Space(20)]
    [InfoBox("节奏点的偏移量", InfoMessageType.None)]
    [SerializeField]private float rhythmOffset = 0f;
    private float lastRhythmOffset = 0f;
    
    [Space(20)]
    [InfoBox("增加则会在原有基础上增加一个区域，区域按顺序显示\n" +
             "Start Time : 开始时间 （离完美点的距离）\n" +
             " End Time  : 结束时间 （离完美点的距离）", InfoMessageType.None)]
    [SerializeField] private List<TimeVisualData> timeVisualDataList;
    
    
    private static Texture2D squareTexture;
    private List<RhythmZoneVisual> rhythmZoneVisuals = new List<RhythmZoneVisual>();
    private bool noPlaying = true;
    
    private void Start()
    {
        // 创建一个1x1纹理
        squareTexture = new Texture2D(1, 1);
        squareTexture.SetPixel(0, 0, Color.white); // 设置纹理的颜色
        squareTexture.Apply(); // 应用纹理的改变
        SpawnRhythmZoneVisual();
        noPlaying = false;
    }

    private void Update()
    {
        if (rhythmOffset != lastRhythmOffset)
        {
            lastRhythmOffset = rhythmOffset;
            transform.position = new Vector3(rhythmOffset * GameConsts.SPEED, 0, 0);
        }
    }

    private void SpawnRhythmZoneVisual()
    {
        int count = 0;
        foreach (var rhythmData in rhythmDataFile.rhythmDataList)
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.name = "RhythmZoneVisual_" + count;
            go.transform.position = new Vector3(rhythmData.perfectTime * GameConsts.SPEED, -2.72f, 0);
            RhythmZoneVisual rhythmZoneVisual = go.AddComponent<RhythmZoneVisual>();
            rhythmZoneVisual.Init(timeVisualDataList);
            count++;
        }
    }
    
    
    
    
    
    [DisableIf("noPlaying"),Button(ButtonSizes.Large)]
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
