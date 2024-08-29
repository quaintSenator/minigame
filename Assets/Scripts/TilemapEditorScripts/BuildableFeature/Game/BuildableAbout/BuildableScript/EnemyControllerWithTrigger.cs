using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerWithTrigger : EnemyController
{
    [SerializeField] private EnemyTrigger enemyTrigger;
    
    public override void Init()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;
        _spriteRenderer = spriteGo.GetComponent<SpriteRenderer>();
        _particleSystem = spriteGo.GetComponent<ParticleSystem>();
        CleverTimerManager.Ask4Timer(delayTime, CanPlay);
        _particleSystem.Stop();

        enemyTrigger.ResetTrigger();
    }
}
