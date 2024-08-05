using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public WindowManager _WindowManager;

    public void Init()
    {
        Debug.Log("Base.Init() was called");
        _WindowManager = WindowManager.Instance;
        //exit逻辑 - 检查并协助绑定关闭页面事件
        var windowExitBtn = gameObject.GetComponentInChildren<ExitButton>();
        
        if (windowExitBtn)
        {
            var btnComponent = windowExitBtn.gameObject.GetComponent<UnityEngine.UI.Button>();
            if (btnComponent)
            {
                btnComponent.onClick.AddListener(onExit);
                Debug.Log("onExit bind ok");
            }
            else
            {
                Debug.Log("btnComponent == null");
            }
        }
        else
        {
            Debug.Log("windowExitBtn == null");
        }
    }
    public void Start()
    {
        Init();
    }
    protected virtual void onExit()
    {
        Debug.Log("Window.onExit was called");
        _WindowManager.CloseWindow();
        Destroy(gameObject);
    }
}
