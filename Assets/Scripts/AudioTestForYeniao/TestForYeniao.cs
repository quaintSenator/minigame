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
        //���ض�ӦSoundBank
        AkBankManager.LoadBank("TestForYeniao", false, false);
        //���������¼�
        AkSoundEngine.PostEvent("Test_RIP", gameObject);      
    }
}
