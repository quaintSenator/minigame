using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePage : Window
{
    // Start is called before the first frame update
    public new void Start()
    {
        base.Init();
    }

    public void OnClickRestartBtn()
    {
        EventManager.InvokeEvent(EventType.GameRestartEvent);
    }
    public void OnClickResumeBtn()
    {
        WindowManager.Instance.ResumeGame();
    }
    // Update is called once per frame
    protected override void onExit()
    {
        base.onExit();
        //在暂停页面，退出需回到选关界面
        SceneManager.LoadScene("GUIScene");
    }
}
