using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{   
    // 鼠标左键是否按下
    private bool isMouseLeftPressing = false;
    // 鼠标右键是否按下
    private bool isMouseRightPressing = false;
    
    [SerializeField] private InputActionMap inputActionMap;
    private Dictionary<EventType, InputAction> inputActionDict = new Dictionary<EventType, InputAction>();
    private Dictionary<KeyCode, List<EventType>> releaseKeyBoardEventDict = new Dictionary<KeyCode, List<EventType>>();
    private Dictionary<MouseInput, List<EventType>> releaseMouseEventDict = new Dictionary<MouseInput, List<EventType>>();

    protected override void OnAwake()
    {
        InitInputActionDict();
    }

    void Update()
    {
        if (Input.anyKey)
        {
            foreach (var inputActionMap in inputActionDict)
            {
                EventType eventType = inputActionMap.Key;
                InputAction inputAction = inputActionMap.Value;
                bool isTrigger = false;
                if (inputAction.inputEventType == InputEventType.press)
                {
                    foreach (var keyCode in inputAction.keyBoardInput)
                    {
                        if (isTrigger || Input.GetKeyDown(keyCode))
                        {
                            isTrigger = true;
                            break;
                        }
                    }

                    foreach (var mouseInput in inputAction.mouseInput)
                    {
                        if (isTrigger)
                        {
                            break;
                        }
                        else if (mouseInput == MouseInput.LeftClick)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                isTrigger = true;
                                break;
                            }
                        }
                        else if (mouseInput == MouseInput.RightClick)
                        {
                            if (Input.GetMouseButtonDown(1))
                            {
                                isTrigger = true;
                                break;
                            }
                        }
                        else if (mouseInput == MouseInput.MiddleClick)
                        {
                            if (Input.GetMouseButtonDown(2))
                            {
                                isTrigger = true;
                                break;
                            }
                        }
                    }

                    if (isTrigger)
                    {
                        EventManager.InvokeEvent(eventType);
                    }
                }
                else if (inputAction.inputEventType == InputEventType.hold)
                {
                    foreach (var keyCode in inputAction.keyBoardInput)
                    {
                        if (isTrigger || Input.GetKey(keyCode))
                        {
                            isTrigger = true;
                            break;
                        }
                    }

                    foreach (var mouseInput in inputAction.mouseInput)
                    {
                        if (isTrigger)
                        {
                            break;
                        }
                        else if (mouseInput == MouseInput.LeftClick)
                        {
                            if (Input.GetMouseButton(0))
                            {
                                isTrigger = true;
                                break;
                            }
                        }
                        else if (mouseInput == MouseInput.RightClick)
                        {
                            if (Input.GetMouseButton(1))
                            {
                                isTrigger = true;
                                break;
                            }
                        }
                        else if (mouseInput == MouseInput.MiddleClick)
                        {
                            if (Input.GetMouseButton(2))
                            {
                                isTrigger = true;
                                break;
                            }
                        }
                    }
                    
                    if (isTrigger)
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
            else
            {
                inputActionDict.Add(inputAction.eventType, new InputAction(inputAction));
            }
        }
    }
    
    [Button(ButtonSizes.Large, Name = "重载输入映射文件")]
    public void ReloadInputActionMap()
    {
        inputActionDict.Clear();
        releaseKeyBoardEventDict.Clear();
        releaseMouseEventDict.Clear();
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
