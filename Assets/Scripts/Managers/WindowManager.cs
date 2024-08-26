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
    PausePage,
    DeadPage
}


public class WindowManager : Singleton<WindowManager>
{
    private Dictionary<WindowType, string> _type2ResourceFileNameDict;
    private Transform uiRootTransform;
    private Stack<GameObject> _uiStack;
    [SerializeField]private Transform _UIRoot;
    private bool _pausable;
    
    protected override void OnAwake()
    {
        //Get UIRoot
        if (_UIRoot == null)
        {
            var canvasGO = GameObject.Find("Canvas");
            if (_UIRoot == null)
            {
                
            }
            else
            {
                _UIRoot = canvasGO.transform;
            }
        }
        _uiStack = new Stack<GameObject>();
        
        _type2ResourceFileNameDict = new Dictionary<WindowType, string>();
        _type2ResourceFileNameDict[WindowType.LevelSelectPage] = "LevelSelectPage";
        _type2ResourceFileNameDict[WindowType.MainPage] = "MainPage";
        _type2ResourceFileNameDict[WindowType.ConfigPage] = "ConfigPage";
        _type2ResourceFileNameDict[WindowType.PausePage] = "PausePage";
        _type2ResourceFileNameDict[WindowType.DeadPage] = "DeadPage";

        var sceneName = SceneManager.GetActiveScene().name;
        _pausable = false;
        if (sceneName.Contains("Level"))
        {
            _pausable = true;
        }
        else if (sceneName.Contains("GUI"))
        {
            ClipUIRoot2Empty();
            //Init Main Page
            InitWindow(WindowType.MainPage, _UIRoot);
        }
    }
    public void ClipUIRoot2Empty()//有时从游戏中返回GUIScene，可能有一些没有清理干净的页面
    {
        
        foreach (Transform child in _UIRoot.transform)
        {
            Debug.LogWarning("Deleting..." + child.gameObject.name);
            Destroy(child.gameObject);
        }
        _uiStack.Clear();
    }
    protected override bool NeedDestory()
    {
        return true;
    }

    private void OnEscapeDown(EventData ed)
    {
        if (_pausable)//_pausable只用来提示当前场景是否允许显示暂停页面(level场景允许)，不要用于动态检测
        {
            if (isAtPausePage())
            {
                //当已经打开了pause_page，那么调用resume_game,与回到游戏按钮等效
                ResumeGame();
            }
            else
            {
                //创建暂停页面
                InitWindow(WindowType.PausePage, _UIRoot);
                //游戏暂停
                EventManager.InvokeEvent(EventType.GamePauseEvent);
                Time.timeScale = 0;
            }
        }
    }

    public bool isAtPausePage()
    {
        if (_UIRoot == null)
        {
            return true;
        }
        return _uiStack.Count > 0 && _uiStack.Peek().gameObject.name.Contains("PausePage");
    }

    public bool isAtDeadPage()
    {
        if (_UIRoot == null)
        {
            return true;
        }
        return _uiStack.Count > 0 && _uiStack.Peek().gameObject.name.Contains("DeadPage");
    }
    public void CallDeadPage(EventData ed)
    {
        //
        //创建死亡页面
        Debug.Log("callDeadPage");
        InitWindow(WindowType.DeadPage, _UIRoot);
        //游戏暂停
        //EventManager.InvokeEvent(EventType.GamePauseEvent);
    }
    public void ResumeTimePause()
    {
        Time.timeScale = 1;
    }
    public void ResumeMusicStop()
    {
        MusicManager.Instance.ResumeLevelMusic();
    }
    public void ResumeGame()
    {
        if (isAtPausePage()) //如果目前暂停页面确实处于最上层
        {
            //恢复游戏
            //EventManager.InvokeEvent(EventType.StartLevelEvent);
            ResumeMusicStop();
            ResumeTimePause();
            
            //关闭暂停页面
            var pausePage = _uiStack.Peek().gameObject;
            _uiStack.Pop();
            Destroy(pausePage);
        }
    }
    private void OnEnable()
    {
        EventManager.AddListener(EventType.Ask4PauseEvent, OnEscapeDown);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.Ask4PauseEvent, OnEscapeDown);
    }

    public void OnStartPlayerDeadEvent(EventData eventData)
    {
        //玩家死亡并不一定会播放死亡弹窗
        var sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Contains("Level"))
        {
            
        }
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
        if (_uiStack.Count > 0)
        {
            _uiStack.Peek().SetActive(true);
        }
        
    }
}
