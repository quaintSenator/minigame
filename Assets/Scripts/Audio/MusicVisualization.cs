using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class MusicVisualization : MonoBehaviour
{
    //�ؿ�Bank�б�
    [SerializeField] 
    private string[] BankNames = { "LevelTest" };

    //�ؿ�����Event�б�
/*    [SerializeField] 
    private Event[] LevelMusicPlayEvent = { };*/

    //�ؿ����
    [SerializeField] 
    private int LevelIndex = 0;

    //�ص�����
    [SerializeField]
    private AkCallbackType CallbackType = AkCallbackType.AK_Marker;

    //�Ƿ��Զ��ڹؿ���ʼʱ���ű�������
    [SerializeField] 
    private bool IfPlayMusicWhenStart = true;

    //������ӻ�����
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
