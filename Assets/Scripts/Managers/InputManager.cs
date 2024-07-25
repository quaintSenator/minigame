using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{   
    // 鼠标左键是否按下
    private bool isMouseLeftPressing = false;
    // 鼠标右键是否按下
    private bool isMouseRightPressing = false;
    
    void Update()
    {
        // 鼠标移动事件
        Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (mouseMovement != Vector2.zero)
        {
            EventManager.InvokeEvent(EventType.MouseMoveEvent, new MouseMovementEventData(mouseMovement));
        }

        // 鼠标左键点击事件
        if (Input.GetMouseButtonDown(0))
        {
            isMouseLeftPressing = true;
            EventManager.InvokeEvent(EventType.MouseLeftClickEvent);
        }
        
        // 鼠标左键释放事件
        if (Input.GetMouseButtonUp(0))
        {
            isMouseLeftPressing = false;
            EventManager.InvokeEvent(EventType.MouseLeftReleaseEvent);
        }

        // 鼠标右键点击事件
        if (Input.GetMouseButtonDown(1))
        {
            isMouseRightPressing = true;
            EventManager.InvokeEvent(EventType.MouseRightClickEvent);
        }
        
        // 鼠标右键释放事件
        if (Input.GetMouseButtonUp(1))
        {
            isMouseRightPressing = false;
            EventManager.InvokeEvent(EventType.MouseRightReleaseEvent);
        }

        // 鼠标中键点击事件
        if (Input.GetMouseButtonDown(2))
        {
            EventManager.InvokeEvent(EventType.MiddleClickEvent);
        }
        
        // 鼠标滚轮事件
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            EventManager.InvokeEvent(EventType.MiddleScrollEvent, new MiddleScrollEventData(scroll));
        }

        // 水平输入事件
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0)
        {
            EventManager.InvokeEvent(EventType.HorizontalInputEvent, new HorizontalInputEventData(horizontalInput));
        }

        // 空格键按下事件
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventManager.InvokeEvent(EventType.SpacebarDownEvent);
        }
        
        // Esc键按下事件
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventManager.InvokeEvent(EventType.EscDownEvent);
        }
        
        // E键按下事件
        if (Input.GetKeyDown(KeyCode.E))
        {
            EventManager.InvokeEvent(EventType.EDownEvent);
        }
        
        // K键按下事件
        if (Input.GetKeyDown(KeyCode.K))
        {
            EventManager.InvokeEvent(EventType.KDownEvent);
        }
        
        // L键按下事件
        if (Input.GetKeyDown(KeyCode.L))
        {
            EventManager.InvokeEvent(EventType.LDownEvent);
        }
    }
    
    public Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    
    public bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    
    public bool IsMouseLeftPressing()
    {
        return isMouseLeftPressing;
    }
    
    public bool IsMouseRightPressing()
    {
        return isMouseRightPressing;
    }
}

// 鼠标移动事件数据
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

// 滚轮事件数据
public class MiddleScrollEventData : EventData
{
    public float scroll;
    public MiddleScrollEventData(float scroll)
    {
        this.scroll = scroll;
    }
}
