using System;
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
    
    public void OnEnable()
    {
        //UIAudioManager.Instance.PlayMainUIMusic();
        //isPlayingMusic = true;
    }


    private void Start()
    {
        UIAudioManager.Instance.PlayMainUIMusic();
    }
    public void Go2MusicPage()
    {
        //跳转音乐界面
        WindowManager.Instance.Go2MusicPage();
    }
    public void ClearProgress()
    {
        //重置进度按钮
        ProgressManager.Instance.DeletePlayerPrefsData();
    }

    public void PlayClickAudio()
    {
        UIAudioManager.Instance.PlayBtnClickAudio();
    }
    public void Go2UGCScene()
    {
        UIAudioManager.Instance.StopMainUIMusic();
        Utils.AddMaskAndLoadScene(transform, "TilemapEditorScene");
    }
}
