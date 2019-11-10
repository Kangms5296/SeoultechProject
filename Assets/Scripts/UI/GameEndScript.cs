using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndScript : MonoBehaviour
{
    public GameObject gameEndPanel;

    public void OnClickESC()
    {
        // 조작 금지
        InputManager.Instance.canInput = false;

        // 화면 멈춤
        Time.timeScale = 0;

        // 마우스 커서 표시
        SystemManager.Instance.VisivbleMouseCursor();

        // 게임 종료 패널 표시
        gameEndPanel.SetActive(true);
    }

    public void OnClickYes()
    {
        // 게임 종료
        Application.Quit();
    }

    public void OnClickNo()
    {
        // 조작 가능
        InputManager.Instance.canInput = true;

        // 화면 다시 진행
        Time.timeScale = 1;

        // 마우스 커서 감춤
        SystemManager.Instance.InvisibleMouseCursor();

        // 게임 종료 패널 삭제
        gameEndPanel.SetActive(false);
    }

}
