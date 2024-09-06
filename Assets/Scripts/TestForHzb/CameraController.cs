using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    private CinemachineBrain cinemachineBrain;
    private CinemachineVirtualCamera cinemachine;


    private CinemachineFramingTransposer cinemachineFraming = null;
    private float normalScreenX = 0;
    private float normalDeadZoneWidth = 0;

    private bool isMovingTowardUp = false;
    private float nextCanTriggerChangeDirectionTime = 0;
    private float triggerChangeDirectionCD = 10;

    private void Awake()
    {
        //cinemachineBrain = GetComponent<CinemachineBrain>();
        cinemachine = GetComponent<CinemachineVirtualCamera>();
        cinemachineFraming = cinemachine.GetCinemachineComponent<CinemachineFramingTransposer>();

        normalScreenX = cinemachineFraming.m_ScreenX;
        normalDeadZoneWidth = cinemachineFraming.m_DeadZoneWidth;

        Debug.Log("cinemachine"+cinemachine);
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.GameStartEvent, OnGameStart);
        EventManager.AddListener(EventType.GameRestartEvent, OnReset);

        EventManager.AddListener(EventType.ChangeDirectionEvent, OnChangeDirectionEvent);
        EventManager.AddListener(EventType.RestartLevelEvent, OnRestartLevelEvent);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.GameStartEvent, OnGameStart);
        EventManager.RemoveListener(EventType.GameRestartEvent, OnReset);

        EventManager.RemoveListener(EventType.ChangeDirectionEvent, OnChangeDirectionEvent);

        EventManager.RemoveListener(EventType.RestartLevelEvent, OnRestartLevelEvent);
    }
    
    private void OnGameStart(EventData data)
    {
        cinemachine.enabled = true;
        Debug.Log("OnGameStart");
    }

    private void OnReset(EventData data)
    {
        // cinemachine.transform.position= new Vector3(0.0f,cinemachine.transform.position.y,cinemachine.transform.position.z);
        //cinemachine.
    }

    private void  OnChangeDirectionEvent(EventData data)
    {
        if (Time.time > nextCanTriggerChangeDirectionTime)
        {
            nextCanTriggerChangeDirectionTime = triggerChangeDirectionCD + Time.time;
            Debug.Log("pass change");
            isMovingTowardUp = !isMovingTowardUp;
        }
        else
        {
            return;
        }


        if (isMovingTowardUp && cinemachineFraming != null)
        {
            cinemachineFraming.m_ScreenX = 0.5f;
            cinemachineFraming.m_DeadZoneWidth = 0.2f;

        }
        else
        {
            GoBackToNormal();
        }
    }

    private void GoBackToNormal()
    {
        cinemachineFraming.m_ScreenX = normalScreenX;
        cinemachineFraming.m_DeadZoneWidth = normalDeadZoneWidth; ;
        isMovingTowardUp = false;
        nextCanTriggerChangeDirectionTime = -1;
    }

    private void OnRestartLevelEvent(EventData data)
    {
        GoBackToNormal();
    }
}
