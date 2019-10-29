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
    

    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    public PostProcessVolume postProcessVolume;
    private ChromaticAberration chromaticAberration;
    public RadialBlurImageEffect radialBlurImageEffect;

    // Hit Effect Use
    private bool isHitEffectCoroutineOn;
    private Coroutine hitEffectCoroutine;
    private float conHitEffectTime;

    // Camera Shake Use
    [System.Serializable]
    public class ShakeNoiseInfo
    {
        public string type;
        public NoiseSettings noise;
    }
    public ShakeNoiseInfo[] shakeNoiseInfos;
    private bool isCameraShakeCoroutineOn;
    private Coroutine cameraShakeCoroutine;
    private float conAmplitude;

    // Slow Mode Use
    private bool isSlowModeCoroutineOn;
    private Coroutine slowModeCoroutine;
    [HideInInspector] public float speedAffectedBySlowMode;
    [HideInInspector] public float speedUnaffectedBySlowMode;

    private void Start()
    {
        if (virtualCamera != null)
            virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        if (postProcessVolume != null)
            chromaticAberration = postProcessVolume.profile.GetSetting<ChromaticAberration>();

        speedAffectedBySlowMode = 1;
        speedUnaffectedBySlowMode = 1;
    }


    public void HitEffect(float time, float magnitude)
    {
        if (isHitEffectCoroutineOn)
            StopCoroutine(hitEffectCoroutine);

        hitEffectCoroutine = StartCoroutine(HitEffectCoroutine(time, magnitude));
    }

    public void CameraShake(string type, float time, float amplitude, float frequency)
    {
        if (conAmplitude > amplitude)
            return;

        if (isCameraShakeCoroutineOn)
            StopCoroutine(cameraShakeCoroutine);

        cameraShakeCoroutine = StartCoroutine(CameraShakeCoroutine(type, time, amplitude, frequency));
    }

    public void SlowMode(float time, float speed)
    {
        if (speedAffectedBySlowMode < speed)
            return;

        if (isSlowModeCoroutineOn)
            StopCoroutine(slowModeCoroutine);

        slowModeCoroutine = StartCoroutine(SlowModeCoroutine(time, speed));
    }

    private IEnumerator HitEffectCoroutine(float time, float magnitude)
    {
        isHitEffectCoroutineOn = true;

        float value;

        while (conHitEffectTime < time)
        {
            value = conHitEffectTime / time;

            chromaticAberration.intensity.value = value * 1 * magnitude;
            radialBlurImageEffect.blurSize = value * 1.6f * magnitude;

            conHitEffectTime += Time.deltaTime * speedUnaffectedBySlowMode;
            yield return null;
        }
        chromaticAberration.intensity.value = 1 * magnitude;
        radialBlurImageEffect.blurSize = 1.5f * magnitude;
        conHitEffectTime = time;



        while (conHitEffectTime > 0)
        {
            value = conHitEffectTime / time;

            chromaticAberration.intensity.value = value * 1 * magnitude;
            radialBlurImageEffect.blurSize = value * 1.5f * magnitude;

            conHitEffectTime -= Time.deltaTime * speedUnaffectedBySlowMode;
            yield return null;
        }
        chromaticAberration.intensity.value = 0;
        radialBlurImageEffect.blurSize = 0;
        conHitEffectTime = 0;

        isHitEffectCoroutineOn = false;
    }

    private IEnumerator CameraShakeCoroutine(string type, float time, float amplitude, float frequency)
    {
        isCameraShakeCoroutineOn = true;

        foreach(ShakeNoiseInfo shakeNoiseInfo in shakeNoiseInfos)
        {
            if(shakeNoiseInfo.type.Equals(type))
            {
                virtualCameraNoise.m_NoiseProfile = shakeNoiseInfo.noise;
                break;
            }
        }
        conAmplitude = amplitude;

        // 점점 카메라가 흔들어진다.
        float conTime = 0;
        while (conTime < time * 0.5f)
        {
            virtualCameraNoise.m_AmplitudeGain = amplitude * conTime * 2;
            virtualCameraNoise.m_FrequencyGain = frequency * conTime * 2;

            conTime += Time.deltaTime * speedUnaffectedBySlowMode;
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

            conTime -= Time.deltaTime * speedUnaffectedBySlowMode;
            yield return null;
        }
        virtualCameraNoise.m_AmplitudeGain = 0;
        virtualCameraNoise.m_FrequencyGain = 0;
        conAmplitude = 0;

        isCameraShakeCoroutineOn = false;
    }

    private IEnumerator SlowModeCoroutine(float time, float speed)
    {
        isSlowModeCoroutineOn = true;

        Time.timeScale = speed;
        speedAffectedBySlowMode = speed;
        speedUnaffectedBySlowMode = 1 / speed;

        float conTime = 0;
        while(conTime < time)
        {
            conTime += Time.deltaTime * speedUnaffectedBySlowMode;
            yield return null;
        }

        Time.timeScale = 1;
        speedAffectedBySlowMode = 1;
        speedUnaffectedBySlowMode = 1;

        isSlowModeCoroutineOn = false;
    }

}
