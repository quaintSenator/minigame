using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    private EndingAnimation animation;
    
    [Button]
    public void TestMethod()
    {
        animation = EndingAnimation.SpawnEnding(Camera.main.transform);
    }
    
    [Button]
    public void StopMove()
    {
        animation.StopMove();
    }
    
    [Button]
    public void PlayMove()
    {
        animation.PlayMove();
    }
}
