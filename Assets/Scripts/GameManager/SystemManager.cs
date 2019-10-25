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

    // 공격 피격 연출
    public float maxHitEffectTime;      // 최고 상태로 변하는 시간
    private float conHitEffectTime;
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    public PostProcessVolume postProcessVolume;
    private ChromaticAberration chromaticAberration;
    public RadialBlurImageEffect radialBlurImageEffect;
    private Coroutine hitEffectCoroutine;
    private bool isHitEffectCoroutineOn = false;


    private void Start()
    {
        if (virtualCamera != null)
            virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        if (postProcessVolume != null)
            chromaticAberration = postProcessVolume.profile.GetSetting<ChromaticAberration>();
    }


    public void HitEffect(float magnitude)
    {
        if (isHitEffectCoroutineOn)
            StopCoroutine(hitEffectCoroutine);

        hitEffectCoroutine = StartCoroutine(HitEffectCoroutine(magnitude));
    }



    private IEnumerator HitEffectCoroutine(float magnitude)
    {
        isHitEffectCoroutineOn = true;
        Time.timeScale = 0.1f;

        float value;

        while (conHitEffectTime < maxHitEffectTime)
        {
            value = conHitEffectTime / maxHitEffectTime;
            virtualCameraNoise.m_AmplitudeGain = value * magnitude;
            virtualCameraNoise.m_FrequencyGain = value * 0.1f;
            chromaticAberration.intensity.value = value * magnitude;
            radialBlurImageEffect.blurSize = value * magnitude * 10;

            conHitEffectTime += Time.deltaTime;
            yield return null;
        }
        virtualCameraNoise.m_AmplitudeGain = magnitude;
        virtualCameraNoise.m_FrequencyGain = 0.1f;
        chromaticAberration.intensity.value = magnitude;
        radialBlurImageEffect.blurSize = magnitude * 10;
        conHitEffectTime = maxHitEffectTime;

        while (conHitEffectTime > 0)
        {
            value = conHitEffectTime / maxHitEffectTime;

            virtualCameraNoise.m_AmplitudeGain = value * magnitude;
            virtualCameraNoise.m_FrequencyGain = value * 0.1f;
            chromaticAberration.intensity.value = value * magnitude;
            radialBlurImageEffect.blurSize = value * magnitude * 10;

            conHitEffectTime -= Time.deltaTime;
            yield return null;
        }
        virtualCameraNoise.m_AmplitudeGain = 0;
        virtualCameraNoise.m_FrequencyGain = 0;
        chromaticAberration.intensity.value = 0;
        radialBlurImageEffect.blurSize = 0;
        conHitEffectTime = 0;

        Time.timeScale = 1f;
        isHitEffectCoroutineOn = false;
    }

}
