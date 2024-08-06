using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigPage : Window
{
    // Start is called before the first frame update
    public new void Start()
    {
        base.Init();
    }
    protected override void onExit()
    {
        base.onExit();
        Debug.Log("ConfigPage.onExit was called");
    }
}
