using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cinemachine;

public class PlayerScript : MonoBehaviour, CharacterScript
{
    // 플레이어 컴포넌트
    private CharacterController controller;
    private Animator animator;

    [Header("Cinemachine")]
    public CinemachineVirtualCamera cm_Tps;
    public CinemachineVirtualCamera cm_Fps;
    
    // 외부 오브젝트
    [Header("Outside Obejct")]
    public Image runImage;                      // 달리기 게이지 부족 위험 효과를 위한 Image 컴포넌트 캐싱

    public RectTransform runGauge;              // 달리기에 사용되는 게이지 이미지
    private float conRunGauge = 1;

    public ParticleSystem runninghDustEffect;   // 달릴때, 혹은 점프 후 착지에 사용할 먼지 Particle
    private float conDustDelay = 0;
    private const float maxDustDelay = 0.18f;

    private Coroutine runningSignCoroutine;

    
    [Header("Character Info")]
    public float frontSpeed;                // 현재 전방 이동 속도
    public float backwardSpeed;             // 현재 후방 이동 속도
    public float sideSpeed;                 // 현재 옆 이동 속도

    public float walkSpeed;                 // 전방 기본 걷는 속도
    public float runSpeed;                  // 전방 기본 뛰는 속도

    public float moveAcceleration;          // 가속 정도
    public float moveDeceleration;          // 감속 정도

    public float jumpForce;                 // 점프력
    private bool isGroundAfterJump = true;  // 점프 후 착지 확인
    private bool canJump = true;            // 점프 가능 유무 확인
    private bool isJump = false;            // 현재 점프중인가

    public float gravity;                   // 중력 가속도
    private float resultGravity = 0;        // 현재 작용되는 중력

    private bool isMoving = false;          // 현재 이동중인가
    private bool canMoving = true;          // 현재 이동할 수 있는가
    private bool isDownShift;               // 현재 쉬프트를 누르는중인가

    private bool isVerticalMove = false;    // 좌우 이동 여부 확인
    private bool isLeft = false;            // 좌우 중 어느 이동인지 확인
    private float verticalSpeed = 0;        // 좌우 이동 힘 확인

    private bool isHorizontalMove = false;  // 상하 이동 여부 확인
    private bool isFront = false;           // 상하 중 어느 이동인지 확인
    private float horizontalSpeed = 0;      // 상하 이동 힘 확인

