using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField]
    private GameObject child_0 = null;
    [SerializeField]
    private GameObject child_1 = null;

    [SerializeField] private SpriteRenderer m_bg_SR;
    [SerializeField] private SpriteRenderer m_bg_SR2;
    private Vector3 firstPosition = Vector3.zero;
    private Vector3 secondPosition = Vector3.zero;
    private Vector3 diffPosition = Vector3.zero;
    private Vector3 zOffset= Vector3.zero;

    [SerializeField]
    private float moveSpeed = 0.5f;

    private Vector3 targetDirection = new Vector3(-1, 0, 0);
    private GameObject parentCam = null;

    // Start is called before the first frame update
    void Start()
    {
        parentCam= transform.parent.gameObject;
        if (child_0 != null)
        {
            firstPosition = child_0.transform.position;
        }
        if (child_1 != null)
        {
            secondPosition = child_1.transform.position;
        }

        if(parentCam != null)
        {
            zOffset = new Vector3(0, 0, firstPosition.z- parentCam.transform.position.z);
        }

        diffPosition = secondPosition - firstPosition;
        firstPosition = firstPosition - (secondPosition-firstPosition);
    }

    public void OnPassResetPoint(EventData eventData)
    {
        PlayerPassRegisterResetPointEvent ed = (PlayerPassRegisterResetPointEvent)eventData;
        var index = ed.index;
        m_bg_SR.material.SetFloat("_StartTime", Time.timeSinceLevelLoad);
        m_bg_SR2.material.SetFloat("_StartTime", Time.timeSinceLevelLoad);
        m_bg_SR.material.SetInt("_Stage", index); 
        m_bg_SR2.material.SetInt("_Stage", index);
    }
    private void OnEnable()
    {
        EventManager.AddListener(EventType.PlayerPassRegisterResetPointEvent, OnPassResetPoint);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.PlayerPassRegisterResetPointEvent, OnPassResetPoint);
    }

    // Update is called once per frame
    void Update()
    {
        if(child_0 == null || child_1== null || parentCam==null)
        {
            return;
        }
        Vector3 prarentCamPosition= parentCam.transform.position;

        float step = moveSpeed * Time.deltaTime; // ����ÿ֡���ƶ�����
        child_0.transform.Translate(targetDirection * moveSpeed * Time.deltaTime, Space.World);
        child_1.transform.Translate(targetDirection * moveSpeed * Time.deltaTime, Space.World);
        if(child_0.transform.position.x <= prarentCamPosition.x - diffPosition.x)
        {
            child_0.transform.position = prarentCamPosition + diffPosition + zOffset;
            //child_0.transform.Translate(diffPosition, Space.World);
        }
        if (child_1.transform.position.x <= prarentCamPosition.x - diffPosition.x)
        {
            child_1.transform.position = prarentCamPosition + diffPosition + zOffset;
            //child_1.transform.Translate(diffPosition, Space.World);
        }
    }
}
