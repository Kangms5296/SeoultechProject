using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialBlurImageEffect : MonoBehaviour
{
    private static RadialBlurImageEffect _instance;
    public static RadialBlurImageEffect instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<RadialBlurImageEffect>();
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "MyClassContainer";
                    _instance = container.AddComponent(typeof(RadialBlurImageEffect)) as RadialBlurImageEffect;
                }
            }

            return _instance;
        }
    }
    


    public float speed;
    public float blurSize;

    public Vector2 blurCenterPos = new Vector2(0.5f, 0.5f);

    [Range(1, 48)]
    public int samples;

    public Material radialBlurMaterial = null;

    private Coroutine blurCoroutine;

    public void VisibleBlur(float size)
    {
        blurSize = size;
    }

    public void InVisibleBlur()
    {
        blurSize = 0;
    }



    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(blurSize > 0.0f)
        {
            radialBlurMaterial.SetInt("_Samples", samples);
            radialBlurMaterial.SetFloat("_BlurSize", blurSize);
            radialBlurMaterial.SetVector("_BlurCenterPos", blurCenterPos);
            Graphics.Blit(source, destination, radialBlurMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
