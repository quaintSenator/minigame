using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class FrameQueue : MonoBehaviour
{
    [SerializeField]public SpriteAtlas mAtlas;
    [SerializeField] public SpriteRenderer mSpriteRenderer;
    private void Start()
    {
        mSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (!mSpriteRenderer || !mAtlas)
        {
            Debug.LogError("Bad FrameQueue Init. Lost of significant reference.");
        }
    }
    
}
