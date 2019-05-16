﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerScript : MonoBehaviour, CharacterScript
{
    // 플레이어를 따라다닐 카메라
    private CameraScript cam;
    private Transform camTrans;

    // 플레이어 객체 물리 관련 컴포넌트
    private CharacterController controller;

    // 플레이어 애니메이션
    private Animator animator;

    // 조작 가능 여부
    bool canControl = true;

    // Move 관련 벡터
    float verticalSpeed;
    float horizontalSpeed;
    bool isFront = false;
    bool isLeft = false;
    Vector3 moveVector;

    // 점프 관련 변수
    private bool isJumping = false;
    private bool isJumpEnd = false;

    // Aiming 관련 변수
    private bool canAiming = true;
    private bool isAiming = false;
    private Coroutine aimingCoroutine;
    private Transform aimingTarget;

    // 사격 관련 변수
    private bool canShoot = false;

    [Header("Character Info")]
    public float conSpeed; // 현재속도
    public float maxSpeed; // 최고 속도
    public float moveAcceleration; // 가속 정도
    public float moveDeceleration; // 감속 정도
    public float jumpForce; // 점프력
    public float gravity; // 중력

    [Header("Character Parts")]
    public Transform spine; // 조준 시 회전할 캐릭터의 상체

    private float resultGravity = 0;

    void Start()
    {
        cam = GameObject.Find("Camera").GetComponent<CameraScript>();
        camTrans = cam.GetComponent<Transform>();

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        // 입력 검사
        GetInput();

        // 캐릭터 이동
        Move(); 
        
        // 캐릭터 회전
        Rotate();
    }
    private void LateUpdate()
    {
        if (canShoot)
        {
            spine.eulerAngles = new Vector3(camTrans.eulerAngles.x  + -7.305f, transform.eulerAngles.y, 0);
        }
    }

    // ===================================================== public function ============================================================

    public void Attack()
    {

    }

    public void Damaged()
    {

    }

    public void Move()
    {
        // 점프 착지 직후에는 멈춤
        if (isJumpEnd == true)
        {
            if (isFront)
            {
                verticalSpeed -= moveDeceleration * 5;
                if (verticalSpeed < 0)
                    verticalSpeed = 0;

            }
            else
            {
                verticalSpeed += moveDeceleration * 5;
                if (verticalSpeed > 0)
                    verticalSpeed = 0;
            }
            if (isLeft)
            {
                horizontalSpeed -= moveDeceleration * 5;
                if (horizontalSpeed < 0)
                    horizontalSpeed = 0;
            }
            else
            {
                horizontalSpeed += moveDeceleration * 5;
                if (horizontalSpeed > 0)
                    horizontalSpeed = 0;
            }
        }
        // 점프를 하는 중간에는 방향을 바꿀 수 없음
        else if (isJumping)
        {

        }
        // 조준 중에는 서서히 이동을 멈춤
        else if(isAiming)
        {
            if (isFront)
            {
                verticalSpeed -= moveDeceleration * 3;
                if (verticalSpeed < 0)
                    verticalSpeed = 0;

            }
            else
            {
                verticalSpeed += moveDeceleration * 3;
                if (verticalSpeed > 0)
                    verticalSpeed = 0;
            }
            if (isLeft)
            {
                horizontalSpeed -= moveDeceleration * 3;
                if (horizontalSpeed < 0)
                    horizontalSpeed = 0;
            }
            else
            {
                horizontalSpeed += moveDeceleration * 3;
                if (horizontalSpeed > 0)
                    horizontalSpeed = 0;
            }
        }
        else
        {
            // 앞뒤 이동계산
            if (Input.GetKey(KeyCode.W))
            {
                verticalSpeed += moveAcceleration;
                if (verticalSpeed > maxSpeed)
                    verticalSpeed = maxSpeed;

                isFront = true;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                verticalSpeed -= moveAcceleration;
                if (verticalSpeed < maxSpeed * -1 * 0.4f)
                    verticalSpeed = maxSpeed * -1 * 0.4f;

                isFront = false;
            }
            else
            {
                if (isFront)
                {
                    verticalSpeed -= moveDeceleration;
                    if (verticalSpeed < 0)
                        verticalSpeed = 0;

                }
                else
                {
                    verticalSpeed += moveDeceleration;
                    if (verticalSpeed > 0)
                        verticalSpeed = 0;
                }
            }

            // 좌우 이동계산
            if (Input.GetKey(KeyCode.A))
            {
                horizontalSpeed += moveAcceleration;
                if (horizontalSpeed > maxSpeed * 0.4f)
                    horizontalSpeed = maxSpeed * 0.4f;

                isLeft = true;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                horizontalSpeed -= moveAcceleration;
                if (horizontalSpeed < maxSpeed * -1 * 0.4f)
                    horizontalSpeed = maxSpeed * -1 * 0.4f;

                isLeft = false;
            }
            else
            {
                if (isLeft)
                {
                    horizontalSpeed -= moveDeceleration;
                    if (horizontalSpeed < 0)
                        horizontalSpeed = 0;
                }
                else
                {
                    horizontalSpeed += moveDeceleration;
                    if (horizontalSpeed > 0)
                        horizontalSpeed = 0;
                }
            }
        }


        // 이동 속도에 따른 앞뒤 이동 벡터 계산
        Vector3 verticalVector = transform.forward * verticalSpeed * Time.deltaTime;
        // 이동 속도에 따른 좌우 이동 벡터 계산
        Vector3 horizontalVector = transform.right * -1 * horizontalSpeed * Time.deltaTime;
        // 최종 이동 벡터 계산
        moveVector = (verticalVector + horizontalVector);
        //moveVector = new Vector3(-1 * horizontalSpeed , 0, verticalSpeed).normalized;
        //transform.Translate(moveVector);


        // 이동 애니메이션 실행
        if (Mathf.Abs(verticalSpeed) >= Mathf.Abs(horizontalSpeed))
            conSpeed = Mathf.Abs(verticalSpeed);
        else
            conSpeed = Mathf.Abs(horizontalSpeed);
        animator.SetFloat("Speed", (conSpeed / maxSpeed) * 0.5f);


        // 중력 계산
        if (isJumping)
            resultGravity -= gravity * Time.deltaTime;
        else
            resultGravity = -gravity;
        Vector3 gravityVector = new Vector3(0, resultGravity, 0);


        Vector3 result = moveVector + gravityVector;
        controller.Move(result);
    }


    public void Rotate()
    {
        // 현재 조준 자세를 취하고 있는 상황이면 회전 불가
        if (isAiming == true && canAiming == false)
            return;

        // 현재 마우스 회전값 저장
        float xRot = Input.GetAxis("Mouse Y") * -1;
        float yRot = Input.GetAxis("Mouse X");

        // 마우스 이동x
        if (xRot == 0 && yRot == 0)
            return;

        
        // 캐릭터 좌우 회전(Y 값 회전)
        transform.Rotate(0, yRot * Time.deltaTime * cam.ySensitivity, 0);
        // 카메라 좌우 회전(Y 값 회전)
        Vector3 temp = camTrans.eulerAngles;
        camTrans.eulerAngles = new Vector3(temp.x, transform.eulerAngles.y, temp.z);
        
        // 카메라 상하 회전(X 값 회전)
        float cameraX = camTrans.eulerAngles.x;
        if (cameraX > 180)
            cameraX = camTrans.eulerAngles.x % 360 - 360;

        // 현재 카메라의 X Rotation 값에 따라 다른 처리
        if (cameraX > 25 && xRot > 0)
        {

        }
        else if (cameraX < -30 && xRot < 0)
        {

        }
        else
            // 그 외엔 회전
            camTrans.Rotate(xRot * Time.deltaTime * cam.xSensitivity, 0, 0);
    }

    // ===================================================== private function ============================================================

    // Key 입력 검사
    private void GetInput()
    {
        // 키보드 입력 확인
        if (Input.anyKey)
        {
            if (canControl)
            {
                // 점프 키 클릭
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // 조작불능 선언
                    canControl = false;
                    StartCoroutine(Jumping());
                }
            }
        }
        // 마우스 좌클릭 이벤트
        if (Input.GetMouseButtonDown(0))
        {
            // 조준 중인 위치로 Chain 발사
            if(isAiming)
            {

            }
            
        }
        // 마우스 우클릭 이벤트
        if (Input.GetMouseButtonDown(1))
        {
            if (canAiming)
            {
                // 점프 중이면 불가능
                if (isJumping)
                    return;

                // 조준 정리가 끝날 때까지 다시 조준하는 행위를 금지 설정
                canAiming = false;

                // 조준 해제
                if (isAiming)
                {
                    // 조준 종료
                    if (aimingCoroutine != null)
                        StopCoroutine(aimingCoroutine);
                    aimingCoroutine = StartCoroutine(AimingEnd());
                }
                // 조준 시작
                else
                {   
                    // 조준 시작
                    if (aimingCoroutine != null)
                        StopCoroutine(aimingCoroutine);
                    aimingCoroutine = StartCoroutine(AimingStart());
                }
            }
        }
    }

    private IEnumerator Jumping()
    {
        // 점프 시작
        while (true)
        {
            // 땅에서 이동중
            if (controller.isGrounded == true && isJumping == false)
            {
                // 점프 애니메이션 실행
                animator.SetTrigger("Jump_Start");

                // 점프 판정
                isJumping = true;

                // 점프 가속도 추가
                resultGravity = jumpForce;


            }
            // 점프중
            else if (controller.isGrounded == false && isJumping == true)
            {

            }
            // 점프 이후 지면에 착지
            else if (controller.isGrounded == true && isJumping == true)
            {
                // 점프 완료 애니메이션 실행
                animator.SetTrigger("Jump_End");

                // 점프 완료상태로 이동속도가 일시적으로 느려지도록 설정
                isJumpEnd = true;
                conSpeed = 0;

                yield return new WaitForSeconds(0.5f);
                isJumpEnd = false;

                // 이동 애니메이션 실행(점프End 애니메이션 종료)
                animator.SetTrigger("Move");

                // 점프 종료
                isJumping = false;

                // 조작 불능 해제
                canControl = true;

                

                break;
            }
            else
            {
                Debug.Log("???");
            }

            yield return null;
        }
    }

    private void Aiming()
    {
        float xRot = Input.GetAxis("Mouse Y") * -1;
        Debug.Log(1);
        // 조준 준비가 완료된 상태이면 마우스 이동에 따라 캐릭터 상체를 회전시켜서 조준

        float spineX = spine.eulerAngles.x;
        if (spineX > 180)
            spineX = camTrans.eulerAngles.x % 360 - 360;

        // 현재 카메라의 X Rotation 값에 따라 다른 처리
        if (spineX > 25 && xRot > 0)
            // 카메라 X Rotation의 최대값
            spineX = 25;
        else if (spineX < -30 && xRot < 0)
            // 카메라 X Rotation의 최소값
            spineX = -30;
        else
            // 그 외엔 회전
            spine.Rotate(xRot * Time.deltaTime * cam.xSensitivity, 0, 0);

    }

    // 조준 시작 코루틴
    private IEnumerator AimingStart()
    {
        // 조준 행위 이외의 다른 행동을 금지
        canControl = false;

        // 조준 시작 설정
        isAiming = true;

        // 조준 시작 애니메이션 실행
        animator.SetTrigger("Aim");

        // 카메라 연출 가능하도록 설정
        cam.SetFollow(false);

        // 카메라 연출 시작(1초에 걸쳐 천천히 조준위치로 이동)
        float conTime = 0;
        float maxTime = 0.5f;
        Vector3 cameraPos_Aiming = new Vector3(0.6f, 0.8f, -0.8f);
        while(conTime < maxTime)
        {
            camTrans.position = Vector3.Lerp(camTrans.position, transform.position, conTime);
            camTrans.rotation = Quaternion.Lerp(camTrans.rotation, transform.rotation, conTime);
            Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, cameraPos_Aiming, conTime);
            conTime += Time.deltaTime;
            yield return null;
        }
        camTrans.position = transform.position;
        camTrans.rotation = transform.rotation;
        Camera.main.transform.localPosition = cameraPos_Aiming;

        // 이제부터 좌클릭 시 사격 가능
        canShoot = true;

        // 다시 우클릭을 하면 조준을 해제하도록 지정
        canAiming = true;
    }

    // 조준 종료 코루틴
    private IEnumerator AimingEnd()
    {
        // 이제부터 좌클릭 시 사격 불가능
        canShoot = false;

        // 조준 종료 애니메이션 실행
        animator.SetTrigger("Move");

        // 카메라 연출
        float conTime = 0;
        float maxTime = 0.5f;
        Vector3 cameraPos_Script = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        Vector3 cameraPos_Follwing = new Vector3(0, 0, -2.5f);
        while (conTime < maxTime)
        {
            camTrans.position = Vector3.Lerp(camTrans.position, cameraPos_Script, conTime);
            camTrans.rotation = Quaternion.Lerp(camTrans.rotation, transform.rotation, conTime);
            Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, cameraPos_Follwing, conTime);
            conTime += Time.deltaTime;
            yield return null;
        }
        camTrans.position = cameraPos_Script;
        camTrans.rotation = transform.rotation;
        Camera.main.transform.localPosition = cameraPos_Follwing;

        // 카메라가 다시 플레이어를 따라다니도록 설정
        cam.SetFollow(true);

        // 조준 종료 기록
        isAiming = false;

        // 다시 우클릭을 하면 조준을 해제하도록 지정
        canAiming = true;

        // 조준 행위 이외의 다른 행동을 금지를 해제
        canControl = true;
    }

}
