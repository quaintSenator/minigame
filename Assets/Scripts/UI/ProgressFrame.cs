using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ProgressFrame : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Text mProgressText;
    [SerializeField] private Image mProgressContent;
    private float initialFrameWidth;
    private float initialFrameHeight;
    [SerializeField] private float _frameContentWidth;
    
    void Start()
    {
        mProgressText.text = GetCurrentLevelProgressText(1);
        var m_rect = gameObject.GetComponent<RectTransform>();
        initialFrameWidth = m_rect.sizeDelta.x;
        initialFrameHeight = m_rect.sizeDelta.y;
        UpdateProgressContent(1);
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
        _frameContentWidth = initialFrameWidth * progress;
        mProgressContent.rectTransform.anchoredPosition = new Vector2(_frameContentWidth / 2, 0);
        mProgressContent.rectTransform.sizeDelta = new Vector2(_frameContentWidth, initialFrameHeight);
        return progress * 100;
    }
}
