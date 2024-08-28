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
    
    void Update()
    {
        if (isMoving)
        {
            transform.position += -transform.right * speed * Time.deltaTime;
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                PoolManager.Instance.ReturnToPool("BossBullet", gameObject);
            }
        }
    }
}
