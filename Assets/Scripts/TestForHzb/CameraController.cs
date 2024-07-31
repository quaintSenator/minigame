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
        //cinemachineBrain = GetComponent<CinemachineBrain>();
        cinemachine = GetComponent<CinemachineVirtualCamera>();
        Debug.Log("cinemachine"+cinemachine);
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.GameStartEvent, OnGameStart);
        EventManager.AddListener(EventType.GameRestartEvent, OnReset);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.GameStartEvent, OnGameStart);
        EventManager.RemoveListener(EventType.GameRestartEvent, OnReset);
    }
    
    private void OnGameStart(EventData data)
    {
        cinemachine.enabled = true;
        Debug.Log("OnGameStart");
    }

    private void OnReset(EventData data)
    {
       // cinemachine.transform.position= new Vector3(0.0f,cinemachine.transform.position.y,cinemachine.transform.position.z);
    }
}
