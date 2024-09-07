using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWaveEffectEndOrDieEventData : EventData
{
    public GameObject goDying;
    
    public AttackWaveEffectEndOrDieEventData(GameObject go)
    {
        goDying = go;
    }
}
public class AttackWaveEffectController : MonoBehaviour
{
    [SerializeField] private GameObject attackWaveEffectPrefab;
    [SerializeField] private float onceTime;

    private float angle2Rotate;

    [SerializeField] private Vector3 rightPosition = new Vector3(1, 0, 0);
    [SerializeField] private Vector3 upPosition = new Vector3(0.5f, 1, 0);

    [SerializeField] private float angle2RotateUp=325.0f;
    [SerializeField] private float angle2RotateRight = 260.0f;

    [SerializeField] private float fadeTime;
    [SerializeField] private GameObject bg;


    private bool isUp = false;

    private void OnEnable()
    {
        EventManager.AddListener(EventType.ChangeDirectionEvent, OnChangeDirectionEvent);
        EventManager.AddListener(EventType.RestartLevelEvent, OnRestartLevelEvent);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.ChangeDirectionEvent, OnChangeDirectionEvent);
        EventManager.RemoveListener(EventType.RestartLevelEvent, OnRestartLevelEvent);
    }

    void Start()
    {
        //向预制件中的material实例传递初始化参数
        angle2Rotate = angle2RotateRight;
    }
    
    public void SpawnEffectInstance()
    {


        var obj = PoolManager.Instance.SpawnFromPool("AttackWaveEffect", attackWaveEffectPrefab, transform);


        obj.transform.localPosition = new Vector3(0, 0, 0);//isUp ? upPosition : rightPosition;


        obj.transform.SetParent(bg.transform);
        CleverTimerManager.Ask4Timer(onceTime + fadeTime + 0.01f, OnAttackWaveDie, new AttackWaveEffectEndOrDieEventData(obj));
        var attackWave = obj.GetComponent<AttackWave>();
        if (attackWave)
        {
            attackWave.Init(Time.time, onceTime, angle2Rotate, fadeTime);
        }
        else
        {
            Debug.LogError("prefab has no attackWave component!!");
        }
    }
    
    void OnAttackWaveDie(EventData ed)
    {
        var obj2Return = ((AttackWaveEffectEndOrDieEventData)ed).goDying;
        PoolManager.Instance.ReturnToPool("AttackWaveEffect", obj2Return);
    }


    private void OnChangeDirectionEvent(EventData eventData)        
    {
        isUp = !isUp;
        if (isUp)
        {
            angle2Rotate = angle2RotateUp;
            this.transform.localPosition = upPosition;
        }
        else
        {
            angle2Rotate = angle2RotateRight;

            this.transform.localPosition = rightPosition;
        }
    }

    private void OnRestartLevelEvent(EventData eventData)
    {
        isUp = false;
        angle2Rotate = angle2RotateRight;
    }
}
