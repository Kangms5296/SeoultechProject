﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    // 전체 객체의 속도 제어를 위한 스피드 값
    public static float allSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        // 마우스 커서 삭제
        RemoveMouseCursor();

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



}
