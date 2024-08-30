using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum WindowType
{
    LevelSelectPage,
    MainPage,
    ConfigPage,
    PausePage,
    DeadPage,
    TipPage
}


public class WindowManager : Singleton<WindowManager>
{
    private Dictionary<WindowType, string> _type2ResourceFileNameDict;
    private Transform uiRootTransform;
    private Stack<GameObject> _uiStack;
    [SerializeField]private Transform _UIRoot;
    private bool _pausable;
    private static bool _tipPopAllowed;
    private bool _shouldGiveFirstRandomLine;
    
    protected override void OnAwake()
    {
        //Get UIRoot
        if (_UIRoot == null)
        {
            var canvasGO = GameObject.Find("Canvas");
            if (canvasGO == null)
            {
                var pref = Resources.Load("Canvas");
                var go = GameObject.Instantiate(pref, null) as GameObject;
                _UIRoot = go.transform;
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
        _type2ResourceFileNameDict[WindowType.TipPage] = "TipPage";

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

        _tipPopAllowed = true;
        _shouldGiveFirstRandomLine = true;
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
        if (generatedObjectRef.name.Contains("Select"))
        {
            _tipPopAllowed = true;
        }
    }
    
    public Transform UIroot()
    {
        return _UIRoot;
    }
    public void CloseWindow()
    {
        var page2Close = _uiStack.Pop();
        if (page2Close.name.Contains("Select"))
        {
            _tipPopAllowed = false;
        }
        if (_uiStack.Count > 0)
        {
            _uiStack.Peek().SetActive(true);
        }
    }

    public bool isPopAllowed()
    {
        return _tipPopAllowed;
    }

    public static bool SetPopAllowed(bool allow)
    {
        _tipPopAllowed = allow;
        return allow;
    }

    public void TryDestroyTip(int id)
    {
        var topPage = _uiStack.Peek().gameObject;
        if (topPage != null && topPage.name.Contains("Tip"))
        {
            var tipPage = topPage.GetComponent<TipPage>();
            if (tipPage && tipPage.GetID() == id)
            {
                CloseWindow();
                Destroy(topPage);
            }
        }
    }

    public string DecideTipLine(int levelNum)
    {
        string res = "";
        if (levelNum == 2)
        {
            res = _shouldGiveFirstRandomLine ? "尚未获得进入僧侣宫殿的能力" : "请先在前面学会飞跃的本领再来";
        }
        else  if(levelNum == 3)
        {
            res = _shouldGiveFirstRandomLine ? "你还没准备好吧？" : "僧侣的军队很疯狂，先去前面提升自己怎么样？";
        }
        _shouldGiveFirstRandomLine = !_shouldGiveFirstRandomLine;
        return res;
    }
    public void OpenTip(int clickedNum)//外部应用这个函数会频繁连点，这里要鉴定连点
    {
        if (_tipPopAllowed)
        {
            _tipPopAllowed = false;//锁0.3s
            CleverTimerManager.Ask4Timer(0.3f, eventData =>
            {
                WindowManager.SetPopAllowed(true);
            });
            var generatedID = (int)Time.time;
            var currentTop = _uiStack.Peek().gameObject;
            if (currentTop.name.Contains("Tip"))
            {
                //上个tip尚未消亡
                var topTipPage = currentTop.GetComponent<TipPage>();
                topTipPage.SetID(generatedID);
                topTipPage.SetText(DecideTipLine(clickedNum));
            }
            else
            {
                //新建tip
                var initTipPref = Resources.Load(_type2ResourceFileNameDict[WindowType.TipPage]);
                GameObject initedTip = Instantiate(initTipPref, _UIRoot) as GameObject;
                _uiStack.Push(initedTip);
                
                var tipPage = initedTip.GetComponent<TipPage>();
                if (tipPage)
                {
                    tipPage.SetText(DecideTipLine(clickedNum));
                    tipPage.SetID(generatedID);
                }
            }
            CleverTimerManager.Ask4Timer(2.0f, eventData =>
            {
                WindowManager.Instance.TryDestroyTip(generatedID);
            });
        }
    }
}
