using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectPage : Window
{
    public new void Start()
    {
        base.Init();
    }
    public void PlayClickAudio()
    {
        UIAudioManager.Instance.PlayBtnClickAudio();
    }
    protected override void onExit()
    {
        base.onExit();
        Debug.Log("LevelSelectPage.onExit was called");
    }
}
