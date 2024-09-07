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

    public float upScreenX = 0.5f;
    public float upDeadZoneWidth = 0.2f;
    public float changeToUpDirectionDuration = 1.50f;
    public float changeToRightDirectionDuration = 0.50f;
    private bool isChangingDircetion = false;

    private float lastTriggerChangeTime = -1;

    private bool isMovingTowardUp = false;
    private float nextCanTriggerChangeDirectionTime = 0;
    private float triggerChangeDirectionCD = 10;



    private void Update()
    {
        UpdateCamera();
    }
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
            // Debug.Log("pass change");
            isMovingTowardUp = !isMovingTowardUp;
            isChangingDircetion = true;
            lastTriggerChangeTime = Time.time ;
        }
        else
        {
            return;
        }


/*        if (isMovingTowardUp && cinemachineFraming != null)
        {
            cinemachineFraming.m_ScreenX = 0.5f;
            cinemachineFraming.m_DeadZoneWidth = 0.2f;

        }
        else
        {
            GoBackToNormal();
        }*/
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


    private void UpdateCamera()
    {
        if(!isChangingDircetion)
        {
            return; 
        }
        if (isMovingTowardUp)
        {
            if (NearlyEqualsFloat(cinemachineFraming.m_ScreenX, upScreenX, 0.01f) && NearlyEqualsFloat(cinemachineFraming.m_DeadZoneWidth, upDeadZoneWidth, 0.01f))
            {
                isChangingDircetion = false;
                cinemachineFraming.m_ScreenX = upScreenX;
                cinemachineFraming.m_DeadZoneWidth = upDeadZoneWidth;
                return;
            }
            else
            {
                float elapsedTime = Time.time - lastTriggerChangeTime; // 已经经过的时间
                float t = Mathf.Clamp01(elapsedTime / changeToRightDirectionDuration); // 计算插值因子，确保在0和1之间

                cinemachineFraming.m_ScreenX = Mathf.Lerp(normalScreenX, upScreenX, t);
                cinemachineFraming.m_DeadZoneWidth = Mathf.Lerp(normalDeadZoneWidth, upDeadZoneWidth, t);
            }
        }

        // if (isMovingTowardUp)
        else
        {
            if (NearlyEqualsFloat(cinemachineFraming.m_ScreenX, normalScreenX, 0.01f) && NearlyEqualsFloat(cinemachineFraming.m_DeadZoneWidth, normalDeadZoneWidth, 0.01f))
            {
                isChangingDircetion = false;
                cinemachineFraming.m_ScreenX = normalScreenX;
                cinemachineFraming.m_DeadZoneWidth = normalDeadZoneWidth;
                return;
            }
            else
            {

                float elapsedTime = Time.time - lastTriggerChangeTime; // 已经经过的时间
                float t = Mathf.Clamp01(elapsedTime / changeToRightDirectionDuration); // 计算插值因子，确保在0和1之间

                cinemachineFraming.m_ScreenX = Mathf.Lerp(upScreenX, normalScreenX, t);
                cinemachineFraming.m_DeadZoneWidth = Mathf.Lerp(upDeadZoneWidth, normalDeadZoneWidth, t);

            }

        }

    }

    public static bool NearlyEqualsFloat(float float1, float float2, float tolerance = 0.01f)
    {
        return Mathf.Abs(float1 - float2)< tolerance;
    }
}
