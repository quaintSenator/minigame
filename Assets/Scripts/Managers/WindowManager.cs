using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindowType
{
    
}
public class WindowManager : Singleton<WindowManager>
{
    private Dictionary<WindowType, string> _type2ResourceFileNameDict;
    public void InitWindow(WindowType windowType, Transform parent)
    {
        var gameObject2Create = Resources.Load(_type2ResourceFileNameDict[windowType]);
        Instantiate(gameObject2Create, parent);
    }
}
