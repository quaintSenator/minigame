using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineBrain cinemachineBrain;

    private void Awake()
    {
        cinemachineBrain = GetComponent<CinemachineBrain>();
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.GameStartEvent, OnGameStart);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.GameStartEvent, OnGameStart);
    }
    
    private void OnGameStart(EventData data)
    {
        cinemachineBrain.enabled = true;
        Debug.Log("OnGameStart");
    }
}
