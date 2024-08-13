using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideController : MonoBehaviour
{
    [SerializeField]
    private float waitTime = 3f;
    [SerializeField]
    private float fadeTime = 1f;
    [SerializeField]
    private float startAlpha = 1f;

    private float fadeTimer = 0f;
    private bool fading = false;

    private float deltaAlpha;

    private SpriteRenderer jumpRenderer;
    private SpriteRenderer attackRenderer;

    private void Awake()
    {
        jumpRenderer = transform.Find("Jump").GetComponent<SpriteRenderer>();
        attackRenderer = transform.Find("Attack").GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.GameStartEvent, OnStart);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.GameStartEvent, OnStart);
    }

    private void Start()
    {
        deltaAlpha = startAlpha / fadeTime;
    }

    private void Update()
    {
        if(fading){
            fadeTimer += Time.deltaTime;

            Color jumpColor = jumpRenderer.color;
            Color attackColor = attackRenderer.color;

            jumpColor.a -= deltaAlpha * Time.deltaTime;
            attackColor.a -= deltaAlpha * Time.deltaTime;

            jumpRenderer.color = jumpColor;
            attackRenderer.color = attackColor;
        }
        if(fadeTimer > fadeTime){
            fading = false;
            fadeTimer = 0;
        }
    }

    private void OnStart(EventData data = null)
    {
        jumpRenderer.enabled = true;
        attackRenderer.enabled = true;
        CleverTimerManager.Instance.Ask4Timer(waitTime, StartFade);
    }

    private void StartFade(EventData data = null)
    {
        fading = true;
    }

}
