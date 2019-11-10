using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;

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

    [Header("For Effect System")]
    public CinemachineBrain cinemachineBrain;
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    public PostProcessVolume postProcessVolume;
    private ChromaticAberration chromaticAberration;
    private ColorGrading colorGrading;
    public RadialBlurImageEffect radialBlurImageEffect;
    public RectTransform worldSpaceCanvas;

    [Header("Timeline")]
    public PlayableDirector playableDirector;
    public TimelineAsset startTimeline;

    [Header("Useful Object Caching")]
    public PlayerScript player;
    public GameObject IngameUI;

    [Header("Die Process")]
    public GameObject diePanel;
    public WavyTextEffect wavyTextEffect;
    public ScalingTextEffect scalingTextEffect;

    [Header("Clear Process")]
    public GameObject clearPanel;
    public WavyTextEffect clearWavyTextEffect;
    public ScalingTextEffect clearScalingTextEffect;

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
    [Header("Shake Noise Caching")]
    public ShakeNoiseInfo[] shakeNoiseInfos;
    private bool isCameraShakeCoroutineOn;
    private Coroutine cameraShakeCoroutine;
    private float conAmplitude;

    // Slow Mode Use
    private bool isSlowModeCoroutineOn;
    private Coroutine slowModeCoroutine;
    [HideInInspector] public float speedAffectedBySlowMode;
    [HideInInspector] public float speedUnaffectedBySlowMode;

    // Color Grading
    private bool changeColorGradingCoroutineOn;
    private Coroutine changeColorGradingCoroutine;

    // 시점 벡터
    [HideInInspector] public Vector3 forward;
    [HideInInspector] public Vector3 left;

    private void Start()
    {
        if (virtualCamera != null)
            virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        if (postProcessVolume != null)
        {
            chromaticAberration = postProcessVolume.profile.GetSetting<ChromaticAberration>();
            colorGrading = postProcessVolume.profile.GetSetting<ColorGrading>();
        }

        // 마우스 커서 삭제
        InvisibleMouseCursor();

        speedAffectedBySlowMode = 1;
        speedUnaffectedBySlowMode = 1;

        StartCoroutine(StartGame());
    }

    // =============================================================================================================== public function ===============================================================================================================

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

    public void ChangeColorGrading(float value)
    {
        if (changeColorGradingCoroutineOn)
            StopCoroutine(changeColorGradingCoroutine);

        changeColorGradingCoroutine = StartCoroutine(ChangeColorGradingCoroutine(value));
    }
    
    public void RotateViewVector(bool instantRotate)
    {
        StartCoroutine(RotateViewVectorCoroutine(instantRotate));
    }

    public void InvisibleMouseCursor()
    {
        // Mouse Lock
        Cursor.lockState = CursorLockMode.Locked;

        // Cursor visible
        Cursor.visible = false;
    }

    public void VisivbleMouseCursor()
    {
        // Mouse Lock
        Cursor.lockState = CursorLockMode.None;

        // Cursor visible
        Cursor.visible = true;
    }

    public void DiePanelOn()
    {
        StartCoroutine(DieCoroutine(1));
    }

    public void ClearPanelOn()
    {
        StartCoroutine(ClearCoroutine(1));
    }

    public void OnClickReturnBtn()
    {
        SceneManager.LoadScene("MainScene");
    }

    // =============================================================================================================== private function ===============================================================================================================

    private IEnumerator StartGame()
    {
        // 입력 무시
        InputManager.Instance.canInput = false;

        // UI 삭제
        IngameUI.SetActive(false);

        // 시작 영상 플레이
        playableDirector.Play(startTimeline);
        while (playableDirector.state == PlayState.Paused)
            yield return null;

        // 시작 영상이 끝날때까지 대기
        while (playableDirector.state == PlayState.Playing)
            yield return null;

        // 설계 미스로, 이 작업이 이루어져야 캐릭터의 이동이 가능합니다.
        PlayerScript[] players = FindObjectsOfType<PlayerScript>();
        foreach (PlayerScript player in players)
            player.animator.applyRootMotion = true;

        // UI 표시
        IngameUI.SetActive(true);

        // 입력 가능
        InputManager.Instance.canInput = true;

        // 첫 라운드 시작
        RoundManager.Instance.RoundStart(0);
        RotateViewVector(true);
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

    private IEnumerator ChangeColorGradingCoroutine(float value)
    {
        changeColorGradingCoroutineOn = true;

        float conTime = 0;
        float maxTime = 1;

        float startValue = colorGrading.saturation.value;

        while (conTime < maxTime)
        {
            colorGrading.saturation.value = Mathf.Lerp(startValue, value, conTime);

            conTime += Time.deltaTime;
            yield return null;
        }

        changeColorGradingCoroutineOn = false;
    }

    private IEnumerator RotateViewVectorCoroutine(bool instantRotate)
    {
        Transform cameraTrans = Camera.main.transform;

        if(instantRotate)
        {
            forward = new Vector3(cameraTrans.forward.x, 0, cameraTrans.forward.z).normalized;
            left = new Vector3(cameraTrans.right.x, 0, cameraTrans.right.z).normalized * -1;

            worldSpaceCanvas.rotation = cameraTrans.rotation;
        }
        else
        {
            // 카메라 회전이 시작될때까지 대기
            while (!cinemachineBrain.IsBlending)
                yield return null;

            // 카메라가 회전되면..
            while (cinemachineBrain.IsBlending)
            {
                // 회전에 맞춰서 시점 벡터를 변환
                forward = new Vector3(cameraTrans.forward.x, 0, cameraTrans.forward.z).normalized;
                left = new Vector3(cameraTrans.right.x, 0, cameraTrans.right.z).normalized * -1;

                // World Space Canvas도 회전
                worldSpaceCanvas.rotation = cameraTrans.rotation;

                yield return null;
            }

            forward = new Vector3(cameraTrans.forward.x, 0, cameraTrans.forward.z).normalized;
            left = new Vector3(cameraTrans.right.x, 0, cameraTrans.right.z).normalized * -1;

            worldSpaceCanvas.rotation = cameraTrans.rotation;
        }
    }

    private IEnumerator DieCoroutine(float speed)
    {
        // UI 삭제
        IngameUI.SetActive(false);

        diePanel.SetActive(true);
        
        CanvasGroup temp = diePanel.GetComponent<CanvasGroup>();
        for (float conTime = 0; conTime < 1; conTime += Time.deltaTime * speed)
        {
            temp.alpha = conTime;
            yield return null;
        }
        temp.alpha = 1;

        VisivbleMouseCursor();

        wavyTextEffect.OnWave();
        scalingTextEffect.OnScaling();
    }

    private IEnumerator ClearCoroutine(float speed)
    {
        // UI 삭제
        IngameUI.SetActive(false);

        // Clear 패널 실행
        clearPanel.SetActive(true);
        CanvasGroup temp = clearPanel.GetComponent<CanvasGroup>();
        for (float conTime = 0; conTime < 1; conTime += Time.deltaTime * speed)
        {
            temp.alpha = conTime;
            yield return null;
        }
        temp.alpha = 1;

        // 마우스 커서 표시
        VisivbleMouseCursor();

        // Clear 패널 내 UI On
        clearWavyTextEffect.OnWave();
        clearScalingTextEffect.OnScaling();
    }
}
