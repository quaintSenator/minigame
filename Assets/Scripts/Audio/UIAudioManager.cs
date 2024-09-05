using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioManager : Singleton<MusicManager>
{
    public string UIBankName="";
    public AK.Wwise.Event MainUIMusicPlayEvent=null;
    public AK.Wwise.Event MainUIMusicStopEvent = null;
    public AK.Wwise.Event MainUIMusicPauseEvent = null;
    public AK.Wwise.Event MainUIMusicResumeEvent = null;

    public AK.Wwise.Event PlayPausePage120BpmMusicEvent = null;
    public AK.Wwise.Event StopPausePage120BpmMusicEvent = null;

    public AK.Wwise.Event PlayPausePage160BpmMusicEvent = null;
    public AK.Wwise.Event StopPausePage160BpmMusicEvent = null;

    public AK.Wwise.Event PlayBtnClickAudioEvent = null;



    #region 外部音乐接口

    //播放按钮点击音效
    public void PlayBtnClickAudio()
    {

        PlayBtnClickAudioEvent.Post(gameObject);

    }

    //开始UI背景音乐
    public void PlayMainUIMusic()
    {
        MainUIMusicPlayEvent.Post(gameObject);
    }

    //停止UI背景音乐
    public void StopMainUIMusic()
    {
        MainUIMusicStopEvent.Post(gameObject);
    }


    //所有弹窗出来都播这个，弹窗消失播下面这个
    public void PlayUIPausePageMusic()
    {
        int currentLevelIndex = ProgressManager.Instance.GetCurrentLevelIndex();
        //针对第三关播放的160Bpm的
        if (currentLevelIndex != 3)
        {
            PlayPausePage120BpmMusicEvent.Post(gameObject);
        }
        else
        {
            PlayPausePage160BpmMusicEvent.Post(gameObject);
        }


    }

    public void StopUIPausePageMusic()
    {
        int currentLevelIndex = ProgressManager.Instance.GetCurrentLevelIndex();
        //针对第三关播放的160Bpm的
        if (currentLevelIndex != 3)
        {
            StopPausePage120BpmMusicEvent.Post(gameObject);
        }
        else
        {
            StopPausePage160BpmMusicEvent.Post(gameObject);
        }
    }

    /*    //暂停UI背景音乐不一定有用
        public void PauseMainUIMusic()
        {
            MainUIMusicPauseEvent.Post(gameObject);
        }

        //恢复UI背景音乐 不一定有用
        public void ResumeMainUIMusic()
        {
            MainUIMusicResumeEvent.Post(gameObject);
        }*/





#endregion


    // Start is called before the first frame update
    void Start()
    {

    }
    //ButtonClickedEvent
    // Update is called once per frame
    void Update()
    {

    }


    private void OnEnable()
    {
        RegisterEvents();

    }

    private void OnDisable()
    {
        UngisterEvents();
    }

    private void RegisterEvents()
    {

    }

    private void UngisterEvents()
    {
    }


    protected override void OnAwake()
    {
        LoadUIBank();
    }

    protected override bool NeedDestory()
    {
        return false;
    }

    private void LoadUIBank()
    {
        AkBankManager.LoadBank(UIBankName, false, false);
    }

    
}
