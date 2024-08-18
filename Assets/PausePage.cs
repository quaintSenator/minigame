using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePage : Window
{
    // Start is called before the first frame update
    public new void Start()
    {
        base.Init();
    }

    // Update is called once per frame
    protected override void onExit()
    {
        base.onExit();
        Debug.Log("PausePage.onExit was called");
    }
}
