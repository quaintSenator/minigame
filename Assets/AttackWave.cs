using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWave : MonoBehaviour
{
    // Start is called before the first frame update
    private Material m_material;
    private Animator m_Animator;
    private void Play()
    {
        m_material.SetFloat("_StartTime", Time.time);
        //m_Animator.enabled = true;
        //m_Animator.Play("attackWaveRotation");
    }
    private void OnEnable()
    {
        m_material = gameObject.GetComponent<MeshRenderer>().material;
       // m_Animator = gameObject.GetComponent<Animator>();
        //m_Animator.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("GetKeyDown(KeyCode.T)");
            Play();
        }
    }
}
