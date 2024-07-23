using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForYeniao : MonoBehaviour
{

    public string TestBankName = "TestForYeniao";

    public string TestEventName = "Test_Drum_Bpm100";

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
        //���ض�ӦSoundBank
        AkBankManager.LoadBank(TestBankName, false, false);
        //���������¼�
        AkSoundEngine.PostEvent(TestEventName, gameObject);      
    }
}
