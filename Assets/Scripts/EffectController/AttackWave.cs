using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWave : MonoBehaviour
{
    // Start is called before the first frame update
    private Material m_material;
    private Animator m_Animator;
    public void Play()
    {
        m_material.SetFloat("_StartTime", Time.time);
    }
    private void OnEnable()
    {
        m_material = gameObject.GetComponent<MeshRenderer>().material;
       // m_Animator = gameObject.GetComponent<Animator>();
        //m_Animator.enabled = false;
    }
    public void Init(float p1, float p2, float p3, float p4)
    {
        m_material.SetFloat("_StartTime", p1);
        m_material.SetFloat("_OnceTime", p2);
        m_material.SetFloat("_Angle2Rotate", p3);
        m_material.SetFloat("_FadeTime", p4);
    }
}
