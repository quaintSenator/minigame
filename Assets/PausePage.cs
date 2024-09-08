using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePage : Window
{
    //Start is called before the first frame update
    public new void Start()
    {
        base.Init();
    }
    public void OnClickRestartBtn()
    {
        WindowManager.Instance.ResumeTimePause();
        WindowManager.Instance.ClipUIRoot2Empty();
        EventManager.InvokeEvent(EventType.StartLevelEvent);
    }
    public void OnClickResumeBtn()
    {
        WindowManager.Instance.ResumeGame();
    }

    public void PlayClickAudio()
    {
        UIAudioManager.Instance.PlayBtnClickAudio();
    }
    protected override void onExit()
    {
        base.onExit();
        //在暂停页面，退出需回到选关界面 注意，此时依然需要从暂停中改出
        WindowManager.Instance.ResumeTimePause();
        ProgressManager.Instance.UpdateLevelProgress(WindowManager.Instance.GetLevelIndex());
        Utils.AddMaskAndLoadScene(transform.parent, "GUIScene");
    }
}
