using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainScript : MonoBehaviour
{
    // 자신을 소유한 플레이어
    public PlayerScript player;

    // 현재 Chain이 조준하고있는 방향
    public Transform chainShooterLookAt;
    private RaycastHit rayHit;
    private Ray ray;

    private void OnTriggerEnter(Collider other)
    {
        // Chain이 target에 닿은경우
        if(other.tag == "Target")
        {
            if (Physics.Raycast(chainShooterLookAt.position, chainShooterLookAt.forward, out rayHit, 200.0F))
            {
                Debug.Log('d');
                Vector3 temp = new Vector3(0, 0, 0);
                temp = rayHit.point;
            }
        }
    }
}
