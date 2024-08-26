using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private Vector3 showUpPosition;
    [SerializeField] private float showUpTime = 1.5f;
    public static BossController CurrentBoss;
    private static Transform centerPoint;

    public static void InitBoss()
    {
        if (centerPoint == null)
        {
            centerPoint = new GameObject("CenterPoint").transform;
            centerPoint.position = new Vector3(0, Utils.GetStartPointPostion().position.y, 0);
            centerPoint.gameObject.AddComponent<CenterPoint>();
        }
        GameObject bossPrefab = Resources.Load<GameObject>("Boss");
        if (bossPrefab)
        {
            CurrentBoss = PoolManager.Instance.SpawnFromPool("Boss", bossPrefab, centerPoint)
                .GetComponent<BossController>();
            CurrentBoss.transform.localPosition = new Vector3(CurrentBoss.spawnPosition.x, CurrentBoss.spawnPosition.y, 10);
        }
    }
    
    [Button]
    public void ShowUp()
    {
        transform.DOLocalMove(showUpPosition, showUpTime);
    }
}
