using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWaveEffectDieEventData : EventData
{
    public GameObject goDying;
    

    public AttackWaveEffectDieEventData(GameObject go)
    {
        goDying = go;
    }
}
public class AttackWaveEffectController : MonoBehaviour
{
    [SerializeField]private GameObject attackWaveEffectPrefab;
    [SerializeField] private float onceTime;
    [SerializeField] private float angle2Rotate;
    [SerializeField] private float fadeTime;
    void Start()
    {
        //向预制件中的material实例传递初始化参数
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SpawnEffectInstance();
        }
    }

    void SpawnEffectInstance()
    {
        var obj = PoolManager.Instance.SpawnFromPool("AttackWaveEffect", attackWaveEffectPrefab, transform);
        CleverTimerManager.Instance.Ask4Timer(1, OnAttackWaveDie, new AttackWaveEffectDieEventData(obj));
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
        var obj2Return = ((AttackWaveEffectDieEventData)ed).goDying;
        PoolManager.Instance.ReturnToPool("AttackWaveEffect", obj2Return);
    }
}
