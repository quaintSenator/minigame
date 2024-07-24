using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineBrain cinemachineBrain;
    private CinemachineVirtualCamera cinemachine;

    private void Awake()
    {
        cinemachineBrain = GetComponent<CinemachineBrain>();
        cinemachine = transform.parent.gameObject.GetComponent<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.GameStartEvent, OnGameStart);
        EventManager.AddListener(EventType.GameRestartEvent, OnGameRestart);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.GameStartEvent, OnGameStart);
        EventManager.RemoveListener(EventType.GameRestartEvent, OnGameRestart);
    }
    
    private void OnGameStart(EventData data)
    {
        //cinemachineBrain.enabled = true;
        Debug.Log("OnGameStart");
    }

    private void OnGameRestart(EventData data)
    {
       // cinemachine.transform.position= new Vector3(0.0f,cinemachine.transform.position.y,cinemachine.transform.position.z);
    }
}
