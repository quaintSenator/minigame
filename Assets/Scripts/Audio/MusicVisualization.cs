using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using System.Drawing;

[System.Serializable]
struct RhythmVisualizationPerfabParameter
{


    public RhythmVisualizationPerfabParameter(
        float _StartTimeOffset,
        float _CircleMaxRadium,
        float _OuterCircleWidth,
        UnityEngine.Color _OuterCircleColor,
        UnityEngine.Color _GrowingCircleColor,
        UnityEngine.Color _NormalColor,
        UnityEngine.Color _PerfectColor, 
        float _BeatStartTime,
        float _NormalRangeStartTime,
        float _NormalRangeEndTime,
        float _PerfectRangeStartTime,
        float _PerfectRangeEndTime,
        float _BeatEndTime,
        float _IntervalTimeB2WBeats)
    {
        this.StartTimeOffset = _StartTimeOffset;
        this.CircleMaxRadium = _CircleMaxRadium;
        this.OuterCircleWidth = _OuterCircleWidth;
        this.OuterCircleColor = _OuterCircleColor;
        this.GrowingCircleColor = _GrowingCircleColor;
        this.NormalColor = _NormalColor;
        this.PerfectColor = _PerfectColor;

        this.BeatStartTime = _BeatStartTime;
        this.NormalRangeStartTime = _NormalRangeStartTime;
        this.NormalRangeEndTime = _NormalRangeEndTime;
        this.PerfectRangeStartTime = _PerfectRangeStartTime;
        this.PerfectRangeEndTime = _PerfectRangeEndTime;
        this.BeatEndTime = _BeatEndTime;
        this.IntervalTimeB2WBeats = _IntervalTimeB2WBeats;
    }

    public static RhythmVisualizationPerfabParameter DefaultParameter => 
        new RhythmVisualizationPerfabParameter(
            0.0f,
            0.48f, 
            0.02f,
            UnityEngine.Color.blue, 
            UnityEngine.Color.green,
            UnityEngine.Color.yellow,
            UnityEngine.Color.red,
            0.0f,
            0.4f,
            0.8f,
            0.5f,
            0.7f,
            1.2f,
            0.0f);

    public float StartTimeOffset;
    public float CircleMaxRadium;

    public float OuterCircleWidth;

    public UnityEngine.Color OuterCircleColor;

    public UnityEngine.Color GrowingCircleColor ;

    public UnityEngine.Color NormalColor;

    public UnityEngine.Color PerfectColor ;

    public float BeatStartTime;

    public float NormalRangeStartTime;

    public float NormalRangeEndTime;

    public float PerfectRangeStartTime ;

    public float PerfectRangeEndTime ;

    public float BeatEndTime ;

    public float IntervalTimeB2WBeats;



}

[System.Serializable]
struct LevelMusic
{
    public AK.Wwise.Event LevelMusicPlayEvent;
    public AK.Wwise.Event LevelMusicStopEvent;
    public AK.Wwise.Event LevelMusicPauseEvent;
    public AK.Wwise.Event LevelMusicResumeEvent;
}


public class MusicVisualization : MonoBehaviour
{
    [SerializeField]
    private PlayerController PlayerControllerInstance = null;

    //关卡Bank列表
    [SerializeField]
    private List<string> BankNames = new List<string> { "LevelTest" };

    //关卡音乐Event列表
    [SerializeField]
    private List<LevelMusic> LevelMusicEvents = new List<LevelMusic>() { };

    //关卡音乐Bpm列表
    [SerializeField]
    private List<float> LevelMusicBpmList = new List<float>() { 100 };

    //关卡序号
    [SerializeField]
    private int LevelIndex = 0;

    //回调类型
    [SerializeField]
    private AkCallbackType CallbackType = AkCallbackType.AK_Marker;

    //是否自动在关卡开始时播放背景音乐
    [SerializeField]
    private bool IfPlayMusicWhenStart = true;

    //节奏可视化开关
    [SerializeField]
    private bool IfUseVisualization = true;

    [SerializeField]
    private GameObject RhythmVisualizationPerfabType = null;

    [SerializeField]
    [Header("RhythmVisualization")]
    int NumberOfRhythmVisualizationPerfabInstance = 5;

    [SerializeField]
    private List<GameObject> RhythmVisualizationPerfabInstanceList = new List<GameObject>();

    private bool IfInitRhythmVisualizationList = false;

    private bool IfInitRhythmVisualizationPosition = false;

    [SerializeField]
    private RhythmVisualizationPerfabParameter RhythmVisualizationPerfabParameterInstance = RhythmVisualizationPerfabParameter.DefaultParameter;

    private float PlayerSpeed = 0;

    [SerializeField]
    private float BeatTimeInterval = 0.6f;

    [SerializeField]
    private Vector3 RhythmVisualzationStartPositionWithPlayerController = new Vector3(100,100.0f,0);

