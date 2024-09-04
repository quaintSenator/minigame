using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPage : Window
{
    
    protected override void onExit()
    {
        base.onExit();
        Debug.Log("MainPage.onExit was called");
    }

    public void Go2MusicPage()
    {
        //跳转音乐界面
    }

    public void ClearProgress()
    {
        
    }
}
