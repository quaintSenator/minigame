using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

public class DragItem : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector2 _pointDownPosition;
    private Vector2 _draggedPosition;
    private RectTransform _mRectTransform;
    [SerializeField] private RectTransform mSlotTransform;
    private float slotWidth;
    private float slotLeftX;
    private float allowedmin;
    private float allowedmax;
    [SerializeField] private ConfigPage _mConfigPage;
    public void Start()
    {
        _mRectTransform = gameObject.GetComponent<RectTransform>();
        slotWidth = mSlotTransform.sizeDelta.x;
        slotLeftX = mSlotTransform.anchoredPosition.x;
        var mPercent = _mConfigPage.GetMyPercent(gameObject.name);
        
        RepositionMyself(mPercent * slotWidth - slotWidth / 2);
    }

    public void RepositionMyself(float x)
    {
        var oldY = _mRectTransform.anchoredPosition.y;
        _mRectTransform.anchoredPosition = new Vector2(x, oldY);
    }
    public void OnDrag(PointerEventData eventData)
    {
        var newX = eventData.position.x;
        allowedmin = slotLeftX - slotWidth / 2;
        allowedmax = slotLeftX + slotWidth / 2;
        newX = newX < allowedmin ? allowedmin : newX;
        newX = newX > allowedmax ? allowedmax : newX;
        //newX是绝对位置
        RepositionMyself(newX - slotLeftX);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var currentPercent = eventData.position.x - allowedmin;
        currentPercent /= slotWidth;
        currentPercent = currentPercent < 0.0f ? 0.0f : currentPercent;
        currentPercent = currentPercent > 1.0f ? 1.0f : currentPercent;
        //接口：根据此百分比发送音量
        _mConfigPage.TellConfigPageDragged(currentPercent, gameObject.name);
    }
}
