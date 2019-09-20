using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cinemachine;

public enum Weapontype { None, Swing, Shooting, Throwing};

public class PlayerScript : MonoBehaviour, CharacterScript
{
    // 플레이어 컴포넌트
    private CharacterController controller;
    private Animator animator;

    [Header("Outside Object Caching")]
    public ParticleSystem dustParticle;     // 점프 후 착지에 사용할 먼지 Particle
    public MeleeWeaponTrail weaponTrail;    // 무기를 휘두를 때 나타나는 잔상 효과
    public Transform weaponParent;          // 플레이어가 들고있는 무기의 부모
    public Transform weaponReturn;          // 필드에 드랍되는 무기들의 부모
    public Transform notUsingWeaponParent;  // 플레이어가 등에 매고있는 무기들의 부모

    // 상태 변화 가능유무 2차배열
    private bool[,] CanChangeAction;
    private int conAction = 0;

    private int weaponChangeIndex;

    [Header("Character Info")]
    public float conMaxSpeed;                   // 현재 최대 속도
    public float walkMaxSpeed;                  // 최대 걷는 속도
    public float runMaxSpeed;                   // 최대 뛰는 속도
    private bool isMoving = false;              // 현재 이동중인가
    private bool canRotate = true;              // 이동간 회전이 가능한가

    public float moveAcceleration;              // 가속 정도
    public float moveDeceleration;              // 감속 정도
    
    public float jumpForce;                     // 점프력
    private bool isGroundAfterJump = true;      // 점프 후 착지 확인
    
    private bool isVerticalMove = false;        // 좌우 이동 여부 확인
    private bool isLeft = false;                // 좌우 중 어느 이동인지 확인
    private float verticalSpeed = 0;            // 좌우 이동 힘 확인

    private bool isHorizontalMove = false;      // 상하 이동 여부 확인
    private bool isFront = false;               // 상하 중 어느 이동인지 확인
    private float horizontalSpeed = 0;          // 상하 이동 힘 확인
    
    private float continuousAttackJudgmentConTime;          // 연속 공격 판정
    public float continuousAttackJudgmentMaxTime;           // 연속 공격 판정 최대 시간
    private bool canContinuousAttack = false;               // 연속 공격 판정 내 공격 버튼을 눌렀는가?
    private Coroutine continuousAttackJudgmentCoroutine;    // 연속 공격 판정 코루틴
    
    private Weapontype weaponType = Weapontype.None;        // 현재 사용하는 무기의 종류
    private bool isUsingWeaponTrail = false;                // 현재 사용하는 무기가 WeaponTrail을 사용하는가?

    public float moveMaginitude = 0;            // 이동 힘
    private Vector3 beforeMove  = Vector3.zero; // 이전 프레임에서의 이동 벡터
    private Vector3 newMove     = Vector3.zero; // 현재 프레임에서의 이동 벡터
    private Vector3 moveVector  = Vector3.zero; // 이전과 현재 프레임에서의 이동 벡터를 보간하여 얻은 최종 이동 벡터
    
    public float gravity;                       // 중력 가속도
    private float resultGravity = 0;            // 현재 작용되는 중력

    void Start()
    {
        CanChangeAction = new bool[7, 7]
        {
            //  Move         Jump        Attack     Running     Pick Up     weapon Change       Roll
            {   true,        true,       true ,     true,       true,       true,               true }, // Move     <= 현재 이동을 하는 중 ~이 가능한가?
            {   false,       false,      false,     false,      false,      true,               false}, // Jump     <= 현재 점프를 하는 중 ~이 가능한가?
            {   false,       false,      false,     false,      false,      false,              true},  // Attack   <= 현재 공격을 하는 중 ~이 가능한가?
            {   true,        false,      false,     true,       false,      true,               true},  // Running  <= 이 배열은 쓰이지 않는다.
            {   false,       false,      false,     false,      false,      false,              true},  // Pick Up
            {   true,        true,       false,     true,       false,      false,              true},  // Weapon Change
            {   false,      false,       false,     false,      false,      false,              false}, // Roll
        };

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        StartCoroutine(IsGrounding());
        StartCoroutine(ContinuousAttackJudgment());
    }

    
    void Update()
    {
        // 캐릭터 이동
        Moving();
    }

    // ===================================================== public function ============================================================

    public void Jump()
    {
        if (!CanChangeAction[conAction, 1])
            return;

        JumpStart();
    }

