using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TilemapCameraController : MonoBehaviour
{
    //虚拟摄像机
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    //移动方向
    private Vector3 moveDirection;
    //移动速度
    [SerializeField] private float moveSpeed = 50.0f;
    //缩放方向
    private float zoomDirection;
    //缩放速度
    [SerializeField] private float zoomSpeed = 5.0f;
    //原始缩放大小
    private float originalZoom;
    //最大缩放大小
    [SerializeField] private float maxZoom = 10.0f;
    //最小缩放大小
    [SerializeField] private float minZoom = 2.5f;

    private void Start()
    {
        originalZoom = virtualCamera.m_Lens.OrthographicSize;
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.MouseMoveEvent, OnMouseMove);
        EventManager.AddListener(EventType.MiddleClickEvent, OnMiddleClick);
        EventManager.AddListener(EventType.MiddleScrollEvent, OnMiddleScroll);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.MouseMoveEvent, OnMouseMove);
        EventManager.RemoveListener(EventType.MiddleClickEvent, OnMiddleClick);
        EventManager.RemoveListener(EventType.MiddleScrollEvent, OnMiddleScroll);
    }

    private void OnMiddleScroll(EventData data)
    {
        Debug.Log("OnMiddleScroll");
        var middleScrollData = data as MiddleScrollEventData;
        zoomDirection = middleScrollData.scroll;
        //如果摄像机缩放到达最大最小值，停止缩放
        if ((zoomDirection < 0 && virtualCamera.m_Lens.OrthographicSize >= maxZoom) || (zoomDirection > 0 && virtualCamera.m_Lens.OrthographicSize <= minZoom))
        {
            zoomDirection = 0;
        }
    }

    private void OnMiddleClick(EventData data)
    {
        Debug.Log("OnMiddleClick");
        virtualCamera.m_Lens.OrthographicSize = originalZoom;
    }

    private void OnMouseMove(EventData data)
    {
        if (InputManager.Instance.IsMouseRightPressing())
        {
            var mouseMovementData = data as MouseMovementEventData;
            float xDir = -mouseMovementData.mouseMovement.x;
            moveDirection = new Vector3(xDir * virtualCamera.m_Lens.OrthographicSize / originalZoom, 0, 0);
        }
        else
        {
            moveDirection = Vector3.zero;
        }
    }

    private void LateUpdate()
    {
        //移动摄像机
        transform.Translate(moveDirection * Time.deltaTime * moveSpeed);
        //缩放摄像机
        virtualCamera.m_Lens.OrthographicSize -= zoomDirection * zoomSpeed;
    }
}
