using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public PlayerScript player;     // 플레이어 정보

    [Header("Managers")]
    public InputManager inputManager;

    [Header("ETC")]
    public GameObject ui;           // 게임 UI

    [Header("Timeline")]
    public PlayableDirector playableDirector;
    public TimelineAsset startTimeline;


    private static GameManager _instance = null;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                    Debug.LogError("There's no active GameManager object");
            }

            return _instance;
        }
    }

    private void Start()
    {
        // 마우스 커서 삭제
        InvisibleMouseCursor();

        StartCoroutine(StartGame());
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


    // ===================================================== public function ============================================================


    // ===================================================== private function ============================================================



    private IEnumerator StartGame()
    {
        // 입력 무시
        inputManager.canInput = false;

        // UI 삭제
        ui.SetActive(false);

        // 시작 영상 플레이
        playableDirector.Play(startTimeline);

        yield return new WaitForSeconds(0.5f);

        // 시작 영상이 끝날때까지 대기
        while(playableDirector.state == PlayState.Playing)
            yield return null;

        player.animator.applyRootMotion = true;

        // UI 표시
        ui.SetActive(true);

        // 입력 가능
        inputManager.canInput = true;

        // 첫 라운드 시작
        RoundManager.Instance.RoundStart(0);
        SystemManager.Instance.RotateViewVector(true);
    }


}