    public void Run()
    {
        if (!CanChangeAction[conAction, 3])
            return;

        if (!isMoving)
            return;

        Running();
    }

    public void Walk()
    {
        if (!CanChangeAction[conAction, 3])
            return;

        Walking();
    }

    public void Attack()
    {
        // 연속공격 판정시간 초기화
        continuousAttackJudgmentConTime = continuousAttackJudgmentMaxTime;
        if (continuousAttackJudgmentCoroutine == null)
            continuousAttackJudgmentCoroutine = StartCoroutine(ContinuousAttackJudgment());

        // 현재 상태에서 공격할 수 있으면..
        if (!CanChangeAction[conAction, 2])
            return;

        // 공격
        AttackStart();
    }

    public bool PickUp()
    {
        // 현재 아이템을 주울 수 있으면..
        if (!CanChangeAction[conAction, 4])
            return false;

        PickUpStart();

        return true;
    }

    public bool Disarm()
    {
        // 현재 아이템을 버릴 수 있으면...
        if (!CanChangeAction[conAction, 4])
            return false;

        // 현재 아이템을 들고있는경우만 가능
        if (weaponType == Weapontype.None)
            return false;

        DisarmStart();

        return true;
    }

    public bool WeaponChange(int index)
    {
        // 현재 무기 교체가 불가하면..
        if (!CanChangeAction[conAction, 5])
            return false;

        weaponChangeIndex = index;
        WeaponChangeStart();


        return true;
    }

    public void RollLeft()
    {
        // 현재 상태에서 공격할 수 있으면..
        if (!CanChangeAction[conAction, 6])
            return;

        RollStart(true, transform.right * -1);
    }

    public void RollRight()
    {
        // 현재 상태에서 공격할 수 있으면..
        if (!CanChangeAction[conAction, 6])
            return;

        RollStart(false, transform.right);
    }

    // ===================================================== private function ============================================================

    private void Moving()
    {
        if (CanChangeAction[conAction , 0])
        {
            // 앞뒤 이동계산
            if (Input.GetKey(KeyCode.W))
            {
                verticalSpeed += moveAcceleration;
                if (verticalSpeed > conMaxSpeed)
                    verticalSpeed = conMaxSpeed;

                isFront = true;
                isVerticalMove = true;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                verticalSpeed -= moveAcceleration;
                if (verticalSpeed < conMaxSpeed * -1)
                    verticalSpeed = conMaxSpeed * -1;

                isFront = false;
                isVerticalMove = true;
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
                isVerticalMove = false;
            }

            // 좌우 이동계산
            if (Input.GetKey(KeyCode.A))
            {
                horizontalSpeed += moveAcceleration;
                if (horizontalSpeed > conMaxSpeed)
                    horizontalSpeed = conMaxSpeed;

                isLeft = true;
                isHorizontalMove = true;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                horizontalSpeed -= moveAcceleration;
                if (horizontalSpeed < conMaxSpeed * -1)
                    horizontalSpeed = conMaxSpeed * -1;

                isLeft = false;
                isHorizontalMove = true;
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

                isHorizontalMove = false;
            }
        }
        isMoving = isHorizontalMove || isVerticalMove;



        // 이동 방향 계산
        newMove = Vector3.forward * verticalSpeed + Vector3.left * horizontalSpeed;
        moveVector = Vector3.Lerp(beforeMove, newMove, Time.smoothDeltaTime * 20);



        // 이동 크기 계산
        moveMaginitude = moveVector.magnitude > conMaxSpeed ? conMaxSpeed : moveVector.magnitude;
        animator.SetFloat("Speed", moveMaginitude / runMaxSpeed);



        // 이전 프레임과 반대 방향으로 방향을 바꾸면 순간적으로 큰 감속
        if (Vector3.Dot(beforeMove, newMove) < 0)
            moveMaginitude = moveMaginitude / 5;
        beforeMove = newMove;


        // 이동 방향으로 회전
        if (canRotate && isMoving)
        {
            Vector2 temp = new Vector2(horizontalSpeed, verticalSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, Quaternion.FromToRotation(Vector3.up, temp).eulerAngles.z, 0), Time.smoothDeltaTime * 20);
        }



