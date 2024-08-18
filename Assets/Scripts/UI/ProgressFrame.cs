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
    [SerializeField] private int[] levelProgress;
    private float initialFrameWidth;
    private float initialFrameHeight;
    
    
    void Start()
    {
        //levelProgress = new int[4];
        mProgressText.text = GetCurrentLevelProgressText(1);
        
        
        initialFrameWidth = mProgressContent.rectTransform.sizeDelta.x;
        initialFrameHeight = mProgressContent.rectTransform.sizeDelta.y;
        
        UpdateProgressContent(1);
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.SwitchLevelEvent, OnSwitchLevel);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.SwitchLevelEvent, OnSwitchLevel);
    }

    private string GetCurrentLevelProgressText(int i)
    {
        if (i == 0)
        {
            return "#ERROR";
        }
        StringBuilder sb = new StringBuilder();
        sb.Append(levelProgress[i].ToString());
        sb.Append("%");
        return sb.ToString();
    }

    private void OnSwitchLevel(EventData ed)
    {
        var intoLevel = ((SwitchLevelEventData)ed).switchingIntoLevel;
        mProgressText.text = GetCurrentLevelProgressText(intoLevel);
        UpdateProgressContent(intoLevel);
    }

    private void UpdateProgressContent(int i)
    {
        var bestFitWidth = initialFrameWidth * levelProgress[i] / 100;
        mProgressContent.rectTransform.anchoredPosition = new Vector2(bestFitWidth / 2, 0);
        mProgressContent.rectTransform.sizeDelta = new Vector2(bestFitWidth, initialFrameHeight);
    }
    
}
