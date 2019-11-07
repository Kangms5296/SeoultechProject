using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class GameManager : MonoBehaviour
{
    // 플레이어 정보
    public PlayerScript player;

    // 게임 UI
    public GameObject ui;

    // 시작 영상(타임라인
    public PlayableDirector playableDirector;
    public TimelineAsset startTimeline;


    // Start is called before the first frame update
    void Awake()
    {
        // 마우스 커서 삭제
        RemoveMouseCursor();
    }

    private void Start()
    {
        StartCoroutine(StartGame());
    }

    // ===================================================== public function ============================================================


    // ===================================================== private function ============================================================

    private void RemoveMouseCursor()
    {
        // Mouse Lock
        Cursor.lockState = CursorLockMode.Locked;

        // Cursor visible
        Cursor.visible = false;
    }

    private IEnumerator StartGame()
    {
        // UI 삭제
        ui.SetActive(false);

        // 시작 영상 플레이
        playableDirector.Play(startTimeline);

        yield return new WaitForSeconds(0.5f);

        // 시작 영상이 끝날때까지 대기
        while(playableDirector.state == PlayState.Playing)
            yield return null;

        player.animator.applyRootMotion = true;

        // UI 삭제
        ui.SetActive(true);

        // 첫 라운드 시작
        RoundManager.Instance.RoundStart(0);
        SystemManager.Instance.RotateViewVector(true);
    }


}
