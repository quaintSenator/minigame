using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindowType
{
    LevelSelectPage,
    MainPage,
    ConfigPage
}


public class WindowManager : Singleton<WindowManager>
{
    private Dictionary<WindowType, string> _type2ResourceFileNameDict;
    private Transform uiRootTransform;
    private Stack<GameObject> _uiStack;
    private Transform _UIRoot;
    protected override void OnAwake()
    {
        //Get UIRoot
        if (_UIRoot == null)
        {
            var canvasGO = GameObject.Find("Canvas");
            _UIRoot = canvasGO.transform;
            if (_UIRoot == null)
            {
                Debug.LogWarning("UIROOT is null");
            }
        }
        _uiStack = new Stack<GameObject>();
        
        _type2ResourceFileNameDict = new Dictionary<WindowType, string>();
        _type2ResourceFileNameDict[WindowType.LevelSelectPage] = "LevelSelectPage";
        _type2ResourceFileNameDict[WindowType.MainPage] = "MainPage";
        _type2ResourceFileNameDict[WindowType.ConfigPage] = "ConfigPage";
        
        //Init Main Page
        InitWindow(WindowType.MainPage, _UIRoot);
        
    }

    public void InitWindow(WindowType windowType, Transform parent)
    {
        string windowPrefabName = _type2ResourceFileNameDict[windowType];
        var gameObject2Create = Resources.Load(windowPrefabName) as GameObject;
        var generatedObjectRef = Instantiate(gameObject2Create, parent);
        
        if (_uiStack.Count > 0)
        {
            _uiStack.Peek().SetActive(false);
        }
        _uiStack.Push(generatedObjectRef);
    }

    public Transform UIroot()
    {
        return _UIRoot;
    }
    public void CloseWindow()
    {
        _uiStack.Pop();
        _uiStack.Peek().SetActive(true);
    }
}
