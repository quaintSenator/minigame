using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PPE2OnRenderTest : MonoBehaviour
{
    [SerializeField] private Material mat1;
    [SerializeField] private Material mat2;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat1);
        Graphics.Blit(source, destination, mat2);
    }
}
