using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class EndingAnimation : MonoBehaviour
{
    [SerializeField] private List<PathMover> pathMovers;
    [SerializeField] private float showUpTime = 0.5f;
    [SerializeField] private float duration = 5f;
    [SerializeField] private SpriteRenderer mask;
    private static EndingAnimation Instance;

    public static EndingAnimation SpawnEnding(Transform parent = null)
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        GameObject endingAnimation = Resources.Load<GameObject>("Ending");
        Instance = Instantiate(endingAnimation, parent).GetComponent<EndingAnimation>();
        Instance.transform.localPosition = new Vector3(0, 0, 10f);
        return Instance;
    }
    
    [Button]
    public void StopMove()
    {
        foreach (var pathMover in pathMovers)
        {
            pathMover.StopMove();
        }
    }

    [Button]
    public void PlayMove()
    {
        Debug.Log("PlayMove");
        foreach (var pathMover in pathMovers)
        {
            pathMover.PlayMove();
        }
    }
    
    private void Start()
    {
        mask.DOColor(new Color(1, 1, 1, 0), showUpTime);
        InitPathMovers();
        PlayMove();
    }
    
    private void InitPathMovers()
    {
        foreach (var pathMover in pathMovers)
        {
            pathMover.EditorMode = false;
            pathMover.GetTween(duration);
        }
    }
}
