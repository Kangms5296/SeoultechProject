using UnityEngine;
using Cinemachine;

public class NextRoundTrigger : MonoBehaviour
{
    public bool isVerticalPhase;

    public int up_leftRoundIndex;
    public CinemachineVirtualCamera up_left;

    public int down_rightRoundIndex;
    public CinemachineVirtualCamera down_right;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 상하 이동
            if (isVerticalPhase)
            {
                // 플레이어가 아래로
                if (transform.position.z > other.transform.position.z)
                {
                    // 기존의 카메라의 우선도를 낮춘다.
                    SystemManager.Instance.virtualCamera.Priority = 20;

                    // 새로 등록된 시네머신의 우선도를 높인다.
                    SystemManager.Instance.virtualCamera = down_right;
                    SystemManager.Instance.virtualCamera.Priority = 30;

                    // 시점 벡터 변환
                    SystemManager.Instance.RotateViewVector(false);

                    // 새로운 Round 시작
                    RoundManager.Instance.RoundStart(down_rightRoundIndex);
                }
                // 플레이어가 위로
                else
                {
                    // 기존의 카메라의 우선도를 낮춘다.
                    SystemManager.Instance.virtualCamera.Priority = 20;

                    // 새로 등록된 시네머신의 우선도를 높인다.
                    SystemManager.Instance.virtualCamera = up_left;
                    SystemManager.Instance.virtualCamera.Priority = 30;

                    // 시점 벡터 변환
                    SystemManager.Instance.RotateViewVector(false);

                    // 새로운 Round 시작
                    RoundManager.Instance.RoundStart(up_leftRoundIndex);
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

                    // 시점 벡터 변환
                    SystemManager.Instance.RotateViewVector(false);

                    // 새로운 Round 시작
                    RoundManager.Instance.RoundStart(up_leftRoundIndex);
                }
                // 플레이어가 오른쪽
                else
                {
                    // 기존의 카메라의 우선도를 낮춘다.
                    SystemManager.Instance.virtualCamera.Priority = 20;

                    // 새로 등록된 시네머신의 우선도를 높인다.
                    SystemManager.Instance.virtualCamera = down_right;
                    SystemManager.Instance.virtualCamera.Priority = 30;

                    // 시점 벡터 변환
                    SystemManager.Instance.RotateViewVector(false);

                    // 새로운 Round 시작
                    RoundManager.Instance.RoundStart(down_rightRoundIndex);
                }
            }
        }
    }
}
