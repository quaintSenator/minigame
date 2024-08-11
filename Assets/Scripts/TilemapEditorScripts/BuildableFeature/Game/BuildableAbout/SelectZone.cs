using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SelectZone : MonoBehaviour
{
    [SerializeField] private RectTransform selectionBox;
    private Vector2 startPos;
    private Vector2 endPos;
    private bool isSelecting = false;
    
    private void OnEnable()
    {
        EventManager.AddListener(EventType.StartSelectZoneEvent, OnStartSelectZone);
        EventManager.AddListener(EventType.EndSelectZoneEvent, OnEndSelectZone);
        EventManager.AddListener(EventType.SetSelectZoneStartPosEvent, OnSetSelectZoneStartPos);
        EventManager.AddListener(EventType.SetSelectZoneEndPosEvent, OnSetSelectZoneEndPos);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.StartSelectZoneEvent, OnStartSelectZone);
        EventManager.RemoveListener(EventType.EndSelectZoneEvent, OnEndSelectZone);
        EventManager.RemoveListener(EventType.SetSelectZoneStartPosEvent, OnSetSelectZoneStartPos);
        EventManager.RemoveListener(EventType.SetSelectZoneEndPosEvent, OnSetSelectZoneEndPos);
    }

    private void OnStartSelectZone(EventData obj)
    {
        isSelecting = true;
    }

    private void OnEndSelectZone(EventData obj)
    {
        isSelecting = false;
        selectionBox.gameObject.SetActive(false);
    }

    private void OnSetSelectZoneStartPos(EventData obj)
    {
        if (isSelecting)
        {
            startPos = Input.mousePosition;
            selectionBox.gameObject.SetActive(true);
        }
    }

    private void OnSetSelectZoneEndPos(EventData obj)
    {
        if (isSelecting && selectionBox.gameObject.activeSelf)
        {
            endPos = Input.mousePosition;
            Vector3 startPosWorld = Camera.main.ScreenToWorldPoint(startPos);
            Vector3 endPosWorld = Camera.main.ScreenToWorldPoint(endPos);
            EventManager.InvokeEvent(EventType.CompleteSelectZoneEvent, new SelectZoneEventData(startPosWorld, endPosWorld));
            selectionBox.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 更新选择框
        if (isSelecting)
        {
            endPos = Input.mousePosition;
            UpdateSelectionBox();
        }
    }

    void UpdateSelectionBox()
    {
        Vector2 boxStart = startPos;
        Vector2 boxEnd = endPos;

        // 计算并设置选择框的位置和大小
        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        selectionBox.position = boxCenter;

        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));
        selectionBox.sizeDelta = boxSize;

        // 确保锚点在左上角
        selectionBox.pivot = new Vector2(0.5f, 0.5f);
    }
}

public class SelectZoneEventData : EventData
{
    public Vector3 startPos;
    public Vector3 endPos;

    public SelectZoneEventData(Vector3 startPos, Vector3 endPos)
    {
        this.startPos = startPos;
        this.endPos = endPos;
    }
}