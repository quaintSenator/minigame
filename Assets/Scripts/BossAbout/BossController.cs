using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [BoxGroup("Boss出场相关设置"), Tooltip("Boss出生位置")]
    [SerializeField] private Vector3 spawnPosition;
    [BoxGroup("Boss出场相关设置"), Tooltip("Boss出场目标位置")]
    [SerializeField] private Vector3 showUpPosition;
    [BoxGroup("Boss出场相关设置"), Tooltip("出生位置移动至目标位置的时间")]
    [SerializeField] private float showUpTime = 1.5f;
    
    [BoxGroup("Boss激光相关设置")]
    [SerializeField] private GameObject laser;
    
    public static BossController CurrentBoss;
    private static Transform centerPoint;

    private void OnEnable()
    {
        EventManager.AddListener(EventType.StopOrPlayMusicEvent, DestroyBoss);
        EventManager.AddListener(EventType.EndPlayerDeadEvent, DestroyBoss);
        EventManager.AddListener(EventType.ReleaseLaserEvent, ReleaseLaser);
        EventManager.AddListener(EventType.ReleaseBulletEvent, ReleaseBullet);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.StopOrPlayMusicEvent, DestroyBoss);
        EventManager.RemoveListener(EventType.EndPlayerDeadEvent, DestroyBoss);
        EventManager.RemoveListener(EventType.ReleaseLaserEvent, ReleaseLaser);
        EventManager.RemoveListener(EventType.ReleaseBulletEvent, ReleaseBullet);
    }

    public static void InitBoss()
    {
        if (centerPoint == null)
        {
            centerPoint = new GameObject("CenterPoint").transform;
            centerPoint.transform.parent = Camera.main.transform;
            centerPoint.transform.localPosition = Vector3.zero;
            centerPoint.gameObject.AddComponent<CenterPoint>();
        }
        GameObject bossPrefab = Resources.Load<GameObject>("Boss");
        if (bossPrefab)
        {
            CurrentBoss = PoolManager.Instance.SpawnFromPool("Boss", bossPrefab, centerPoint)
                .GetComponent<BossController>();
            CurrentBoss.transform.localPosition = new Vector3(CurrentBoss.spawnPosition.x, CurrentBoss.spawnPosition.y, 10);
            CurrentBoss.ShowUp();
        }
    }

    private void DestroyBoss(EventData obj)
    {
        PoolManager.Instance.ReturnToPool("Boss", gameObject);
    }
    
    [Button]
    public void ShowUp()
    {
        transform.DOLocalMove(showUpPosition, showUpTime);
    }

    [Button]
    public void ReleaseLaser(EventData obj)
    {
        var data = obj as ReleaseLaserEvent;
        
        float localTargetY = data.position.y - centerPoint.transform.position.y;
        transform.DOLocalMove(new Vector3(transform.localPosition.x, localTargetY, 0), 0.2f)
            .onComplete = () =>
        {
            transform.DOShakeScale(data.continueTime, 0.1f, 2, 10, true, ShakeRandomnessMode.Harmonic);
        };
        laser.SetActive(true);
        laser.transform.DOShakeScale(data.continueTime, 0.1f, 2, 10, true, ShakeRandomnessMode.Harmonic)
            .onComplete = () =>
        {
            laser.SetActive(false);
            transform.DOLocalMove(showUpPosition, 0.2f);
        };
    }

    [Button]
    public void ReleaseBullet(EventData data)
    {
        transform.DOShakeScale(1, 0.6f, 7, 90, true, ShakeRandomnessMode.Harmonic);
    }
}
