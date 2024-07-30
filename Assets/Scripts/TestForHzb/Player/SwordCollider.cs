using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    private BoxCollider2D swordCollider;
    private EnemyController enemy;
    private int damage;


    private void Awake()
    {
        swordCollider = GetComponent<BoxCollider2D>();
        damage = 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            enemy = other.GetComponent<EnemyController>();
            enemy.TakeAttack(damage);
        }
    }

}