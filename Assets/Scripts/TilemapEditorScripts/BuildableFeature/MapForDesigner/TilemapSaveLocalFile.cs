using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapSaveLocalFile : ScriptableObject
{
    // 保存时间
    public string saveTime;
    // 保存的地图数据
    public string mapData;
    
    // 在Inspectors面板上显示一个按钮，点击按钮后执行LoadTilemapFile方法
    [ContextMenu("Load Tilemap File")]
    public void LoadTilemapFile()
    {
    }

    public TilemapSaveLocalFile(string saveTime, string mapData)
    {
        this.saveTime = saveTime;
        this.mapData = mapData;
    }
}
