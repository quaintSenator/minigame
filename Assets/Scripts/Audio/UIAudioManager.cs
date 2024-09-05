using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;


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

    public RTPC MasterVolume = null;
    public RTPC LevelMusicVolume = null;
    public RTPC UIVolume = null;
    public RTPC InteractiveVolume = null;


    #region �ⲿ���ֽӿ�

    //���Ű�ť�����Ч
    public void PlayBtnClickAudio()
    {

        PlayBtnClickAudioEvent.Post(gameObject);

    }

    //��ʼUI��������
    public void PlayMainUIMusic()
    {
        MainUIMusicPlayEvent.Post(gameObject);
    }

    //ֹͣUI��������
    public void StopMainUIMusic()
    {
        MainUIMusicStopEvent.Post(gameObject);
    }


    //���е����������������������ʧ���������
    public void PlayUIPausePageMusic()
    {
        int currentLevelIndex = ProgressManager.Instance.GetCurrentLevelIndex();
        //��Ե����ز��ŵ�160Bpm��
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
        //��Ե����ز��ŵ�160Bpm��
        if (currentLevelIndex != 3)
        {
            StopPausePage120BpmMusicEvent.Post(gameObject);
        }
        else
        {
            StopPausePage160BpmMusicEvent.Post(gameObject);
        }
    }

    /*    //��ͣUI�������ֲ�һ������
        public void PauseMainUIMusic()
        {
            MainUIMusicPauseEvent.Post(gameObject);
        }

        //�ָ�UI�������� ��һ������
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
