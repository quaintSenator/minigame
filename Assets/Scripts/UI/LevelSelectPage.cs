using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectPage : Window
{
    public new void Start()
    {
        base.Init();
    }
    protected override void onExit()
    {
        base.onExit();
        Debug.Log("LevelSelectPage.onExit was called");
    }
}
