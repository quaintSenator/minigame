using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class MusicVisualization : MonoBehaviour
{
    //关卡Bank列表
    [SerializeField] 
    private string[] BankNames = { "LevelTest" };

    //关卡音乐Event列表
    [SerializeField] 
    private AK.Wwise.Event[] LevelMusicPlayEvent = { };

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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void PlayLevelMusic()
    {
        if(IfPlayMusicWhenStart)
        {
            if (!(LevelIndex < BankNames.Length)
                || !(LevelIndex < LevelMusicPlayEvent.Length))
            {
                Debug.LogWarning("The LevelIndex out of length of LevelMusicPlayEvent or LevelMusicPlayEvent");
                return;
            }
            AkBankManager.LoadBank(BankNames[LevelIndex], false, false);

            if (IfUseVisualization)
            {
                LevelMusicPlayEvent[LevelIndex].Post( gameObject, (uint)CallbackType, CallbackFunctionMarker);
            }
            else
            {
                LevelMusicPlayEvent[LevelIndex].Post(gameObject);
            }


        }
    }

    //播放声音的回调函数
    //目前打的Marker会在节拍点前提前一定时间，可视化需要计算出正确的位置
    private void CallbackFunctionMarker(object InCookies, AkCallbackType InCallbackType, object InInfo)
    {
        if (InCallbackType == AkCallbackType.AK_Marker)
        {
            var MarkerInfo = InInfo as AkMarkerCallbackInfo;
            if (MarkerInfo != null)
            {
                //获取Controller移动速度

                //计算位置

                //延迟播放播放动画
            }
        }
    }





}
