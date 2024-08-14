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
    [SerializeField] private float angle2Rotate;
    [SerializeField] private float fadeTime;
    [SerializeField] private GameObject bg;
    void Start()
    {
        //向预制件中的material实例传递初始化参数
        
    }
    
    public void SpawnEffectInstance()
    {
        var obj = PoolManager.Instance.SpawnFromPool("AttackWaveEffect", attackWaveEffectPrefab, transform);
        obj.transform.localPosition = new Vector3(0, 0, 0);
        obj.transform.SetParent(bg.transform);
        CleverTimerManager.Instance.Ask4Timer(onceTime + fadeTime + 0.01f, OnAttackWaveDie, new AttackWaveEffectEndOrDieEventData(obj));
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
}
