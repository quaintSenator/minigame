using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigButton : MonoBehaviour
{
    public void OnClickConfig()
    {
        var _windowManager = WindowManager.Instance;
        _windowManager.InitWindow(WindowType.ConfigPage,_windowManager.UIroot());
    }
}
