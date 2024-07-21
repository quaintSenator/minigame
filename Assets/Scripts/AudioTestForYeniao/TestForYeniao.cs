using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForYeniao : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayTest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayTest()
    {
        //加载对应SoundBank
        AkBankManager.LoadBank("TestForYeniao", false, false);
        //触发播放事件
        AkSoundEngine.PostEvent("Test_RIP", gameObject);      
    }
}