    private Vector3 RhythmVisualzationPosition = Vector3.zero;

    [SerializeField]
    private float NextVisualPositionXOffset = 0.0f;

/*    [SerializeField]
    private float VisualPositionYOffset = 10f;*/

    private int IndexOfLastRhythmVisualzationInstance = 0;



    // Start is called before the first frame update
    void Start()
    {
        if (IfPlayMusicWhenStart)
        {
            PlayLevelMusic();
            EventManager.InvokeEvent(EventType.MusicStartEvent);

            if (IfUseVisualization)
            {
                GetPlayerControllerSpeed();
                InitRhythmVisualizationPerfabList();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void InitRhythmVisualizationPerfabList()
    {
        if (RhythmVisualizationPerfabType != null && IfUseVisualization)
        {
            for (int IndexOfPerfab = 0; IndexOfPerfab < NumberOfRhythmVisualizationPerfabInstance; IndexOfPerfab++)
            {
                GameObject InstanceOfRhythmVisualizationPerfab = GameObject.Instantiate(RhythmVisualizationPerfabType);

                Renderer RendererInstance = InstanceOfRhythmVisualizationPerfab.GetComponent<Renderer>();

                if (RendererInstance != null)
                {
                    InitSingleRhythmVisualizationPerfabParameter(RendererInstance);
                    RendererInstance.enabled = false;
                }

                RhythmVisualizationPerfabInstanceList.Add(InstanceOfRhythmVisualizationPerfab);
            }
            IfInitRhythmVisualizationList = true;
        }
    }

    private void GetPlayerControllerSpeed()
    {
        if (PlayerControllerInstance != null)
        {
            PlayerSpeed = PlayerControllerInstance.GetSpeed();
            return;
        }
        else
        {
            Debug.LogWarning("Can not get the PlayerInstance!");
            return;
        }
    }


    private void InitSingleRhythmVisualizationPerfabParameter(Renderer RendererInstance)
    {
        Material MaterialInstance = RendererInstance.material;
        RhythmVisualizationPerfabParameterInstance.StartTimeOffset= Time.realtimeSinceStartup;
        if (MaterialInstance != null)
        {
            MaterialInstance.SetFloat("_StartTimeOffset", RhythmVisualizationPerfabParameterInstance.StartTimeOffset);
            MaterialInstance.SetFloat("_CircleMaxRadium", RhythmVisualizationPerfabParameterInstance.CircleMaxRadium);
            MaterialInstance.SetFloat("_OuterCircleWidth", RhythmVisualizationPerfabParameterInstance.OuterCircleWidth);

            MaterialInstance.SetColor("_GrowingCircleColor", RhythmVisualizationPerfabParameterInstance.GrowingCircleColor);
            MaterialInstance.SetColor("_NormalColor", RhythmVisualizationPerfabParameterInstance.NormalColor);
            MaterialInstance.SetColor("_PerfectColor", RhythmVisualizationPerfabParameterInstance.PerfectColor);

            MaterialInstance.SetFloat("_BeatStartTime", RhythmVisualizationPerfabParameterInstance.BeatStartTime);
            MaterialInstance.SetFloat("_BeatEndTime", RhythmVisualizationPerfabParameterInstance.BeatEndTime);

            MaterialInstance.SetFloat("_NormalRangeStartTime", RhythmVisualizationPerfabParameterInstance.NormalRangeStartTime);
            MaterialInstance.SetFloat("_NormalRangeEndTime", RhythmVisualizationPerfabParameterInstance.NormalRangeEndTime);

            MaterialInstance.SetFloat("_PerfectRangeStartTime", RhythmVisualizationPerfabParameterInstance.PerfectRangeStartTime);
            MaterialInstance.SetFloat("_PerfectRangeEndTime", RhythmVisualizationPerfabParameterInstance.PerfectRangeEndTime);

            MaterialInstance.SetFloat("_IntervalTimeB2WBeats", RhythmVisualizationPerfabParameterInstance.IntervalTimeB2WBeats);
        }
    }


    public void PlayLevelMusic()
    {
        //TODO:补充延时播放相关
        if (IfPlayMusicWhenStart)
        {
            if (!(LevelIndex < BankNames.Count)
                || !(LevelIndex < LevelMusicEvents.Count))
            {
                Debug.LogWarning("The LevelIndex out of length of LevelMusicPlayEvent or LevelMusicPlayEvent");
                return;
            }
            AkBankManager.LoadBank(BankNames[LevelIndex], false, false);

            if (IfUseVisualization )
            {
                LevelMusicEvents[LevelIndex].LevelMusicPlayEvent.Post(gameObject, (uint)CallbackType, CallbackFunctionMarker);
            }
            else
            {
                LevelMusicEvents[LevelIndex].LevelMusicPlayEvent.Post(gameObject);
            }


        }
    }


    public void StopLevelMusic()
    {
        if ((LevelIndex < BankNames.Count) && (LevelIndex < LevelMusicEvents.Count))
        {
            LevelMusicEvents[LevelIndex].LevelMusicStopEvent.Post(gameObject);
        }
    }

    public void PauseLevelMusic()
    {
        if ((LevelIndex < BankNames.Count) && (LevelIndex < LevelMusicEvents.Count))
        {
            LevelMusicEvents[LevelIndex].LevelMusicPauseEvent.Post(gameObject);
        }
    }

    public void ResumeLevelMusic()
    {
        if ((LevelIndex < BankNames.Count) && (LevelIndex < LevelMusicEvents.Count))
        {
            LevelMusicEvents[LevelIndex].LevelMusicResumeEvent.Post(gameObject);
        }
    }


    public void SeekLevelMusicByTimeMS(int timeMS)
    {
        if ((LevelIndex < BankNames.Count) && (LevelIndex < LevelMusicEvents.Count))
        {
            LevelMusicEvents[LevelIndex].LevelMusicPlayEvent.SeekEventByTime(gameObject, timeMS);
        }
    }


    //播放声音的回调函数
    //目前打的Marker会在节拍点前提前一定时间，可视化需要计算出正确的位置
    private void CallbackFunctionMarker(object InCookies, AkCallbackType InCallbackType, object InInfo)
    {
        EventManager.InvokeEvent(EventType.MusicRecordEvent);
        if (InCallbackType == AkCallbackType.AK_Marker  && IfUseVisualization)
        {
            var MarkerInfo = InInfo as AkMarkerCallbackInfo;
            if (MarkerInfo != null)
            {
                if (!IfInitRhythmVisualizationPosition)
                {
                    InitRhythmVisualizationPerfabInstanceListPosition();
                }
                else
                {
                    UpdateRhythmVisualizationPerfabInstanceListLocation();
                }
            }
        }
    }

    private void ComputeRhythmVisualizationPerfabNextVisualPositionXOffset()
    {
        NextVisualPositionXOffset= (Vector3.right * PlayerSpeed * BeatTimeInterval).x;
        return ;
    }


    private void InitRhythmVisualizationPerfabInstanceListPosition()
    {
        if (PlayerControllerInstance == null)
        {
            return;
        }
        ComputeRhythmVisualizationPerfabNextVisualPositionXOffset();


        Vector3 PlayerControllerPosition = PlayerControllerInstance.transform.position;

        for (int IndexOfRhythmVisualzationInstance = 0; IndexOfRhythmVisualzationInstance < RhythmVisualizationPerfabInstanceList.Count; IndexOfRhythmVisualzationInstance++)
        {
            if (IndexOfRhythmVisualzationInstance == 0)
            {
                //TODO.目前使用玩家的初始位置
                RhythmVisualizationPerfabInstanceList[IndexOfRhythmVisualzationInstance].transform.position = 
                    PlayerControllerPosition + 
                    RhythmVisualzationStartPositionWithPlayerController;
            }
            else
            {
                RhythmVisualizationPerfabInstanceList[IndexOfRhythmVisualzationInstance].transform.position =
                    RhythmVisualizationPerfabInstanceList[IndexOfRhythmVisualzationInstance - 1].transform.position
                    + new Vector3(NextVisualPositionXOffset, 0, 0);
            }

            Renderer RendererInstance = RhythmVisualizationPerfabInstanceList[IndexOfRhythmVisualzationInstance].GetComponent<Renderer>();

            if (RendererInstance != null)
            {
                //InitSingleRhythmVisualizationPerfabParameter(RendererInstance);
                RendererInstance.enabled = true;
            }

        }
        IndexOfLastRhythmVisualzationInstance = 0;
        IfInitRhythmVisualizationPosition = true;

    }
    private void UpdateRhythmVisualizationPerfabInstanceListLocation()
    {
        int CountOfRhythmVisualizationPerfabInstanceList = RhythmVisualizationPerfabInstanceList.Count;
        if (CountOfRhythmVisualizationPerfabInstanceList == 0 )
        {
            return;
        }
        if(IndexOfLastRhythmVisualzationInstance>= CountOfRhythmVisualizationPerfabInstanceList)
        {
            Debug.LogWarning("Index out of the RhythmVisualizationPerfabInstanceList range");
            return;
        }

        RhythmVisualizationPerfabInstanceList[IndexOfLastRhythmVisualzationInstance].transform.position
            += new Vector3(NextVisualPositionXOffset * CountOfRhythmVisualizationPerfabInstanceList, 0, 0);

        IndexOfLastRhythmVisualzationInstance +=1;

        IndexOfLastRhythmVisualzationInstance %= CountOfRhythmVisualizationPerfabInstanceList;


    }





}
