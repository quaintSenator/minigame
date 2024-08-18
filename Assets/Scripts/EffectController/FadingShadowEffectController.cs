using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FadingShadowEffectController : MonoBehaviour
{
    [SerializeField] public Transform stillParent;
    [SerializeField] public float FadingShadowLifetime;
    [SerializeField] public string frameBuildString;
    private FrameSerial _shadowSpawnFrameSerial;
    [SerializeField] private GameObject _fadingShadowPrefab;

    
    
    private Dictionary<double, GameObject> dictTimerID2GOref;
    // Start is called before the first frame update
    void Start()
    {
        dictTimerID2GOref = new Dictionary<double, GameObject>();
        _shadowSpawnFrameSerial = new FrameSerial(frameBuildString, spawnSnapShot);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _shadowSpawnFrameSerial.CallBySerial();
        }
    }

    public void spawnSnapShot(EventData eventData)
    {
        var spawnResult = PoolManager.Instance.SpawnFromPool("BoxMeFadingShadowCopy",_fadingShadowPrefab, null);
        
        var absoluteTimeID = CleverTimerManager.Ask4Timer(FadingShadowLifetime, eventData =>
        {
            //回忆ref，并交还pool
            var timerData = (TimerDieEventData)eventData;
            var goRef2Return = dictTimerID2GOref[timerData.absoluteTime];
            PoolManager.Instance.ReturnToPool("BoxMeFadingShadowCopy", null);
        });
        
        dictTimerID2GOref[absoluteTimeID] = spawnResult;
    }
}
