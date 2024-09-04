using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConfigPage : Window
{

    //configPage总控三个滑条，实际是三个滑条
    private float[] Vols;
    

    public new void Start()
    {
        Init();
    }
    
    
    public void Init()
    {
        base.Init();
        WindowManager.Instance.ReadPrefs();
    }
    protected override void onExit()
    {
        base.onExit();
        WindowManager.Instance.ReadPrefs();
    }
}
