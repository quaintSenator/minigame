using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    private float speed = 0f;
    private bool isMoving = false;
    private float lifeTime = 0f;
    
    public void Init(Vector3 position, float speed, float lifeTime)
    {
        this.speed = speed;
        this.lifeTime = lifeTime;
        transform.position = position;
        isMoving = true;
    }

    

    private void OnEnable()
    {
        EventManager.AddListener(EventType.StartPlayerDeadEvent, DestroySelf);
        EventManager.AddListener(EventType.StartLevelEvent, DestroySelf);
        EventManager.AddListener(EventType.PlayerDeadStoryEvent, DestroySelf);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.StartPlayerDeadEvent, DestroySelf);
        EventManager.RemoveListener(EventType.StartLevelEvent, DestroySelf);
        EventManager.RemoveListener(EventType.PlayerDeadStoryEvent, DestroySelf);
    }

    private void DestroySelf(EventData obj)
    {
        lifeTime = 0;
        isMoving = false;
        PoolManager.Instance.ReturnToPool("BossBullet", gameObject);
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position += -transform.right * speed * Time.deltaTime;
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                isMoving = false;
                PoolManager.Instance.ReturnToPool("BossBullet", gameObject);
            }
        }
    }
}
