using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPage : Window
{
    
    protected override void onExit()
    {
        base.onExit();
        Debug.Log("MainPage.onExit was called");
    }
}
