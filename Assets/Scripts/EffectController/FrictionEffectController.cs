using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrictionEffectController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem myParticleSystem;
    [SerializeField] private float minimumPlayerYDecidedAsOnGround;
    [SerializeField] private Color particleRandomColorRangeL;
    [SerializeField] private Color particleRandomColorRangeR;
    // Start is called before the first frame update
    void selfPSInit()
    {
        var main = myParticleSystem.main;
        ParticleSystem.MinMaxGradient colorRange2Set = new ParticleSystem.MinMaxGradient(particleRandomColorRangeL, particleRandomColorRangeR);
        main.startColor = colorRange2Set;
    }
    private void OnEnable()
    {
        selfPSInit();
    }
    
    void Update()
    {
        
    }
}
