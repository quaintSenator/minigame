using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyTrigger : MonoBehaviour
{
    [SerializeField] private int gapSpace = 1;
    private int lastGapSpace = 0;
    [SerializeField] private GameObject triggerPreview;
    [SerializeField] private GameObject enemyPreview;
    [SerializeField] private GameObject linkLinePreview;
    [SerializeField] private GameObject enemyVisual;
    [SerializeField] private EnemyControllerWithTrigger enemy;

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "TilemapEditorScene")
        {
            enemyPreview.SetActive(true);
        }
        else
        {
            enemyPreview.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out MusicCurrentPosLine line) || other.TryGetComponent(out PlayerController player))
        {
            ShowEnemy();
        }
    }
    
    public void ShowEnemy()
    {
        enemyVisual.SetActive(true);
        enemyPreview.SetActive(false);
        enemy.InitComponent();
        enemyVisual.transform.DOScale(new Vector3(1, 1, 1),0.4f).onComplete = () =>
        {
            enemyVisual.transform.DOShakeScale(0.2f, 0.3f, 5, 90, true, ShakeRandomnessMode.Harmonic);
            EventManager.InvokeEvent(EventType.ReleaseEnemyEvent);
        };
    }

    private void Update()
    {
        if (gapSpace != lastGapSpace)
        {
            lastGapSpace = gapSpace;
            triggerPreview.transform.localPosition = new Vector3(-gapSpace * GameConsts.TILE_SIZE, 0, 0);
            enemyPreview.transform.localPosition = Vector3.zero;
            linkLinePreview.transform.localPosition = new Vector3(-gapSpace * GameConsts.TILE_SIZE / 2f, 0, 0);
            linkLinePreview.transform.localScale = new Vector3((gapSpace - 1) * GameConsts.TILE_SIZE, 0.3f, 1);
            GetComponent<BoxCollider2D>().offset = new Vector2(-gapSpace * GameConsts.TILE_SIZE, 0);
        }
    }

    public void ResetTrigger()
    {
        if (SceneManager.GetActiveScene().name != "TilemapEditorScene")
        {
            enemyPreview.SetActive(true);
        }
        enemyVisual.SetActive(false);
        enemyVisual.transform.localScale = Vector3.zero;
    }
}
