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
/*    [SerializeField] 
    private Event[] LevelMusicPlayEvent = { };*/

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
/*            if(!(LevelIndex< BankNames.Length) 
                || !(LevelIndex<LevelMusicPlayEvent.Length))
            {
                Debug.LogWarning("The LevelIndex out of length of LevelMusicPlayEvent or LevelMusicPlayEvent");
                return;
            }
            AkBankManager.LoadBank(BankNames[LevelIndex]);

            if (IfUseVisualization)
            {
                AkSoundEngine.PostEvent(LevelMusicPlayEvent[LevelIndex], gameObject,(uint)CallbackType, CallbackFunctionMarker);
            }
            else
            {
                AkSoundEngine.PostEvent(LevelMusicPlayEvent[LevelIndex], gameObject);
            }*/


        }
    }


    private void CallbackFunctionMarker(object InCookies, AkCallbackType in_type, object in_info)
    {

    }





}
