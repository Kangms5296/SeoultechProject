using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Transform player;

    // 메인 카메라
    public Camera mainCamera;

    // 마우스 민감도
    public float xSensitivity = 1.2f;
    public float ySensitivity = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        transform.position = new Vector3(player.position.x, player.position.y + 1.3f, player.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        // 카메라는 캐릭터를 따라다닌다.
        //FollowPlayer();
    }



    // ===================================================== public function ============================================================

    public void FollowPlayer()
    {
        transform.position = new Vector3(player.position.x, player.position.y + 1.3f, player.position.z);
    }

}
