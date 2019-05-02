using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ShadowThresholdCustomEffect : MonoBehaviour
{
    public Material shadowMaterial;

    [Range(0, 1)]
    public float shadowThreshold;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        shadowMaterial.SetFloat("_ShadowThreshold", shadowThreshold);
        Graphics.Blit(source, destination, shadowMaterial);
    }
}
