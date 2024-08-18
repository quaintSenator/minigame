using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

public class DragItem : MonoBehaviour, IDragHandler, IPointerUpHandler
{
    private Vector2 _pointDownPosition;
    private Vector2 _draggedPosition;
    private RectTransform _mRectTransform;
    [SerializeField] private RectTransform mSlotTransform;
    private float _slotL;
    private float _slotR;

    public void Start()
    {
        _mRectTransform = gameObject.GetComponent<RectTransform>();
        if (mSlotTransform)
        {
            _slotL = mSlotTransform.anchoredPosition.x;
            _slotR = _slotL + mSlotTransform.sizeDelta.x - _mRectTransform.sizeDelta.x / 2;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        var newX = eventData.position.x;
        newX = newX > _slotR ? _slotR : newX;
        newX = newX < _slotL ? _slotL : newX;
        var oldY = _mRectTransform.anchoredPosition.y;
        _mRectTransform.anchoredPosition = new Vector2(newX, oldY);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var currentPercent = eventData.position.x - _slotL;
        currentPercent = currentPercent / (_slotR - _slotL);
        //接口：根据此百分比发送音量
    }
}
