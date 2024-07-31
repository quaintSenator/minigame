using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    private BoxCollider2D swordCollider;
    private SpriteRenderer spriteRenderer;
    private EnemyController enemy;
    private int damage;


    private void Awake()
    {
        swordCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        damage = 1;
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.JDownEvent,OnAttack);
        EventManager.AddListener(EventType.JUpEvent,OnBackAttack);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.JDownEvent,OnAttack);
        EventManager.RemoveListener(EventType.JUpEvent,OnBackAttack);
    }

    private void OnAttack(EventData data = null)
    {
        swordCollider.enabled = true;
        spriteRenderer.enabled = true;
    }

    private void OnBackAttack(EventData data = null)
    {
        swordCollider.enabled = false;
        spriteRenderer.enabled = false;
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