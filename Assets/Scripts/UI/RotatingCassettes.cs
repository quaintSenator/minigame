using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchLevelEventData : EventData
{
    public int switchingIntoLevel = 0;

    public SwitchLevelEventData(int i)
    {
        switchingIntoLevel = i;
    }
}
public class RotatingCassettes : MonoBehaviour
{
    [SerializeField] private Transform[] cassettes;
    public Animator m_Animator;
    [SerializeField] private static int currentMiddleCassette;
    [SerializeField] private Button _rightScrollBtn;
    [SerializeField] private Button _leftScrollBtn;
    [SerializeField] private Transform[] points;
    [SerializeField] private List<string> musicNames;
    [SerializeField] private Text musicName;
    void Start()
    {
        currentMiddleCassette = 1;
        foreach (Transform childTransform in transform)
        {
            switch (childTransform.name)
            {
                case "cassette1":
                    cassettes[1] = childTransform;
                    break;
                case "cassette2":
                    cassettes[2] = childTransform;
                    break;
                case "cassette3":
                    cassettes[3] = childTransform;
                    break;
                default: break;
            }
        }

        cassettes[3].gameObject.GetComponent<Button>().enabled = false;
        cassettes[3].gameObject.SetActive(false);
        cassettes[2].gameObject.GetComponent<Button>().enabled = false;
        cassettes[2].gameObject.SetActive(false);
        
        m_Animator = gameObject.GetComponent<Animator>();
        m_Animator.enabled = false;
        UpdateSelectingPoints(currentMiddleCassette);
        
        
    }

    public void OnEnable()
    {
        musicNames = new List<string>();
        musicNames.Add("Lovely Road");
        musicNames.Add("Breakthrough");
        musicNames.Add("Up");
        if (gameObject.name.Contains("MusicPlay"))
        {
            Debug.Log("UI asks play: " + 1);
            musicName.text = musicNames[0];
            UIAudioManager.Instance.PlayOrStopLevelMusicByBtn(1);
        }
    }

