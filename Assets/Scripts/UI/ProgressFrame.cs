using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressFrame : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Text mProgressText;
    [SerializeField] private Image mProgressContent;
    private float initialFrameWidth;
    private float initialFrameHeight;
    [SerializeField] private Image mCirc;
    [SerializeField] private float _frameContentWidth;
    
    void Start()
    {
        var levelID = 1;
        if (!SceneManager.GetActiveScene().name.Contains("GUI"))
        {
            //暂停页面也在使用
            levelID = WindowManager.Instance.GetLevelIndex();
        }
        //否则，选关页面，levelID用1
        mProgressText.text = GetCurrentLevelProgressText(levelID);
        var m_rect = gameObject.GetComponent<RectTransform>();
        initialFrameWidth = m_rect.sizeDelta.x;
        initialFrameHeight = m_rect.sizeDelta.y;
        UpdateProgressContent(levelID);
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.SwitchLevelAnimEndEvent, OnSwitchedLevel);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.SwitchLevelAnimEndEvent, OnSwitchedLevel);
    }

    private string GetCurrentLevelProgressText(int i)
    {
        if (i == 0)
        {
            return "#ERROR";
        }
        StringBuilder sb = new StringBuilder();
        sb.Append(Math.Round(UpdateProgressContent(i), 2).ToString());
        sb.Append("%");
        return sb.ToString();
    }

    private void OnSwitchedLevel(EventData ed)
    {
        var intoLevel = ((SwitchLevelEventData)ed).switchingIntoLevel;
        mProgressText.text = GetCurrentLevelProgressText(intoLevel);
        UpdateProgressContent(intoLevel);
    }
    
    private float UpdateProgressContent(int i)
    {
        var progress = ProgressManager.Instance.GetLevelProgress(i);
        progress = progress > 1 ? 1 : progress;
        _frameContentWidth = initialFrameWidth * progress;
        //mProgressContent.rectTransform.anchoredPosition = new Vector2(_frameContentWidth / 2, 0);
        mProgressContent.rectTransform.sizeDelta = new Vector2(_frameContentWidth, initialFrameHeight);
        var oldPos = mCirc.rectTransform.anchoredPosition;
        mCirc.rectTransform.anchoredPosition = new Vector2(_frameContentWidth - 64f, oldPos.y);
        Debug.Log("GetLevelProgress i = " + i + "progress = " + progress);
        return progress * 100;
    }
}
