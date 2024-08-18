using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum WindowType
{
    LevelSelectPage,
    MainPage,
    ConfigPage,
    PausePage
}


public class WindowManager : Singleton<WindowManager>
{
    private Dictionary<WindowType, string> _type2ResourceFileNameDict;
    private Transform uiRootTransform;
    private Stack<GameObject> _uiStack;
    private Transform _UIRoot;
    private bool _pausable;
    
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
        _type2ResourceFileNameDict[WindowType.PausePage] = "PausePage";

        var sceneName = SceneManager.GetActiveScene().name;
        _pausable = false;
        if (sceneName.Contains("Level"))
        {
            _pausable = true;
        }
        else if (sceneName.Contains("GUI"))
        {
            //Init Main Page
            InitWindow(WindowType.MainPage, _UIRoot);
        }
    }

    private void OnEscapeDown(EventData ed)
    {
        if (_pausable)//_pausable只用来提示当前场景是否允许显示暂停页面(level场景允许)，不要用于动态检测
        {
            if (_uiStack.Peek().gameObject.name.Contains("pause_page"))
            {
                
            }
            else
            {
                InitWindow(WindowType.PausePage, _UIRoot);
            }
        }
    }
    

    private void OnEnable()
    {
        EventManager.AddListener(EventType.EscapeDownEvent, OnEscapeDown);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.EscapeDownEvent, OnEscapeDown);
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
