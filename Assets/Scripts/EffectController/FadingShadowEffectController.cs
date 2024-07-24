using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingShadowEffectController : MonoBehaviour
{
    [SerializeField] public Transform stillParent;

    [SerializeField] public float FadingShadowLifetime;

    [SerializeField] public string frameBuildString;
    private FrameSerial _shadowSpawnFrameSerial;

    private Dictionary<int, GameObject> dictTimerID2GOref;
    // Start is called before the first frame update
    void Start()
    {
        dictTimerID2GOref = new Dictionary<int, GameObject>();
        _shadowSpawnFrameSerial = new FrameSerial(frameBuildString, spawnSnapShot);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _shadowSpawnFrameSerial.callBySerial();
        }
    }

    public void spawnSnapShot(EventData eventData)
    {
        #region Step1::makeShadowStill
        var spawnResult = PoolManager.Instance().Spawn(PoolItemType.BoxMeFadingShadowCopy, transform);
        var position = transform.position;
        var rotation = transform.rotation;
        spawnResult.transform.SetParent(stillParent);
        spawnResult.transform.SetPositionAndRotation(position, rotation);
        #endregion

        #region Step2::RecordRefInDictionaryAndSetTimer
        var timerID = TimerManager.Ask4Timer(FadingShadowLifetime, eventData =>
        {
            //回忆ref，并交还pool
            var timerData = (TimerDieEventData)eventData;
            var goRef2Return = dictTimerID2GOref[timerData.TimerID];
            PoolManager.Instance().ReturnToPool(goRef2Return, PoolItemType.BoxMeFadingShadowCopy);
        });
        dictTimerID2GOref[timerID] = spawnResult; //记录ref
        #endregion
    }
}
