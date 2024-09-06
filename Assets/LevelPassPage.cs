using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPassPage : Window
{
    [SerializeField] private Text m_text;
    public new void Start()
    {
        base.Init();
        var currentLevel = WindowManager.Instance.GetLevelIndex();
        if (currentLevel == 1)
        {
            m_text.text = "学会了飞跃，可以到更远的地方去了";
        }
        else if (currentLevel == 2)
        {
            m_text.text = "成功闯过了僧侣的宫殿，但是这个世界真的到此为止了吗？";
        }
        else
        {
            m_text.text = "";
        }
    }
    public void OnClickRestartBtn()
    {
        //在过关页面场景下，必须改出强停止
        WindowManager.Instance.ResumeGame();
        WindowManager.Instance.ClipUIRoot2Empty();
        EventManager.InvokeEvent(EventType.StartLevelEvent);
    }
    public void OnClickNextLevelBtn()
    {
        WindowManager.Instance.ResumeTimePause();
        var currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName.Contains("Level_1"))
        {
            //去第二关
            Utils.AddMaskAndLoadScene(transform.parent, "Level_2");
        }
        else if (currentSceneName.Contains("Level_2"))
        {
            //去第三关
            Utils.AddMaskAndLoadScene(transform.parent, "Level_3");
        }
        else if(currentSceneName.Contains("Level_3"))
        {
            //TODO:写Ending动画逻辑
        }
        //给调试场景留下切口
#if UNITY_EDITOR
        if (currentSceneName.Contains("Level_TestPassLevel"))
        {
            //去第二关
            Utils.AddMaskAndLoadScene(transform.parent, "Level_2");
        }
#endif
    }
    protected override void onExit()
    {
        base.onExit();
        //退出需回到选关界面
        //注意，此时依然需要从暂停中改出
        WindowManager.Instance.ClipUIRoot2Empty();
        //无需担心自己删自己的问题，Destroy总是在当帧结束后才执行
        WindowManager.Instance.ResumeTimePause();
        Utils.AddMaskAndLoadScene(transform.parent, "GUIScene");
    }
}
