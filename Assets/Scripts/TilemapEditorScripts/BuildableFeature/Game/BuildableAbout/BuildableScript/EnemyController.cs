using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BuildableBase {

    public int maxHealth;

    private int health;

    private void Awake()
    {
        health = maxHealth;
    }

    public override void Init()
    {
        health = maxHealth;
    }

    public void TakeAttack(int damage)
    {
        health-=damage;
        if (health<=0)
        {
            OnDead();
        }
    }

    private void OnDead()
    {
        Destroy(gameObject);
    }
}