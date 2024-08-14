using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BuildableBase {

    public int maxHealth;
    private int health;
    [SerializeField] private Material pixelCollapseMat;
    [SerializeField] private Material spriteDefaultMat;
    [SerializeField] private float delayTime = 0;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject spriteGo;
    private ParticleSystem _particleSystem;
    private Animator animator;

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
        CleverTimerManager.Instance.Ask4Timer(delayTime, CanPlay);
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
        Debug.Log("sf::OnDead Was called");
        gameObject.tag = "Safe";
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        animator.enabled = false;
        _spriteRenderer.material = pixelCollapseMat;
        _spriteRenderer.material.EnableKeyword("_ALPHABLEND_ON");
        float enemyHeight = 0f;
        if (gameObject.name.Equals("Enemy"))
        {
            enemyHeight = 1.4f;
        }
        spriteGo.transform.localPosition = new Vector3(0, enemyHeight, -1);
        spriteGo.transform.rotation = Quaternion.Euler(0, 180, 0);
        _spriteRenderer.material.SetFloat("_StartTime", Time.time);
        _particleSystem.Play();
        
        CleverTimerManager.Instance.Ask4Timer(2f, data =>
        {
            Destroy(gameObject);
        });
    }

    private void CanPlay(EventData data)
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