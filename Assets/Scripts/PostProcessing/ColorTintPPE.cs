using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTintPPE : MonoBehaviour
{
    public Material colorTintMaterial;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, colorTintMaterial);
    }
}
