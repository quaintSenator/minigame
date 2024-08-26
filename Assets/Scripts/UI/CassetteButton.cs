using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CassetteButton : MonoBehaviour
{
    [SerializeField]private GameObject m_lockIcon;
    [SerializeField] private int m_id;
    public void OnEnable()
    {
        EventManager.AddListener(EventType.SwitchLevelEvent, OnSwitch);
    }

    private void Start()
    {
        var name = gameObject.name;
        m_id = Int32.Parse(gameObject.name.Substring(name.Length - 1, 1));
        m_lockIcon.SetActive(ProgressManager.Instance.GetLevelLocked(m_id));
    }

    public void OnDisable()
    {
        EventManager.RemoveListener(EventType.SwitchLevelEvent, OnSwitch);
    }

    public void OnSwitch(EventData eventData)
    {
        var clickedLevelNum = ((SwitchLevelEventData)eventData).switchingIntoLevel;
        if (gameObject.name.Contains(clickedLevelNum.ToString()))//我被点了吗
        {
            //后台的
            m_lockIcon.SetActive(ProgressManager.Instance.GetLevelLocked(clickedLevelNum) && m_id == RotatingCassettes.GetMidCassetteId());
        }
    }
    public void OnCassetteClick()
    {
        Debug.Log("OnCassetteClick");
        var clickedLevelNum = -1;
        switch (gameObject.name)
        {
            case "cassette1":
                clickedLevelNum = 1;
                break;
            case "cassette2":
                clickedLevelNum = 2;
                break;
            case "cassette3":
                clickedLevelNum = 3;
                break;
        }
        
        if (!ProgressManager.Instance.GetLevelLocked(clickedLevelNum))
        {
            EnterLevel(clickedLevelNum);
        }
    }
    
    public void EnterLevel(int i)
    {
        var sceneName = "Level_" + i;
        SceneManager.LoadScene(sceneName);
    }
}
