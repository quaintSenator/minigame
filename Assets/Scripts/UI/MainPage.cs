using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        //重置进度按钮
        ProgressManager.Instance.DeletePlayerPrefsData();
    }
    
    public void Go2UGCScene()
    {
        SceneManager.LoadScene("TilemapEditorScene");
    }
}