        // 이동
        Vector3 moveResult = (moveVector.normalized * moveMaginitude + Vector3.up * resultGravity) * Time.smoothDeltaTime;
        controller.Move(moveResult);
    }

    private void AttackStart()
    {
        // 기존의 이동을 멈춘다.
        StopMove();

        // 무기 타입에 따라..
        switch (weaponType)
        {
            // 무기가 없다.(주먹질)
            case Weapontype.None:
                animator.SetTrigger("Punch");
                conAction = 2;

                // 그 외엔 조금 이동
                StartCoroutine(AttackMovingCoroutine(0.3f));

                break;


            // 몽둥이, 검 등의 무기
            case Weapontype.Swing:
                // 무기 잔상 효과 On
                WeaponTrailOn();

                // 공격 시 전방 이동
                if (IsRunning())
                {
                    animator.SetTrigger("Swing");
                    conAction = 2;

                    // 최고 속도로 달리는 상황이면 앞으로 돌진
                    StartCoroutine(AttackMovingCoroutine(4f));
                }
                else
                {
                    animator.SetTrigger("Swing");
                    conAction = 2;

                    // 그 외엔 조금 이동
                    StartCoroutine(AttackMovingCoroutine(0.5f));
                }
                break;
        }
    }

    private void AttackEnd()
    {
        if(canContinuousAttack)
        {
            // 무기 타입에 따라..
            switch (weaponType)
            {
                // 무기가 없다.(주먹질)
                case Weapontype.None:
                    animator.SetTrigger("Punch");

                    // 그 외엔 조금 이동
                    StartCoroutine(AttackMovingCoroutine(0.3f));

                    break;


                // 몽둥이, 검 등의 무기
                case Weapontype.Swing:
                    animator.SetTrigger("Swing");

                    // 그 외엔 조금 이동
                    StartCoroutine(AttackMovingCoroutine(0.5f));

                    break;
            }
        }
        else
        {
            animator.SetTrigger("Move");
            conAction = 0;

            if(isUsingWeaponTrail)
                WeaponTrailOff();
        }
    }
    

    private void Damaged()
    {

    }

    private void Running()
    {
        // 점점 최고 속도를 올린다.
        if (conMaxSpeed < runMaxSpeed)
            conMaxSpeed += moveAcceleration * Time.smoothDeltaTime * 20;
        else
            conMaxSpeed = runMaxSpeed;
    }

    private void Walking()
    {
        // 점점 최고 속도를 내린다.
        if (conMaxSpeed > walkMaxSpeed)
            conMaxSpeed -= moveDeceleration * Time.smoothDeltaTime * 20;
        else
            conMaxSpeed = walkMaxSpeed;
    }

    private void JumpStart()
    {
        // 점프간 이동 금지
        isVerticalMove = false;
        isHorizontalMove = false;

        // 점프 후 땅에 착지하는지 확인
        isGroundAfterJump = false;

        // 어느정도 빠른 속도로 달리는 상황에서의 점프
        if (IsRunning())
        {
            // 점프 시작
            animator.SetTrigger("JumpRunning");
            conAction = 1;

            // 위로 힘을 준다.
            resultGravity += jumpForce * 1.3f;
        }
        // 멈춰있거나 걷는 상황에서의 점프
        else
        {
            // 점프 시작
            animator.SetTrigger("JumpStart");
            conAction = 1;

            // 위로 힘을 준다.
            resultGravity += jumpForce;
        }
    }

    private void JumpEnd()
    {
        // 이동 애니메이션 실행
        animator.SetTrigger("Move");
        conAction = 0;
    }

    private void WeaponTrailOn()
    {
        isUsingWeaponTrail = true;
        weaponTrail.Emit = true;
    }

    private void WeaponTrailOff()
    {
        isUsingWeaponTrail = false;
        weaponTrail.Emit = false;
    }
    
    private void StopMove()
    {
        verticalSpeed = 0;
        isVerticalMove = false;
        horizontalSpeed = 0;
        isHorizontalMove = false;

        conMaxSpeed = walkMaxSpeed;
    }

    private bool IsRunning()
    {
        if (moveMaginitude == runMaxSpeed)
            return true;
        else
            return false;
    }

    private IEnumerator IsGrounding()
    {
        while(true)
        {
            // 땅에 착지한 상태
            if(controller.isGrounded)
            {
                // 상하 이동 없음
                resultGravity = 0;

                // 점프 후 착지한 경우
                if(!isGroundAfterJump)
                {
                    isGroundAfterJump = true;

                    // 어느정도 빠른 속도이면 멈추지 않고 계속 달린다
                    if(IsRunning())
                    {
                        dustParticle.Emit(3);

                        JumpEnd();
                    }
                    // 그 외는 점프 후 멈춘다.
                    else
                    {
                        dustParticle.Emit(2);

                        JumpEnd();
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

    IEnumerator AttackMovingCoroutine(float distance)
    {
        Vector3 initVector = transform.position;
        Vector3 goalVector = transform.position + transform.forward * distance;

        float conTime = 0;
        while (conTime < 1)
        {
            transform.position = Vector3.Lerp(initVector, goalVector, conTime);
            conTime += Time.deltaTime * 10;
            yield return null;
        }
        transform.position = goalVector;
    }

    IEnumerator ContinuousAttackJudgment()
    {
        canContinuousAttack = true;
        while (true)
        {
            if(continuousAttackJudgmentConTime <= 0)
                break;

            continuousAttackJudgmentConTime -= Time.deltaTime;
            yield return null;
        }
        canContinuousAttack = false;
        continuousAttackJudgmentCoroutine = null;
    }

    private void PickUpStart()
    {
        // 이동을 멈춘다.
        StopMove();
        
        DropObjectScript.isThereWeaponAroundPlayer = false;

        // 줍는 모션 실행
        animator.SetTrigger("PickUp");
        conAction = 4;
    }

    private void ChangeWeapon()
    {
        Transform weapon;

        // 현재 무기를 가지고 있다면
        if (weaponType != Weapontype.None)
        {
            // 무기를 바닥에 놓는다.
            weapon = weaponParent.GetChild(0);
            weapon.SetParent(weaponReturn);
            weapon.position = transform.position + transform.forward * 0.5f;
            weapon.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            weapon.GetComponent<WeaponScript>().ChangeToDrop();
        }

        // 무기 교체를 알려서, 공격 시 모션을 변화하게 한다.
        weapon = DropObjectScript.dropObject.transform;
        weaponType = DropObjectScript.dropObject.weaponType;
        if (weaponType == Weapontype.Swing)
            weaponTrail = weapon.Find("Equiped").GetComponent<MeleeWeaponTrail>();

        // 바닥에 있던 무기를 손으로 이동시킨다.
        weapon.SetParent(weaponParent);
        weapon.localPosition = Vector3.zero;
        weapon.localRotation = Quaternion.Euler(0, 0, 0);
        DropObjectScript.dropObject.ChangeToEquiped();

        // 'Click F' Text를 화면에서 지운다.
        WorldSpaceCanvasUIs.SetActive("Click F", false);
        Debug.Log("할렐루야!");
    }

    private void PickUpEnd()
    {
        // 이동
        animator.SetTrigger("Move");
        conAction = 0;
    }

    private void DisarmStart()
    {
        // 이동을 멈춘다.
        StopMove();

        // 줍는 모션 실행
        animator.SetTrigger("Disarm");
        conAction = 4;
    }

    private void DisarmWeapon()
    {
        Transform weapon;

        // 현재 무기를 가지고 있다면
        if (weaponType != Weapontype.None)
        {
            // 무기를 바닥에 놓는다.
            weapon = weaponParent.GetChild(0);
            weapon.SetParent(weaponReturn);
            weapon.position = transform.position + transform.forward * 0.5f;
            weapon.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            weapon.GetComponent<WeaponScript>().ChangeToDrop();
        }

        // 무기를 착용하기 전까지 주먹질을 한다.
        weaponType = Weapontype.None;
    }

    private void DisarmEnd()
    {
        // 이동
        animator.SetTrigger("Move");
        conAction = 0;
    }

    private void WeaponChangeStart()
    {
        // 무기 교체 모션 실행
        animator.SetTrigger("WeaponChange");
        conAction = 5;
    }

    private void RollStart(bool isLeft, Vector3 moveDirection)
    {
        // 이동을 멈춘다.
        StopMove();

        dustParticle.Emit(2);

        animator.SetTrigger("Roll");
        conAction = 6;

        // 구르기 모션 실행
        if (isLeft)
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y - 90, 0);
        else
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y + 90, 0);

        StartCoroutine(AttackMovingCoroutine(1));
    }

    private void RollEnd()
    {
        // 이동
        animator.SetTrigger("Move");
        conAction = 0;
    }

}
