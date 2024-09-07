using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
    
    [Button]
    public void Init(float p1 =-1, float p2 = 0.125f, float p3 = 260, float p4=0.375f)
    {
        if(p1 == -1)
        {
            p1 = Time.timeSinceLevelLoad;
        }

/*        p2 = 0.125f;
        p3 = 260;
        p4 = 0.375f;*/
        m_material.SetFloat("_StartTime", p1);
        m_material.SetFloat("_OnceTime", p2);
        m_material.SetFloat("_Angle2Rotate", p3);
        m_material.SetFloat("_FadeTime", p4);
    }
}
