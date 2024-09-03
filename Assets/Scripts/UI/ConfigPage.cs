using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConfigPage : Window
{

    //configPage总控三个滑条，实际是三个滑条
    private float[] Vols;
    public static Dictionary<string, int> ItemName2ID;

    public new void Start()
    {
        Init();
    }

    public void ReadPrefs()
    {
        var everChangedConfPref = PlayerPrefs.GetInt("everChangedConfPref", 0);
        if (everChangedConfPref == 0)
        {
            PlayerPrefs.SetFloat("effectVol", 0.5f);
            PlayerPrefs.SetFloat("musicVol", 0.5f);
            PlayerPrefs.SetFloat("totalVol", 0.5f);
            PlayerPrefs.SetInt("everChangedConfPref", 1);
        }
        Vols[0] = PlayerPrefs.GetFloat("effectVol");
        Vols[1] = PlayerPrefs.GetFloat("musicVol");
        Vols[2] = PlayerPrefs.GetFloat("totalVol");
    }

    public void WritePrefs()
    {
        PlayerPrefs.SetFloat("effectVol", Vols[0]);
        PlayerPrefs.SetFloat("musicVol", Vols[1]);
        PlayerPrefs.SetFloat("totalVol", Vols[2]);
    }
    public void Init()
    {
        base.Init();
        ItemName2ID = new Dictionary<string, int>();
        ItemName2ID.Add("drag_slide_totalVol_item", 0);
        ItemName2ID.Add("drag_slide_effectVol_item", 1);
        ItemName2ID.Add("drag_slide_bgVol_item", 2);
        Vols = new float[3];
        ReadPrefs();
    }

    public float GetMyPercent(string slideName)
    {
        if (ItemName2ID.ContainsKey(slideName))
        {
            return Vols[ItemName2ID[slideName]];
        }

        return 0.0f;
    }
    protected override void onExit()
    {
        base.onExit();
        ReadPrefs();
        Debug.Log(Vols[0]);
        Debug.Log(Vols[1]);
        Debug.Log(Vols[2]);
    }

    private int RecognizeChild(string childItemName)
    {
        if (childItemName.Contains("bgVol"))
        {
            return 1;
        }
        if (childItemName.Contains("effectVol"))
        {
            return 2;
        }
        if (childItemName.Contains("totalVol"))
        {
            return 3;
        }
        return 0;
    }
    public void TellConfigPageDragged(float percent, string dragItemName)
    {
        Debug.Log("itemName = " + dragItemName + " percent = " + percent);
        //更新Vols[]
        var itemId = RecognizeChild(dragItemName);
        Vols[itemId - 1] = percent;
        //写入prefs
        WritePrefs();
        //发送音量
        var emittingMusicVol = ProgressPercent2EmitParam(Vols[0] * Vols[2]);
        var emittingIntVol = ProgressPercent2EmitParam(Vols[1] * Vols[2]);
        EmitConfiguredVolumns(emittingMusicVol, emittingIntVol);
    }
    public float ProgressPercent2EmitParam(float percent)
    {
        return 200 * percent - 100f;
    }
    private void EmitConfiguredVolumns(float musicVol, float InteractVol)
    {
        Debug.Log("musicVol = " + musicVol);
        Debug.Log("InteractVol = " + InteractVol);
        MusicManager.Instance.SetLevelMusicVolume(musicVol);
        MusicManager.Instance.SetInteractiveVolume(InteractVol);
    }
    
}
