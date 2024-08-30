using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPScounter : MonoBehaviour
{
    private float lastShowTime = 0;
    private float showDeltaTime = 0.3f;
    private int frameCount = 0;
    public Text fpsText;
    
    void Update()
    {
        frameCount++;
        if (Time.time - lastShowTime > showDeltaTime)
        {
            fpsText.text = "FPS: " + (int)(frameCount / (Time.time - lastShowTime));
            lastShowTime = Time.time;
            frameCount = 0;
        }
    }
}
