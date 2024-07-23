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

    private void Update()
    {
        GetMoveDirection();
        GetZoomDirection();
    }

    //当玩家按住右键是左右移动CameraController，进而控制摄像机的移动，或者左右键移动
    public void GetMoveDirection()
    {
        //键盘移动系数，用于控制移动速度
        float moveFactor = 0.5f;
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            moveDirection = new Vector3(-x * virtualCamera.m_Lens.OrthographicSize / originalZoom, 0, 0);
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal") * moveFactor, 0, 0);
        }
        else
        {
            moveDirection = Vector3.zero;
        }
    }
    
    //当玩家滚动鼠标滚轮时，控制摄像机的缩放
    public void GetZoomDirection()
    {
        zoomDirection = Input.GetAxis("Mouse ScrollWheel");
        //如果点击了鼠标滚轮，恢复到原始大小
        if (Input.GetMouseButtonDown(2))
        {
            virtualCamera.m_Lens.OrthographicSize = originalZoom;
        }
        //如果摄像机缩放到达最大最小值，停止缩放
        if ((zoomDirection < 0 && virtualCamera.m_Lens.OrthographicSize >= maxZoom) || (zoomDirection > 0 && virtualCamera.m_Lens.OrthographicSize <= minZoom))
        {
            zoomDirection = 0;
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
