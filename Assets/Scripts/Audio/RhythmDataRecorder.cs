using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class RhythmDataRecorder : Singleton<RhythmDataRecorder>
{
    private List<RhythmData> audioDataList = new List<RhythmData>();
    private float currentRecordTime = 0f;
    private int count = 0;
    

    public void StartRecordTime()
    {
        currentRecordTime = 0f;
        count = 0;
        Debug.Log("Start Record Time");
        StartCoroutine(UpdateTime());
    }
    
    public void RecordTime()
    {
        Debug.Log($"Record No.{count} : {currentRecordTime}");
        count++;
        audioDataList.Add(new RhythmData(count, currentRecordTime));
    }
    
    [Button]
    public void StopRecordTime()
    {
        StopCoroutine(UpdateTime());
        
        // 保存地图数据
        RhythmDataFile saveData = ScriptableObject.CreateInstance<RhythmDataFile>();
        saveData.rhythmDataList = new List<RhythmData>(audioDataList);
        
        //读取路径里面有多少文件
        string path = "Assets/Scripts/Audio/RhythmData";
        string[] files = System.IO.Directory.GetFiles(path);
        string saveName = "RhythmSaveData_" + (files.Length/2 + 1) + ".asset";
        AssetDatabase.CreateAsset(saveData, path + "/" + saveName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Saved tilemaps to Assets/Scripts/TilemapEditorScripts/BuildableFeature/MapForDesigner/MapSaveData.asset");
    }
    
    //协程，更新时间
    private IEnumerator UpdateTime()
    {
        var wait = new WaitForFixedUpdate();
        while (true)
        {
            currentRecordTime += Time.fixedDeltaTime;
            yield return wait;
        }
    }
}

[Serializable]
public class RhythmData
{
    public int count = 0;
    public float perfectTime;
    public RhythmData(int count, float perfectTime)
    {
        this.count = count;
        this.perfectTime = perfectTime;
    }
}
