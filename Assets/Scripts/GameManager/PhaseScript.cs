using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PhaseScript : MonoBehaviour
{
    public bool isVerticalPhase;
    public CinemachineVirtualCamera up_left;
    public CinemachineVirtualCamera down_right;

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // 상하 이동
            if(isVerticalPhase)
            {
                // 플레이어가 아래로
                if(transform.position.z > other.transform.position.z)
                {
                    // 기존의 카메라의 우선도를 낮춘다.
                    SystemManager.Instance.virtualCamera.Priority = 20;

                    // 새로 등록된 시네머신의 우선도를 높인다.
                    SystemManager.Instance.virtualCamera = down_right;
                    SystemManager.Instance.virtualCamera.Priority = 30;
                }
                // 플레이어가 위로
                else
                {
                    // 기존의 카메라의 우선도를 낮춘다.
                    SystemManager.Instance.virtualCamera.Priority = 20;

                    // 새로 등록된 시네머신의 우선도를 높인다.
                    SystemManager.Instance.virtualCamera = up_left;
                    SystemManager.Instance.virtualCamera.Priority = 30;
                }
            }
            // 좌우 이동
            else
            {
                // 플레이어가 왼쪽
                if (transform.position.x > other.transform.position.x)
                {
                    // 기존의 카메라의 우선도를 낮춘다.
                    SystemManager.Instance.virtualCamera.Priority = 20;

                    // 새로 등록된 시네머신의 우선도를 높인다.
                    SystemManager.Instance.virtualCamera = up_left;
                    SystemManager.Instance.virtualCamera.Priority = 30;
                }
                // 플레이어가 오른쪽
                else
                {
                    // 기존의 카메라의 우선도를 낮춘다.
                    SystemManager.Instance.virtualCamera.Priority = 20;

                    // 새로 등록된 시네머신의 우선도를 높인다.
                    SystemManager.Instance.virtualCamera = down_right;
                    SystemManager.Instance.virtualCamera.Priority = 30;
                }
            }

            SystemManager.Instance.RotateViewVector();
        }
    }
}
