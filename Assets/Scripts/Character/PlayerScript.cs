using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 상태
public enum State { IDLE, MOVE, JUMP };

public class PlayerScript : MonoBehaviour, CharacterScript
{
    // 플레이어를 따라다닐 카메라
    private CameraScript cam;

    // 플레이어 객체 물리 관련 컴포넌트
    private CharacterController controller;

    // 플레이어 애니메이션
    private Animator animator;

    [Header("Camera Info")]
    public Transform cameraTrans;

    // 이동 벡터
    Vector3 moveVector;

    // 조작 가능 여부
    bool canControl = true;

    // 시스템적인 반복 jump 함수 호출과 유저의 연속 점프 방지를 위한 변수
    private bool isJumping = false;
    private bool isJumpEnd = false;

    [Header("Character Info")]
    public State state; // 캐릭터 상태
    public float conSpeed; // 현재속도
    public float maxSpeed; // 현재 최고속도
    public float moveAcceleration; // 가속 정도
    public float moveDeceleration; // 감속 정도
    public float jumpForce; // 점프력
    public float gravity; // 중력

    float resultGravity = 0;

    void Start()
    {
        cam = GameObject.Find("Camera").GetComponent<CameraScript>();

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

    }
    void Update()
    {

    }
    void FixedUpdate()
    {
        // 입력 검사
        GetInput();

        // 캐릭터 이동
        Move();

        // 캐릭터 회전
        Rotate();
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
        // 이동 정보 확인
        Vector3 frontVector = transform.forward * Input.GetAxis("Vertical");
        Vector3 rightVector = transform.right * Input.GetAxis("Horizontal");
        moveVector = (frontVector + rightVector).normalized;
        /*
         * 다음의 경우에서는 속도가 점점 내려간다.
         * 이동 키를 안누르고 있는 경우
         * 도약 후 착지하는 경우
        */
        if (moveVector.sqrMagnitude == 0 || isJumpEnd == true)
        {
            // 현재 속도가 점점 줄어든다.
            conSpeed -= moveDeceleration;
            if (conSpeed < 0)
                conSpeed = 0;
        }
        // 그 외는 이동
        else
        {
            // 현재 속도가 점점 늘어난다.
            conSpeed += moveAcceleration;
            if (conSpeed > maxSpeed)
                conSpeed = maxSpeed;


        }
        moveVector = moveVector * conSpeed * Time.deltaTime;
        // 이동 애니메이션 실행
        animator.SetFloat("Speed", conSpeed / maxSpeed);
        



        // 중력 계산
        if (isJumping)
            resultGravity -= gravity * Time.deltaTime;
        else
            resultGravity = -gravity * Time.deltaTime;

        //resultGravity = (conJump - gravity) * Time.deltaTime;
        Vector3 gravityVector = new Vector3(0, resultGravity, 0);






        Vector3 result = moveVector + gravityVector;
        controller.Move(result);
    }

    float cameraX;
    float characterY;
    public void Rotate()
    {
        // 현재 마우스 회전값 저장
        float xRot = Input.GetAxis("Mouse Y") * cam.ySensitivity * -1;
        float yRot = Input.GetAxis("Mouse X") * cam.xSensitivity;




        // 카메라 상하(임계값 내) 계산
        Quaternion cameraQuaternion = cameraTrans.localRotation * Quaternion.Euler(xRot, 0, 0);
  
        cameraX = cameraQuaternion.eulerAngles.x;
        if (cameraX > 180)
            cameraX = cameraQuaternion.eulerAngles.x % 360 - 360;

        if (cameraX > 20)
            cameraX = 20;
        else if (cameraX < -30)
            cameraX = -30;



        // 캐릭터 좌우 계산
        Quaternion characterQuaternion = transform.localRotation * Quaternion.Euler(0, yRot, 0);

        characterY = characterQuaternion.eulerAngles.y;
        if (characterY > 180)
            characterY = characterQuaternion.eulerAngles.y % 360 - 360;



        // 캐릭터 좌우 회전
        transform.localEulerAngles = new Vector3(0, characterY, 0);

        // 카메라 상하 회전
        cameraTrans.localEulerAngles = new Vector3(cameraX, 0, 0);
    }
    
    // ===================================================== private function ============================================================

    private void GetInput()
    {
        // 스페이스바 클릭
        if (Input.GetButtonDown("Jump"))
        {
            // 점프 가능한지 판별
            if (canControl)
            {
                // 조작불능 선언
                canControl = false;
                StartCoroutine(Jumping());
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
                
                // 점프 가속도 추가
                resultGravity = jumpForce;

                // 점프 판정
                isJumping = true;
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
}
