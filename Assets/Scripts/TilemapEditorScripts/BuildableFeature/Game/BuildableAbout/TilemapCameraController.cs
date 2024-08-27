using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class TilemapCameraController : Singleton<TilemapCameraController>
{
    //虚拟摄像机
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private CinemachineVirtualCamera virtualCamera2;
    
    private CinemachineVirtualCamera currentVirtualCamera;
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
    //原始高度
    private float originalHeight;
    //最大缩放大小
    [SerializeField] private float maxZoom = 10.0f;
    //最小缩放大小
    [SerializeField] private float minZoom = 2.5f;
    
    private Transform startPoint;

    private void Start()
    {
        startPoint = Utils.GetStartPointPostion();
        originalZoom = virtualCamera.m_Lens.OrthographicSize;
        originalHeight = transform.position.y;
        currentVirtualCamera = virtualCamera;
        virtualCamera.gameObject.SetActive(true);
        virtualCamera2.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.MouseMoveEvent, OnMouseMove);
        EventManager.AddListener(EventType.ResetCameraEvent, OnMiddleClick);
        EventManager.AddListener(EventType.MiddleScrollEvent, OnMiddleScroll);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.MouseMoveEvent, OnMouseMove);
        EventManager.RemoveListener(EventType.ResetCameraEvent, OnMiddleClick);
        EventManager.RemoveListener(EventType.MiddleScrollEvent, OnMiddleScroll);
    }

    private void OnMiddleScroll(EventData data)
    {
        var middleScrollData = data as MiddleScrollEventData;
        zoomDirection = middleScrollData.scroll;
        //如果摄像机缩放到达最大最小值，停止缩放
        if ((zoomDirection < 0 && currentVirtualCamera.m_Lens.OrthographicSize >= maxZoom) || (zoomDirection > 0 && currentVirtualCamera.m_Lens.OrthographicSize <= minZoom))
        {
            zoomDirection = 0;
        }
        //缩放摄像机
        currentVirtualCamera.m_Lens.OrthographicSize -= zoomDirection * zoomSpeed;
    }

    private void OnMiddleClick(EventData data)
    {
        currentVirtualCamera.m_Lens.OrthographicSize = originalZoom;
        transform.position = new Vector3(transform.position.x, originalHeight, transform.position.z);
    }

    private void OnMouseMove(EventData data)
    {
        if (InputManager.Instance.IsMouseRightPressing() && !RhythmViewer.CurrentMusicIsPlaying)
        {
            var mouseMovementData = data as MouseMovementEventData;
            Vector2 mouseMovement = mouseMovementData.mouseMovement;
            moveDirection = new Vector3(-mouseMovement.x * currentVirtualCamera.m_Lens.OrthographicSize / originalZoom, -mouseMovement.y * currentVirtualCamera.m_Lens.OrthographicSize / originalZoom, 0);
        }
        else
        {
            moveDirection = Vector3.zero;
        }
        if(moveDirection.magnitude < 0.14f)
        {
            moveDirection = Vector3.zero;
        }
    }

    [Button]
    public void ChangeCamera(bool fromBoss = false)
    {
        if (fromBoss)
        {
            virtualCamera.gameObject.SetActive(false);
            virtualCamera2.gameObject.SetActive(true);
            currentVirtualCamera = virtualCamera2;
            return;
        }
        if (currentVirtualCamera == virtualCamera)
        {
            virtualCamera.gameObject.SetActive(false);
            virtualCamera2.gameObject.SetActive(true);
            currentVirtualCamera = virtualCamera2;
        }
        else
        {
            virtualCamera.gameObject.SetActive(true);
            virtualCamera2.gameObject.SetActive(false);
            currentVirtualCamera = virtualCamera;
        }
    }

    private void LateUpdate()
    {
        //移动摄像机
        if(RhythmViewer.CurrentMusicIsPlaying)
        {
            transform.position = new Vector3(RhythmViewer.CurrentMusicTime * GameConsts.SPEED + startPoint.position.x, -1.21f, -1.05f);
        }
        transform.Translate(moveDirection * Time.deltaTime * moveSpeed);
    }
}
