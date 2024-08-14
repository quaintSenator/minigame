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
    [SerializeField]private GameObject attackWave;
    [SerializeField]private Material attackWaveMat;
    private bool canAttack = true;
    private int damage;


    private void Awake()
    {
        swordCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        /*if (attackWave)
        {
            var meshRenderer = attackWave.gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                attackWaveMat = meshRenderer.material;
            }
        }*/
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
            CleverTimerManager.Instance.Ask4Timer(coldDownTime,SetAttack);
            if (attackWaveMat)
            {
                attackWaveMat.SetFloat("_TimeOfStart", Time.time);
            }
            canAttack = false;
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