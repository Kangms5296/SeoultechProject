﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 전체 객체의 속도 제어를 위한 스피드 값
    public static float allSpeed;

    // Start is called before the first frame update
    void Start()
    {
        // 마우스 커서 삭제
        RemoveMouseCursor();

    }


    // ===================================================== public function ============================================================

    public void RemoveMouseCursor()
    {
        // Mouse Lock
        Cursor.lockState = CursorLockMode.Locked;

        // Cursor visible
        Cursor.visible = false;
    }



}
