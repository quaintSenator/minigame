using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizedRhythmNode : MonoBehaviour
{
    private Material m_mat;
    [SerializeField] private float preset_HoldTime;
    [SerializeField] private float preset_beatStartTime;
    [SerializeField] private float preset_perfectRangeStartTime;
    [SerializeField] private float preset_perfectRangeEndTime;
    [SerializeField] private float preset_BeatEndTime;
    void Start()
    {
        var renderer = gameObject.GetComponent<Renderer>();
        m_mat = renderer.material;
    }
    
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.A))
        {
            Play();
        }*/
    }

    public void Play()
    {
        if(m_mat == null)
        {
            return;
        }
        m_mat.SetFloat("_CallTime", Time.time);
        m_mat.SetFloat("_HoldTime", Time.time + preset_HoldTime);
        m_mat.SetFloat("_BeatStartTime", Time.time + preset_beatStartTime);
        m_mat.SetFloat("_PerfectRangeStartTime", Time.time + preset_perfectRangeStartTime);
        m_mat.SetFloat("_PerfectRangeEndTime", Time.time + preset_perfectRangeEndTime);
        m_mat.SetFloat("_BeatEndTime", Time.time + preset_BeatEndTime);
    }

    public float GetStartTimeOffset()
    {
        return preset_perfectRangeStartTime;
    }
}
