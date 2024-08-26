using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeadPage : Window
{
    private string[] s1 = new[] { "这是你的终点？" };
    private string[] s2 = new[] { "向上还是向北？" };
    private string[] s3 = new[] { "看到顶点了吗？" };
    [SerializeField] private Text m_txt;
    public void OnClickRestart()
    {
        Debug.LogWarning("OnclickRestart");
        EventManager.InvokeEvent(EventType.StartPlayerDeadEvent);
        WindowManager.Instance.ClipUIRoot2Empty();
    }

    public void OnEnable()
    {
        DecideText();
    }

    public void DecideText()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Contains("1"))
        {
            m_txt.text = s1[0];
        }
        else
        {
            if(sceneName.Contains("2"))
            {
                m_txt.text = s2[0];
            }
            else
            {
                m_txt.text = s3[0];
            }
        }
    }

    protected override void onExit()
    {
        base.onExit();
        //在暂停页面，退出需回到选关界面
        SceneManager.LoadScene("GUIScene");
    }

}
