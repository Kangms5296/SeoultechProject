using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour, CharacterScript
{
    // 플레이어를 따라다닐 카메라
    private CameraScript cam;

    // 플레이어 객체 물리 관련 컴포넌트
    private CharacterController controller;

    public Transform head;

    [Header("Character Info")]
    public float moveSpeed;
    public float rotateSpeed;


    
    void Start()
    {
        // 플레이어를 따라다닐 카메라 캐싱
        cam = GameObject.FindObjectOfType<CameraScript>();


        controller = GetComponent<CharacterController>();

    }
    void Update()
    {

    }
    void FixedUpdate()
    {
        // 캐릭터 이동
        CharacterMove();

        // 캐릭터 회전
        CharacterRotate();

        // 카메라 회전
        CameraRotate();
    }


    // ===================================================== public function ============================================================

    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void Damaged()
    {
        throw new System.NotImplementedException();
    }

    public void Move()
    {
    }

    // ===================================================== private function ============================================================

    private Vector3 CameraRotate()
    {
        // 현재 마우스 회전값 저장
        float xRot = Input.GetAxis("Mouse Y") * cam.ySensitivity * -1;
        float yRot = Input.GetAxis("Mouse X") * cam.xSensitivity;
        Quaternion cameraQuaternion = cam.transform.localRotation * Quaternion.Euler(xRot, yRot, 0);

        // 카메라 x값 계산
        float cameraX = cameraQuaternion.eulerAngles.x;
        if (cameraX > 180)
            cameraX = cameraQuaternion.eulerAngles.x % 360 - 360;

        // 카메라 x 임계값 지정
        if (cameraX < -60)
            cameraX = -60;
        else if (cameraX > 60)
            cameraX = 60;

        // 카메라 y값 계산
        float cameraY = cameraQuaternion.eulerAngles.y;
        if (cameraY > 180)
            cameraY = cameraQuaternion.eulerAngles.y % 360 - 360;

        // 카메라 y 임계값 지정
        if (cameraY < -90)
            cameraY = -90;
        else if (cameraY > 90)
            cameraY = 90;

        Vector3 result = new Vector3(cameraX, cameraY, 0);

        // 카메라 회전
        cam.transform.localEulerAngles = result;

        // 머리 회전
        head.eulerAngles = result + new Vector3(0, transform.eulerAngles.y, 0);

        return result;
    }

    private void CharacterRotate()
    {
        // 좌우로 회전중이 아니면 바로 종료
        if (Input.GetAxis("Horizontal") == 0)
            return;
        transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime);

    }

    private void CharacterMove()
    {
        // 앞뒤로 이동중이 아니면 바로 종료
        if (Input.GetAxis("Vertical") == 0)
            return;
        
        controller.Move(new Vector3(cam.mainCamera.transform.forward.x, 0, cam.mainCamera.transform.forward.z) * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
        
    }

}
