using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapseTestor : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Material _material;
    [SerializeField] private ParticleSystem _particleSystem;
    
    void Start()
    {
        _material = gameObject.GetComponent<MeshRenderer>().material;
        _particleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _material.SetFloat("_StartTime", Time.time);
            _particleSystem.Play(true);
        }
    }
}
