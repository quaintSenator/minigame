using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    private BoxCollider2D swordCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private EnemyController enemy;
    [SerializeField]private float coldDownTime = 0.4f;
    [SerializeField]private AttackWaveEffectController _attackWaveEffectController;
    [SerializeField]private Material attackWaveMat;
    private bool canAttack = true;
    private int damage;

    public AK.Wwise.Event ActionAttackEvent = null;


    private void Awake()
    {
        swordCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        damage = 1;
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.JDownEvent,OnAttack);
        //EventManager.AddListener(EventType.JUpEvent,OnBackAttack);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.JDownEvent,OnAttack);
        //EventManager.RemoveListener(EventType.JUpEvent,OnBackAttack);
    }

    private void OnAttack(EventData data = null)
    {
        if(canAttack)
        {
            animator.SetTrigger("Attack");
            CleverTimerManager.Ask4Timer(coldDownTime, SetAttack);
            _attackWaveEffectController.SpawnEffectInstance();
            canAttack = false;

            ActionAttackEvent.Post(gameObject);
        }
        //swordCollider.enabled = true;
        //spriteRenderer.enabled = true;
    }

    private void OnBackAttack(EventData data = null)
    {
        //swordCollider.enabled = false;
        //spriteRenderer.enabled = false;
    }

    private void SetAttack(EventData data =null)
    {
        canAttack = true;
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