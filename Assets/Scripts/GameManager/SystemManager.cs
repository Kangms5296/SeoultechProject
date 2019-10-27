using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;

public class SystemManager : MonoBehaviour
{
    private static SystemManager _instance = null;

    public static SystemManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(SystemManager)) as SystemManager;

                if (_instance == null)
                    Debug.LogError("There's no active SystemManager object");
            }

            return _instance;
        }
    }
    
    public float maxHitEffectTime;
    private float conHitEffectTime;

    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    public PostProcessVolume postProcessVolume;
    private ChromaticAberration chromaticAberration;

    public RadialBlurImageEffect radialBlurImageEffect;

    private bool isHitEffectCoroutineOn = false;
    private Coroutine hitEffectCoroutine;

    private bool isCameraShakeCoroutineOn = false;
    private Coroutine cameraShakeCoroutine;
    private float conMagnitude = 0;

    private void Start()
    {
        if (virtualCamera != null)
            virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        if (postProcessVolume != null)
            chromaticAberration = postProcessVolume.profile.GetSetting<ChromaticAberration>();
    }


    public void HitEffect(bool isSlowMode, float magnitude)
    {
        if (isHitEffectCoroutineOn)
            StopCoroutine(hitEffectCoroutine);

        hitEffectCoroutine = StartCoroutine(HitEffectCoroutine(isSlowMode, magnitude));
    }

    public void CameraShake(float time, float magnitude)
    {
        // 현재 흔들리는 정도가 흔들어아햐는 정도보다 쌔면 무시
        if (conMagnitude > magnitude)
            return;

        // 
        if (isCameraShakeCoroutineOn)
            StopCoroutine(cameraShakeCoroutine);

        cameraShakeCoroutine = StartCoroutine(CameraShakeCoroutine(time, magnitude));
    }

    private IEnumerator HitEffectCoroutine(bool isSlowMode, float magnitude)
    {
        isHitEffectCoroutineOn = true;

        float value;

        if (isSlowMode)
            Time.timeScale = 0.25f;

        while (conHitEffectTime < maxHitEffectTime)
        {
            value = conHitEffectTime / maxHitEffectTime;

            chromaticAberration.intensity.value = value * 0.8f * magnitude;
            radialBlurImageEffect.blurSize = value * 1.6f * magnitude;

            conHitEffectTime += Time.deltaTime;
            yield return null;
        }
        chromaticAberration.intensity.value = 0.8f * magnitude;
        radialBlurImageEffect.blurSize = 1.5f * magnitude;
        conHitEffectTime = maxHitEffectTime;



        while (conHitEffectTime > 0)
        {
            value = conHitEffectTime / maxHitEffectTime;

            chromaticAberration.intensity.value = value * 0.8f * magnitude;
            radialBlurImageEffect.blurSize = value * 1.5f * magnitude;

            conHitEffectTime -= Time.deltaTime;
            yield return null;
        }
        chromaticAberration.intensity.value = 0;
        radialBlurImageEffect.blurSize = 0;
        conHitEffectTime = 0;

        if (isSlowMode)
            Time.timeScale = 1f;

        isHitEffectCoroutineOn = false;
    }

    private IEnumerator CameraShakeCoroutine(float time, float magnitude)
    {
        isCameraShakeCoroutineOn = true;


        float amplitude = 0.1f  * magnitude;
        float frequency = 0.08f;


        // 점점 카메라가 흔들어진다.
        float conTime = 0;
        while (conTime < time * 0.5f)
        {
            virtualCameraNoise.m_AmplitudeGain = amplitude * conTime * 2;
            virtualCameraNoise.m_FrequencyGain = frequency * conTime * 2;

            conTime += Time.deltaTime;
            yield return null;
        }
        conTime = 0.5f;
        virtualCameraNoise.m_AmplitudeGain = amplitude;
        virtualCameraNoise.m_FrequencyGain = frequency;


        // 점점 카메라가 멈춘다.
        while (conTime > 0)
        {
            virtualCameraNoise.m_AmplitudeGain = amplitude * conTime * 2;
            virtualCameraNoise.m_FrequencyGain = frequency * conTime * 2;

            conTime -= Time.deltaTime;
            yield return null;
        }
        virtualCameraNoise.m_AmplitudeGain = 0;
        virtualCameraNoise.m_FrequencyGain = 0;


        isCameraShakeCoroutineOn = false;
    }

}
