using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BuildableBase {

    public int maxHealth;
    protected int health;
    [SerializeField] protected Material pixelCollapseMat;
    [SerializeField] protected Material spriteDefaultMat;
    [SerializeField] protected float delayTime = 0;
    protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected GameObject spriteGo;
    protected ParticleSystem _particleSystem;
    protected Animator animator;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;
        _spriteRenderer = spriteGo.GetComponent<SpriteRenderer>();
        _particleSystem = spriteGo.GetComponent<ParticleSystem>();
        CleverTimerManager.Ask4Timer(delayTime, CanPlay);
        _particleSystem.Stop();
    }

    public void TakeAttack(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            OnDead();
        }
    }

    private void OnDead()
    {
        gameObject.tag = "Safe";
        gameObject.GetComponent<Collider2D>().enabled = false;
        animator.enabled = false;
        _spriteRenderer.material = pixelCollapseMat;
        _spriteRenderer.material.EnableKeyword("_ALPHABLEND_ON");
        /*float enemyHeight = 0f;
        if (gameObject.name.Equals("Enemy"))
        {
            enemyHeight = 1.4f;
        }*/
        //spriteGo.transform.localPosition = new Vector3(0, enemyHeight, 0);
        //spriteGo.transform.rotation = Quaternion.Euler(0, 180, 0);
        _spriteRenderer.material.SetFloat("_StartTime", Time.time);
        _particleSystem.Play();
        
        CleverTimerManager.Ask4Timer(2f, data =>
        {
            Destroy(gameObject);
        });
    }

    protected void CanPlay(EventData data)
    {
        animator.SetTrigger("CanPlay");
    }

    public void SetDelayTime(float time)
    {
        if(time > 0){
            delayTime = time;
        }
    }
}