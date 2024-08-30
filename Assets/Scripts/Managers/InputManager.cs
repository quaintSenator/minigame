using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InputManager : Singleton<InputManager>
{   
    // 鼠标左键是否按下
    private bool isMouseLeftPressing = false;
    // 鼠标右键是否按下
    private bool isMouseRightPressing = false;
    
    [SerializeField] private InputActionMap inputActionMap;
    private Dictionary<KeyCode, List<EventType>> pressKeyBoardEventDict = new Dictionary<KeyCode, List<EventType>>();
    private Dictionary<MouseInput, List<EventType>> pressMouseEventDict = new Dictionary<MouseInput, List<EventType>>();
    private Dictionary<KeyCode, List<EventType>> releaseKeyBoardEventDict = new Dictionary<KeyCode, List<EventType>>();
    private Dictionary<MouseInput, List<EventType>> releaseMouseEventDict = new Dictionary<MouseInput, List<EventType>>();
    private Dictionary<KeyCode,List<EventType>> holdKeyBoardEventDict = new Dictionary<KeyCode, List<EventType>>();
    private Dictionary<MouseInput,List<EventType>> holdMouseEventDict = new Dictionary<MouseInput, List<EventType>>();

    protected override void OnAwake()
    {
        InitInputActionDict();
        Debug.Log($"pressKeyBoardEventDict count : {pressKeyBoardEventDict.Count}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            BossController.InitBoss();
        }
        
        
        if (Input.anyKeyDown)
        {
            foreach (var pressEvent in pressKeyBoardEventDict) 
            {
                if (Input.GetKeyDown(pressEvent.Key))
                {
                    Debug.Log("pressEvent.Key " + pressEvent.Key);
                    foreach (var eventType in pressEvent.Value)
                    {
                        Debug.Log("eventType " + eventType);
                        EventManager.InvokeEvent(eventType);
                    }
                }
            }

            foreach (var pressEvent in pressMouseEventDict)
            {
                if (Input.GetMouseButtonDown((int)pressEvent.Key))
                {
                    if (SceneManager.GetActiveScene().name != "TilemapEditorScene" &&
                        (WindowManager.Instance.isAtPausePage() || WindowManager.Instance.isAtDeadPage()))
                    {
                        return;
                    }
                    foreach (var eventType in pressEvent.Value)
                    {
                        EventManager.InvokeEvent(eventType);
                    }
                }
            }
        }
        
        if (Input.anyKey)
        {
            foreach (var holdEvent in holdKeyBoardEventDict)
            {
                if (Input.GetKey(holdEvent.Key))
                {
                    foreach (var eventType in holdEvent.Value)
                    {
                        EventManager.InvokeEvent(eventType);
                    }
                }
            }

            foreach (var holdEvent in holdMouseEventDict)
            {
                if (Input.GetMouseButton((int)holdEvent.Key))
                {
                    foreach (var eventType in holdEvent.Value)
                    {
                        EventManager.InvokeEvent(eventType);
                    }
                }
            }
        }
        

        foreach (var releaseEvent in releaseKeyBoardEventDict)
        {
            if (Input.GetKeyUp(releaseEvent.Key))
            {
                foreach (var eventType in releaseEvent.Value)
                {
                    EventManager.InvokeEvent(eventType);
                }
            }
        }

        foreach (var releaseEvent in releaseMouseEventDict)
        {
            if (Input.GetMouseButtonUp((int)releaseEvent.Key))
            {
                foreach (var eventType in releaseEvent.Value)
                {
                    EventManager.InvokeEvent(eventType);
                }
            }
        }
            
        // 鼠标滚轮事件
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            EventManager.InvokeEvent(EventType.MiddleScrollEvent, new MiddleScrollEventData(scroll));
        }
        
        // 鼠标移动事件
        Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (mouseMovement != Vector2.zero)
        {
            EventManager.InvokeEvent(EventType.MouseMoveEvent, new MouseMovementEventData(mouseMovement));
        }// 鼠标左键
        if (Input.GetMouseButtonDown(0))
        {
            isMouseLeftPressing = true;
        }
        // 鼠标左键释放事件
        if (Input.GetMouseButtonUp(0))
        {
            isMouseLeftPressing = false;
        }

        // 鼠标右键
        if (Input.GetMouseButtonDown(1))
        {
            isMouseRightPressing = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isMouseRightPressing = false;
        }
    }
    
    public void InitInputActionDict()
    {
        foreach (var inputAction in inputActionMap.inputActions)
        {
            if (inputAction.inputEventType == InputEventType.release)
            {
                foreach (var keyCode in inputAction.keyBoardInput)
                {
                    if (releaseKeyBoardEventDict.ContainsKey(keyCode))
                    {
                        releaseKeyBoardEventDict[keyCode].Add(inputAction.eventType);
                    }
                    else
                    {
                        releaseKeyBoardEventDict.Add(keyCode, new List<EventType>() { inputAction.eventType });
                    }
                }
                foreach (var mouseInput in inputAction.mouseInput)
                {
                    if (releaseMouseEventDict.ContainsKey(mouseInput))
                    {
                        releaseMouseEventDict[mouseInput].Add(inputAction.eventType);
                    }
                    else
                    {
                        releaseMouseEventDict.Add(mouseInput, new List<EventType>() { inputAction.eventType });
                    }
                }
            }
            else if (inputAction.inputEventType == InputEventType.press)
            {
                foreach (var keyCode in inputAction.keyBoardInput)
                {
                    if (pressKeyBoardEventDict.ContainsKey(keyCode))
                    {
                        pressKeyBoardEventDict[keyCode].Add(inputAction.eventType);
                    }
                    else
                    {
                        pressKeyBoardEventDict.Add(keyCode, new List<EventType>() { inputAction.eventType });
                    }
                }
                foreach (var mouseInput in inputAction.mouseInput)
                {
                    if (pressMouseEventDict.ContainsKey(mouseInput))
                    {
                        pressMouseEventDict[mouseInput].Add(inputAction.eventType);
                    }
                    else
                    {
                        pressMouseEventDict.Add(mouseInput, new List<EventType>() { inputAction.eventType });
                    }
                }
            }
            else
            {
                foreach (var keyCode in inputAction.keyBoardInput)
                {
                    if (holdKeyBoardEventDict.ContainsKey(keyCode))
                    {
                        holdKeyBoardEventDict[keyCode].Add(inputAction.eventType);
                    }
                    else
                    {
                        holdKeyBoardEventDict.Add(keyCode, new List<EventType>() { inputAction.eventType });
                    }
                }
                foreach (var mouseInput in inputAction.mouseInput)
                {
                    if (holdMouseEventDict.ContainsKey(mouseInput))
                    {
                        holdMouseEventDict[mouseInput].Add(inputAction.eventType);
                    }
                    else
                    {
                        holdMouseEventDict.Add(mouseInput, new List<EventType>() { inputAction.eventType });
                    }
                }
            }
        }
    }
    
    [Button(ButtonSizes.Large, Name = "重载输入映射文件")]
    public void ReloadInputActionMap()
    {
        releaseKeyBoardEventDict.Clear();
        releaseMouseEventDict.Clear();
        pressKeyBoardEventDict.Clear();
        pressMouseEventDict.Clear();
        holdKeyBoardEventDict.Clear();
        holdMouseEventDict.Clear();
        InitInputActionDict();
    }
    
    public Vector3 GetMouseWolrdPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    
    public Ray RaycastMouseRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
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

    protected override bool NeedDestory()
    {
        return true;
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

// 滚轮事件数据
public class MiddleScrollEventData : EventData
{
    public float scroll;
    public MiddleScrollEventData(float scroll)
    {
        this.scroll = scroll;
    }
}

// 数字键按下事件数据
public class NumDownEventData : EventData
{
    public int num;
    public NumDownEventData(int num)
    {
        this.num = num;
    }
}
