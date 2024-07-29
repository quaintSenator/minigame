using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapSaveLocalFile : ScriptableObject
{
    // 保存时间
    public string saveTime;
    // 保存的地图数据
    public string mapData;
    


    public TilemapSaveLocalFile(string saveTime, string mapData)
    {
        this.saveTime = saveTime;
        this.mapData = mapData;
    }
}
