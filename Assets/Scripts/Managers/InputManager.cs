using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{   
    void Update()
    {
        // 鼠标移动事件
        Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (mouseMovement != Vector2.zero)
        {
            EventManager.InvokeEvent(EventType.OnMouseMove, new MouseMovementEventData(mouseMovement));
        }

        // 鼠标左键点击事件
        if (Input.GetMouseButtonDown(0))
        {
            EventManager.InvokeEvent(EventType.OnMouseLeftClick);
        }

        // 鼠标右键点击事件
        if (Input.GetMouseButtonDown(1))
        {
            EventManager.InvokeEvent(EventType.OnMouseRightClick);
        }

        // 鼠标中键点击事件
        if (Input.GetMouseButtonDown(2))
        {
            EventManager.InvokeEvent(EventType.OnMiddleClick);
        }

        // 水平输入事件
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0)
        {
            EventManager.InvokeEvent(EventType.OnHorizontalInput, new HorizontalInputEventData(horizontalInput));
        }

        // 空格键按下事件
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventManager.InvokeEvent(EventType.OnSpacebarDown);
        }
        
        // Esc键按下事件
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventManager.InvokeEvent(EventType.OnEscDown);
        }
    }
    
    public Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}

// 鼠标事件数据
public class MouseMovementEventData : EventData
{
    public Vector2 mouseMovement;
    public MouseMovementEventData(Vector2 mouseMovement)
    {
        this.mouseMovement = mouseMovement;
    }
}

// 水平输入事件数据
public class HorizontalInputEventData : EventData
{
    public float horizontalInput;
    public HorizontalInputEventData(float horizontalInput)
    {
        this.horizontalInput = horizontalInput;
    }
}
