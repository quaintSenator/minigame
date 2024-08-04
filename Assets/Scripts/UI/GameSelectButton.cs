using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelectButton : MonoBehaviour
{
    //为了催促windowmanager实例化，存一个引用在Start中更新
    private WindowManager _windowManager;
    
    void Start()
    {
        //催促WindowManager实例化
        _windowManager = WindowManager.Instance;
        
    }
    public void OnClickGameSelect()
    {
        _windowManager.InitWindow(WindowType.LevelSelectPage, _windowManager.UIroot());
    }
}