    public static int GetMidCassetteId()
    {
        return currentMiddleCassette;
    }
    private void lockScrollAndCassetteClick(int except)
    {
        _leftScrollBtn.enabled = false;
        _rightScrollBtn.enabled = false;
        for (var i = 1; i <= 3; i++)
        {
            if (cassettes[i] && i != except)
            {
                var btn = cassettes[i].gameObject.GetComponent<Button>();
                if (btn)
                {
                    btn.enabled = false;
                }
            }
        }
    }
    private void unlockScrollAndCassetteClick()
    {
        _leftScrollBtn.enabled = true;
        _rightScrollBtn.enabled = true;
        for (var i = 1; i <= 3; i++)
        {
            if (cassettes[i])
            {
                var btn = cassettes[i].gameObject.GetComponent<Button>();
                if (btn)
                {
                    btn.enabled = true;
                }
            }
        }
    }
    public void OnRightScroll()
    {
        if (!m_Animator.enabled)
        {
            m_Animator.enabled = true;
        }
        string anim2Play = "cassetteRotateAnimation";
        switch (currentMiddleCassette)
        {
            case 1:
                anim2Play += "3_1";
                cassettes[1].gameObject.SetActive(true);
                cassettes[3].gameObject.SetActive(true);
                currentMiddleCassette = 3;
                break;
            case 2:
                anim2Play += "1_2";
                cassettes[1].gameObject.SetActive(true);
                cassettes[2].gameObject.SetActive(true);
                currentMiddleCassette = 1;
                break;
            case 3:
                anim2Play += "2_3";
                cassettes[2].gameObject.SetActive(true);
                cassettes[3].gameObject.SetActive(true);
                currentMiddleCassette = 2;
                break;
            default: break;
        }
        EventManager.InvokeEvent(EventType.SwitchLevelEvent, new SwitchLevelEventData(currentMiddleCassette));
        lockScrollAndCassetteClick(currentMiddleCassette);
        m_Animator.Play(anim2Play);
        UpdateSelectingPoints(currentMiddleCassette);
        if (gameObject.name.Contains("MusicPlay"))
        {
            Debug.Log("UI asks play: " + currentMiddleCassette);
            UIAudioManager.Instance.PlayOrStopLevelMusicByBtn(currentMiddleCassette);
            musicName.text = musicNames[currentMiddleCassette - 1];
        }
    }
    public void OnLeftScroll()
    {
        if (!m_Animator.enabled)
        {
            m_Animator.enabled = true;
        }
        string anim2Play = "cassetteRotateAnimation";
        switch (currentMiddleCassette)
        {
            case 1:
                anim2Play += "2_1";
                cassettes[1].gameObject.SetActive(true);
                cassettes[2].gameObject.SetActive(true);
                currentMiddleCassette = 2;
                break;
            case 2:
                anim2Play += "3_2";
                cassettes[3].gameObject.SetActive(true);
                cassettes[2].gameObject.SetActive(true);
                currentMiddleCassette = 3;
                break;
            case 3:
                anim2Play += "1_3";
                cassettes[1].gameObject.SetActive(true);
                cassettes[3].gameObject.SetActive(true);
                currentMiddleCassette = 1;
                break;
            default: break;
        }
        EventManager.InvokeEvent(EventType.SwitchLevelEvent, new SwitchLevelEventData(currentMiddleCassette));
        lockScrollAndCassetteClick(currentMiddleCassette);
        m_Animator.Play(anim2Play);
        UpdateSelectingPoints(currentMiddleCassette);
        if (gameObject.name.Contains("MusicPlay"))
        {
            Debug.Log("UI asks play: " + currentMiddleCassette);
            UIAudioManager.Instance.PlayOrStopLevelMusicByBtn(currentMiddleCassette);
            musicName.text = musicNames[currentMiddleCassette - 1];
        }
    }
    private void UpdateSelectingPoints(int c)
    {
        for (int i = 1; i <= 3; i++)
        {
            var image2Change = points[i].gameObject.GetComponent<Image>();
            if (i == c)
            {
                image2Change.color = new Color(253f/255f, 202f/255f, 206f/255f);
            }
            else
            {
                image2Change.color = Color.white;
            }
        }
    }
    public void after1_2()
    {
        Debug.LogWarning("after1_2");
        unlockScrollAndCassetteClick();
        cassettes[2].gameObject.SetActive(false);
        EventManager.InvokeEvent(EventType.SwitchLevelAnimEndEvent, new SwitchLevelEventData(1));
    }
    public void after2_3()
    {
        unlockScrollAndCassetteClick();
        cassettes[3].gameObject.SetActive(false);
        EventManager.InvokeEvent(EventType.SwitchLevelAnimEndEvent, new SwitchLevelEventData(2));
    }
    public void after3_1()
    {
        unlockScrollAndCassetteClick();
        cassettes[1].gameObject.SetActive(false);
        EventManager.InvokeEvent(EventType.SwitchLevelAnimEndEvent, new SwitchLevelEventData(3));
    }

    public void after1_3()
    {
        unlockScrollAndCassetteClick();
        cassettes[3].gameObject.SetActive(false);
        EventManager.InvokeEvent(EventType.SwitchLevelAnimEndEvent, new SwitchLevelEventData(1));
    }

    public void after2_1()
    {
        unlockScrollAndCassetteClick();
        cassettes[1].gameObject.SetActive(false);
        EventManager.InvokeEvent(EventType.SwitchLevelAnimEndEvent, new SwitchLevelEventData(2));
    }

    public void after3_2()
    {
        unlockScrollAndCassetteClick();
        cassettes[2].gameObject.SetActive(false);
        EventManager.InvokeEvent(EventType.SwitchLevelAnimEndEvent, new SwitchLevelEventData(3));
    }
}
