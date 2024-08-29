using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [SerializeField] private int gapSpace = 1;
    private int lastGapSpace = 0;
    
    private void Update()
    {
        if (gapSpace != lastGapSpace)
        {
            lastGapSpace = gapSpace;
            transform.localPosition = new Vector3(-gapSpace * GameConsts.TILE_SIZE, 0, 0);
        }
    }
}
