using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BuildableBase {

    public int maxHealth;

    private int health;

    [SerializeField]
    private float delayTime = 0;

    private Animator animator;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;
        CleverTimerManager.Instance.Ask4Timer(delayTime, CanPlay);
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
        Destroy(gameObject);
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