    private bool isAiming = false;          // 조준중인가?
    private Coroutine aimingCoroutine;      // 조준 애니메이션 코루틴


    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        StartCoroutine(IsGrounding());
    }

    void Update()
    {
        // 입력 검사
        GetInput();

        // 캐릭터 이동
        Move(); 
        
        // 캐릭터 회전
        Rotate();

        // 달리는 상황에서 게이지 감소
        Running();
    }

    // ===================================================== public function ============================================================

    public void Move()
    {
        if (canMoving)
        {
            // 앞뒤 이동계산
            if (Input.GetKey(KeyCode.W))
            {
                verticalSpeed += moveAcceleration * Time.deltaTime;
                if (verticalSpeed > frontSpeed)
                    verticalSpeed = frontSpeed;

                isFront = true;
                isVerticalMove = true;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                verticalSpeed -= moveAcceleration * Time.deltaTime;
                if (verticalSpeed < backwardSpeed * -1)
                    verticalSpeed = backwardSpeed * -1;

                isFront = false;
                isVerticalMove = true;
            }
            else
            {
                if (isFront)
                {
                    verticalSpeed -= moveDeceleration * Time.deltaTime;
                    if (verticalSpeed < 0)
                        verticalSpeed = 0;
                }
                else
                {
                    verticalSpeed += moveDeceleration * Time.deltaTime;
                    if (verticalSpeed > 0)
                        verticalSpeed = 0;
                }
                isVerticalMove = false;
            }

            // 좌우 이동계산
            if (Input.GetKey(KeyCode.A))
            {
                horizontalSpeed += moveAcceleration * Time.deltaTime * 1.5f;
                if (horizontalSpeed > sideSpeed)
                    horizontalSpeed = sideSpeed;

                isLeft = true;
                isHorizontalMove = true;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                horizontalSpeed -= moveAcceleration * Time.deltaTime * 1.5f;
                if (horizontalSpeed < sideSpeed * -1)
                    horizontalSpeed = sideSpeed * -1;

                isLeft = false;
                isHorizontalMove = true;
            }
            else
            {
                if (isLeft)
                {
                    horizontalSpeed -= moveDeceleration * Time.deltaTime * 1.5f;
                    if (horizontalSpeed < 0)
                        horizontalSpeed = 0;
                }
                else
                {
                    horizontalSpeed += moveDeceleration * Time.deltaTime * 1.5f;
                    if (horizontalSpeed > 0)
                        horizontalSpeed = 0;
                }

                isHorizontalMove = false;
            }
        }
        else
        {
            /*
            if (isFront)
            {
                verticalSpeed -= moveDeceleration * Time.deltaTime;
                if (verticalSpeed < 0)
                    verticalSpeed = 0;
            }
            else
            {
                verticalSpeed += moveDeceleration * Time.deltaTime;
                if (verticalSpeed > 0)
                    verticalSpeed = 0;
            }
            isVerticalMove = false;

            if (isLeft)
            {
                horizontalSpeed -= moveDeceleration * Time.deltaTime * 1.5f;
                if (horizontalSpeed < 0)
                    horizontalSpeed = 0;
            }
            else
            {
                horizontalSpeed += moveDeceleration * Time.deltaTime * 1.5f;
                if (horizontalSpeed > 0)
                    horizontalSpeed = 0;
            }

            isHorizontalMove = false;
            */
        }
        isMoving = isVerticalMove || isHorizontalMove;

        // 이동 속도에 따른 앞뒤 이동 벡터 계산
        Vector3 verticalVector = transform.forward * verticalSpeed;
        // 이동 속도에 따른 좌우 이동 벡터 계산
        Vector3 horizontalVector = transform.right * -1 * horizontalSpeed;
        // 벡터의 크기 계산
        float moveMaginitude = (verticalVector + horizontalVector).magnitude > runSpeed ? runSpeed : (verticalVector + horizontalVector).magnitude;
        // 최종 이동 벡터 계산
        Vector3 moveVector = (verticalVector + horizontalVector).normalized * moveMaginitude;


        // 앞뒤 이동에 따른 애니메이션 전환
        if (isFront)
            animator.SetFloat("Vertical", verticalSpeed / walkSpeed);
        else
            animator.SetFloat("Vertical", verticalSpeed / backwardSpeed);
        // 좌우 이동에 따른 애니메이션 전환
        animator.SetFloat("Horizontal", -horizontalSpeed / sideSpeed);


        // 중력 계산
        Vector3 gravityVector = new Vector3(0, resultGravity, 0);


        // 최종 값을 이용하여 이동
        Vector3 result = (moveVector + gravityVector);
        controller.Move(result * Time.deltaTime);
    }



    public void Rotate()
    {        // 현재 마우스 회전값 저장
        float xRot = Input.GetAxis("Mouse Y") * -1;
        float yRot = Input.GetAxis("Mouse X");

        // 마우스 이동x
        if (xRot == 0 && yRot == 0)
            return;

        // 캐릭터 좌우 회전(Y 값 회전)
        transform.Rotate(0, yRot * Time.deltaTime * 100, 0);

        /*
        // 캐릭터 좌우 회전(Y 값 회전)
        transform.Rotate(0, yRot * Time.deltaTime * cam.ySensitivity, 0);
        // 카메라 좌우 회전(Y 값 회전)
        Vector3 temp = camTrans.eulerAngles;
        camTrans.eulerAngles = new Vector3(temp.x, transform.eulerAngles.y, temp.z);

        // 카메라 상하 회전(X 값 회전)
        float cameraX = camTrans.eulerAngles.x;
        if (cameraX > 180)
            cameraX = camTrans.eulerAngles.x % 360 - 360;

        // 카메라의 X Rotation 값 최대 값 제한
        if (cameraX > 25 && xRot > 0)
        {

        }
        else if (cameraX < -75 && xRot < 0)
        {

        }
        else
        {
            // 그 외엔 회전
            camTrans.Rotate(xRot * Time.deltaTime * cam.xSensitivity, 0, 0);
        }
        */
    }


    public void Attack()
    {

    }

    public void Damaged()
    {

    }

    public void Running()
    {
        // isDownShift : 쉬프트를 누르는지 점검
        // isVerticalMove : 앞뒤 이동인가
        // isFront : 전방 이동인가
        if (isDownShift && isFront && isVerticalMove)
        {
            // 점점 최고 속도를 올린다.
            if (frontSpeed < runSpeed)
                frontSpeed += moveAcceleration * Time.deltaTime;
            else
                frontSpeed = runSpeed;

            // 점프중이 아니면..
            if (!isJump)
            {
                // 게이지 감소
                conRunGauge -= Time.deltaTime * 0.2f;
                if (conRunGauge < 0)
                {
                    conRunGauge = 0;

                    // 캐릭터가 힘들어서 걷는 속도로 늦춰진다.
                    isDownShift = false;
                }

                // 이동 먼지효과 재생
                conDustDelay += Time.deltaTime;
                if(conDustDelay > maxDustDelay)
                {
                    conDustDelay = 0;
                    runninghDustEffect.Emit(1);
                }
            }
        }
        else
        {
            // 점점 최고 속도를 내린다.
            if (frontSpeed > walkSpeed)
                frontSpeed -= moveDeceleration * Time.deltaTime;
            else
                frontSpeed = walkSpeed;

            // 게이지 증가
            if(isMoving)
                conRunGauge += Time.deltaTime * 0.1f;
            else
                conRunGauge += Time.deltaTime * 0.25f;

            if (conRunGauge > 1)
                conRunGauge = 1;
        }

        runGauge.localScale = new Vector3(conRunGauge, 1, 1);
        animator.SetFloat("Tired", 1 - conRunGauge);

        if (conRunGauge < 0.4f && runningSignCoroutine == null)
            runningSignCoroutine = StartCoroutine(RunningGaugeSign());
    }

    public void JumpStart()
    {
        // 점프간 점프입력 및 이동 금지
        canJump = false;
        canMoving = false;
        isJump = true;
        
        // 점프 후 땅에 착지하는지 확인
        isGroundAfterJump = false;

        // 어느정도 빠른 속도로 달리는 상황에서의 점프
        if (IsRunning())
        {
            animator.SetTrigger("JumpRunning");

            // 위로 힘을 준다.
            resultGravity += jumpForce * 1.3f;

            // 달리기 게이지가 줄어든다
            conRunGauge -= 0.1f;
            if (conRunGauge < 0)
                conRunGauge = 0;


        }
        // 멈춰있거나 걷는 상황에서의 점프
        else
        {
            // 점프 시작
            animator.SetTrigger("JumpStart");

            // 위로 힘을 준다.
            resultGravity += jumpForce;
        }
    }

    public void JumpEnd()
    {
        // 캐릭터 이동 가능
        canMoving = true;

        // 다시 점프 가능하도록 설정
        canJump = true;
        
        // 이동 애니메이션 실행
        animator.SetTrigger("Move");
    }

    // ===================================================== private function ============================================================
    

    // Key 입력 검사
    private void GetInput()
    {
        // 점프 키 클릭
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(canJump)
            {
                JumpStart();
            }
        }

        // Shift 키 버튼 다운
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isDownShift = true;
        }

        // Shift 키 버튼 업
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isDownShift = false;
        }

        // 마우스 우측 버튼 클릭
        if(Input.GetMouseButtonDown(1))
        {
            // 기존에 조준하고 있었으면
            if(isAiming)
            {
                // 조준 해제
                isAiming = !isAiming;

                // tps 모드의 카메라로 전환
                cm_Tps.Priority = 10;
                cm_Fps.Priority = 8;

                // 조준하는 애니메이션 코루틴 수행
                if (aimingCoroutine != null)
                    StopCoroutine(aimingCoroutine);
                aimingCoroutine = StartCoroutine(AimingEndCoroutine());
            }
            else
            {
                isAiming = !isAiming;

                cm_Tps.Priority = 8;
                cm_Fps.Priority = 10;

                // 애니메이션 수정
                if (aimingCoroutine != null)
                    StopCoroutine(aimingCoroutine);
                aimingCoroutine = StartCoroutine(AimingStartCoroutine());
            }
        }
    }

    private bool IsRunning()
    {
        if (verticalSpeed > runSpeed - 0.5f)
            return true;
        else
            return false;
    }

    private IEnumerator AimingStartCoroutine()
    {
        float temp = 0;
        while(temp < 1)
        {
            animator.SetLayerWeight(1, temp);

            temp += Time.deltaTime * 3;
            yield return null;
        }
        animator.SetLayerWeight(1, 1);
    }

    private IEnumerator AimingEndCoroutine()
    {
        float temp = 0;
        while (temp < 1)
        {
            animator.SetLayerWeight(1, 1 - temp);

            temp += Time.deltaTime * 3;
            yield return null;
        }
        animator.SetLayerWeight(1, 0);
    }

    private IEnumerator IsGrounding()
    {
        while(true)
        {
            // 매달려있는 상태


            // 땅에 착지한 상태
            if(controller.isGrounded)
            {
                // 상하 이동 없음
                resultGravity = 0;

                // 점프 후 착지한 경우
                if(!isGroundAfterJump)
                {
                    // 점프 종료를 알림
                    isJump = false;
                    isGroundAfterJump = true;

                    // 어느정도 빠른 속도이면 멈추지 않고 계속 달린다
                    if(IsRunning())
                    {
                        runninghDustEffect.Emit(3);
                        JumpEnd();

                        // 착지 애니메이션 실행
                        animator.SetTrigger("Move");
                    }
                    // 그 외는 점프 후 멈춘다.
                    else
                    {
                        runninghDustEffect.Emit(2);
                        verticalSpeed = 0;

                        JumpEnd();

                        // 착지 애니메이션 실행
                        animator.SetTrigger("Move");
                    }
                }
            }
            // 공중에 있는 상태
            else
            {
                // 아래로 내려감
                resultGravity -= gravity * Time.deltaTime;
            }
            yield return null;
        }
    }

    private IEnumerator RunningGaugeSign()
    {
        const float beforeX = 255.0f / 255;
        const float beforeY = 220.0f / 255;
        const float beforeZ = 160.0f / 255;

        const float afterX = 255.0f / 255;
        const float afterY = 100.0f / 255;
        const float afterZ = 100.0f / 255;

        float conTime = 0;
        float maxTime = 1f;

        // 경고로 붉은 이펙트
        while (conTime < maxTime)
        {
            conTime += Time.deltaTime * 3;

            runImage.color = new Color(beforeX * (1 - conTime) + afterX * conTime, beforeY * (1 - conTime) + afterY * conTime, beforeZ * (1 - conTime) + afterZ * conTime);
            yield return null;
        }
        conTime = 1;
        runImage.color = new Color(afterX, afterY, afterZ);

        // 다시 원래대로
        while (conTime > 0)
        {
            conTime -= Time.deltaTime * 3;

            runImage.color = new Color(beforeX * (1 - conTime) + afterX * conTime, beforeY * (1 - conTime) + afterY * conTime, beforeZ * (1 - conTime) + afterZ * conTime);
            yield return null;
        }
        conTime = 0;
        runImage.color = new Color(beforeX, beforeY, beforeZ);

        runningSignCoroutine = null;
    }
}
