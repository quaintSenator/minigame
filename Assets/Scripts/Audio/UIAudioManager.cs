using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using UnityEditor;
using Sirenix.OdinInspector;


public class UIAudioManager : Singleton<UIAudioManager>
{
    public string UIBankName = "";
    public AK.Wwise.Event MainUIMusicPlayEvent = null;
    public AK.Wwise.Event MainUIMusicFadePlayEvent = null;
    public AK.Wwise.Event MainUIMusicStopEvent = null;
    public AK.Wwise.Event MainUIMusicPauseEvent = null;
    public AK.Wwise.Event MainUIMusicResumeEvent = null;

    public AK.Wwise.Event PlayPausePage120BpmMusicEvent = null;
    public AK.Wwise.Event StopPausePage120BpmMusicEvent = null;

    public AK.Wwise.Event PlayPausePage160BpmMusicEvent = null;
    public AK.Wwise.Event StopPausePage160BpmMusicEvent = null;

    public AK.Wwise.Event PlayBtnClickAudioEvent = null;

    public RTPC MasterVolume = null;
    public RTPC LevelMusicVolume = null;
    public RTPC UIVolume = null;
    public RTPC InteractiveVolume = null;

    [SerializeField]
    private List<LevelMusic> LevelMusicEvents = null;
    private int lastPlayLevelMusicIndex = -1;
    private bool isPlayingLevelMusicBtn = false;

    private bool isPlayMainUIMusic = false;

    private bool bShouldPlayFadeMainUIMusic = true;

    #region 外部音乐接口

    //播放按钮点击音效
    public void PlayBtnClickAudio()
    {

        PlayBtnClickAudioEvent.Post(gameObject);

    }

    //开始UI背景音乐
    public void PlayMainUIMusic()
    {
        if (isPlayMainUIMusic || isPlayingLevelMusicBtn)
        {
            return;
        }
        if (bShouldPlayFadeMainUIMusic)
        {
            MainUIMusicFadePlayEvent.Post(gameObject);
        }
        else 
        {
            MainUIMusicPlayEvent.Post(gameObject);
        }
        isPlayMainUIMusic = true;


    }

    //停止UI背景音乐
    public void StopMainUIMusic()
    {
        MainUIMusicStopEvent.Post(gameObject);
        isPlayMainUIMusic = false;
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

    public void SetMasterVolume(float Volume)
    {
        MasterVolume.SetGlobalValue(Volume);
    }

    public void SetLevelMusicVolume(float Volume)
    {
        LevelMusicVolume.SetGlobalValue(Volume);
    }

    public void SetUIVolume(float Volume)
    {
        UIVolume.SetGlobalValue(Volume);
    }

    public void SetInteractiveVolume(float Volume)
    {
        InteractiveVolume.SetGlobalValue(Volume);
    }


    public void PlayOrStopLevelMusicByBtn(int levelMusicIndex)
    {
        AkCallbackType CallbackType = AkCallbackType.AK_EndOfEvent;
        if (levelMusicIndex <= 0 || levelMusicIndex > LevelMusicEvents.Count)
        {
            Debug.LogError("Wrong Level Index");
            return;
        }

        if (isPlayingLevelMusicBtn && levelMusicIndex == lastPlayLevelMusicIndex)
        {

            LevelMusicEvents[lastPlayLevelMusicIndex].LevelMusicStopEvent.Post(gameObject);
            lastPlayLevelMusicIndex = -1;
            bShouldPlayFadeMainUIMusic = true;
            PlayMainUIMusic();
            isPlayingLevelMusicBtn = false;

        }
        //if (isPlayingLevelMusicBtn && levelMusicIndex == lastPlayLevelMusicIndex)
        else if (isPlayingLevelMusicBtn)
        {
            LevelMusicEvents[lastPlayLevelMusicIndex].LevelMusicStopEvent.Post(gameObject);
            LevelMusicEvents[levelMusicIndex].LevelMusicPlayEvent.Post(gameObject, (uint)CallbackType, CallbackFunctionEndEvent);
            lastPlayLevelMusicIndex = levelMusicIndex;

        }
        else
        {
            StopMainUIMusic();

            LevelMusicEvents[levelMusicIndex].LevelMusicPlayEvent.Post(gameObject, (uint)CallbackType, CallbackFunctionEndEvent);

            lastPlayLevelMusicIndex = levelMusicIndex;
            isPlayingLevelMusicBtn = true;
        }

    }



    #endregion


    #region DebugTest
#if UNITY_EDITOR
    public int debugLevelIndex = 0;

    [Button]
    private void DebugTestPlayOrStopLevelMusicByBtn()
    {

        PlayOrStopLevelMusicByBtn(debugLevelIndex);
    }
#endif
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


    private void CallbackFunctionEndEvent(object InCookies, AkCallbackType InCallbackType, object InInfo)
    {
        /*        LevelMusicEvents[lastPlayLevelMusicIndex].LevelMusicStopEvent.Post(gameObject);
                lastPlayLevelMusicIndex = -1;
                isPlayingLevelMusicBtn = false;
                PlayMainUIMusic();*/

        LevelMusicEvents[lastPlayLevelMusicIndex].LevelMusicStopEvent.Post(gameObject);
        lastPlayLevelMusicIndex = -1;
        bShouldPlayFadeMainUIMusic = true;
        PlayMainUIMusic();
        isPlayingLevelMusicBtn = false;
    }
}
