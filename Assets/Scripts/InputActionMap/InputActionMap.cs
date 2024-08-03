using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu(fileName = "InputActionMap", menuName = "InputActionMap")]
public class InputActionMap : ScriptableObject
{
    public List<InputAction> inputActions;
}

[Serializable]
public class InputAction
{
    [Space(10)]
    public EventType eventType;
    public InputEventType inputEventType;
    public List<KeyCode> keyBoardInput;
    public List<MouseInput> mouseInput;
    
    
    public InputAction()
    {
        keyBoardInput = new List<KeyCode>();
        mouseInput = new List<MouseInput>();
    }
    
    public InputAction(EventType eventType, InputEventType inputEventType, List<KeyCode> keyBoardInput, List<MouseInput> mouseInput)
    {
        this.eventType = eventType;
        this.inputEventType = inputEventType;
        this.keyBoardInput = keyBoardInput;
        this.mouseInput = mouseInput;
    }
    
    public InputAction(InputAction inputAction)
    {
        eventType = inputAction.eventType;
        inputEventType = inputAction.inputEventType;
        keyBoardInput = new List<KeyCode>(inputAction.keyBoardInput);
        mouseInput = new List<MouseInput>(inputAction.mouseInput);
    }
}

public enum InputEventType
{
    press,
    hold,
    release,
}

public enum MouseInput
{
    LeftClick,
    RightClick,
    MiddleClick,
}
