﻿using System.Collections;
using UnityEngine;
using Cinemachine;

public class PlayerScript : MonoBehaviour, ICharacterScript
{
    [Header("Player Component")]
    public CharacterController controller;
    public Animator animator;

    [Header("Outside Object Caching")]
    public ParticleSystem dustParticle;                         // 먼지 Particle
    public CrossHairScript crossHair;                           // 플레이어 조준 간 화면에 표시되는 crossHair
    public AttackHitAreaScript attackHitArea;                   // 근거리 공격을 할 때 사용하는 공격 범위 판정 Collider

    [Header("Weapon Parent Set")]
    public Transform handlingWeaponParent;                      // 플레이어가 들고있는 무기의 부모
    public Transform backWeaponParent;                          // 플레이어가 등에 매고있는 무기의 부모
    public Transform bellyWeaponParent;                         // 플레이어가 배에 걸고있는 무기의 부모
    public Transform droppedWeaponParent;                       // 필드에 드랍되는 무기들의 부모
    public Transform cantUseWeaponParent;                       // 파괴된 무기들의 부모

    [Header("Using Script Caching")]
    public PlayerSkillScript skillManager;                      // 플레이어 스킬 관련 스크립트
    public WeaponSlotManagerScript weaponSlotManager;           // 플레이어 무기 교체 관련 스크립트
    public MotionTrail motionTrail;                             // 고속 이동 간 사용할 Motion Trail 스크립트
    public PlayerHpBarScript hpBarScript;                       // 플레이어 Hp UI 관련 스크립트

    [Header("Audio")]
    public AudioSource moveAudio;                               // 이동 소리 Source
    public AudioSource weaponAudio;                             // 무기 소리 Source
    public AudioSource etcAudio;                                // 나머지 행동의 소리 Source
    public AudioClip damagedClip;
    public AudioClip punchClip;
    public AudioClip footstepClip;
    public AudioClip jumpClip;
    public AudioClip rollingClip;

    // 상태 변화 가능유무 2차배열
    private bool[,] CanChangeAction;
    private int conAction = 0;

    [Header("Character Info")]
    public int maxHp;                                           // 플레이어 최대 체력
    private int conHp;                                          // 플레이어 현재 체력

    public float conMaxSpeed;                                   // 현재 최대 속도
    public float walkMaxSpeed;                                  // 최대 걷는 속도
    public float runMaxSpeed;                                   // 최대 뛰는 속도
    private bool isMoving = false;                              // 현재 이동중인가

    public float moveAcceleration;                              // 가속 정도
    public float moveDeceleration;                              // 감속 정도
    
    public float jumpForce;                                     // 점프력
    private bool isGroundAfterJump = true;                      // 점프 후 착지 확인
    
    private bool isVerticalMove = false;                        // 좌우 이동 여부 확인
    private bool isLeft = false;                                // 좌우 중 어느 이동인지 확인
    private float verticalSpeed = 0;                            // 좌우 이동 힘 확인

    private bool isHorizontalMove = false;                      // 상하 이동 여부 확인
    private bool isFront = false;                               // 상하 중 어느 이동인지 확인
    private float horizontalSpeed = 0;                          // 상하 이동 힘 확인
    
    private float continuousAttackJudgmentConTime;              // 연속 공격 판정
    public float continuousAttackJudgmentMaxTime;               // 연속 공격 판정 최대 시간
    private bool canContinuousAttack = false;                   // 연속 공격 판정 내 공격 버튼을 눌렀는가?
    private bool isFinishAttackDelay = false;                   // 연속 공격 마지막 이후 딜레이인가?
    private Coroutine FinishAttackDelayCoroutine;               // 공격 종료 딜레이 코루틴

    public float moveMaginitude = 0;                            // 이동 힘
    private Vector3 beforeMove  = Vector3.zero;                 // 이전 프레임에서의 이동 벡터
    private Vector3 newMove     = Vector3.zero;                 // 현재 프레임에서의 이동 벡터
    private Vector3 moveVector  = Vector3.zero;                 // 이전과 현재 프레임에서의 이동 벡터를 보간하여 얻은 최종 이동 벡터

    private int conWeaponIndex;                                 // 현재 사용하는 무기의 slot 정보
    private Weapontype conWeaponType =  Weapontype.None;        // 현재 사용하는 무기의 종류
    private SwingWeaponScript           swingWeapon;
    private ShootingWeaponScript        shootingWeapon;
    private ThrowingWeaponScript        throwingWeapon;

    private bool isFocusMode = false;                           // 현재 집중모드를 사용중인가?

    private bool continuousShootingOn;                          // 연속 총기 공격 판정

    private bool isAttackMovingCoroutineRunning = false;        // 공격 간 전방 이동 코루틴 사용 유무
    private Coroutine attackMovingCoroutine;                    // 공격 간 전방 이동 코루틴

    public float gravity;                                       // 중력 가속도
    private float resultGravity = 0;                            // 현재 작용되는 중력

    [HideInInspector] public bool canTakeDamage = true;                          // 피격 판정

    void Start()
    {
        CanChangeAction = new bool[10, 10]
        {
            //  Move         Jump         Attack     Running     Pick Up     weapon Change       Roll        Focus Mode  Focue Mode Attack  Damaged
            {   true ,       true ,       true ,     true ,      true ,      true ,              true ,      true ,      false,             true }, // 0    Move     <= 현재 이동을 하는 중 ~이 가능한가?
            {   false,       false,       false,     false,      false,      false,              false,      false,      false,             true }, // 1    Jump     <= 현재 점프를 하는 중 ~이 가능한가?
            {   false,       false,       false,     false,      false,      false,              true ,      false,      false,             false}, // 2    Attack   <= 현재 공격을 하는 중 ~이 가능한가?
            {   true ,       false,       false,     true ,      false,      true ,              true ,      true ,      false,             true }, // 3    Running  <= 이 배열은 쓰이지 않는다.
            {   false,       false,       false,     false,      false,      false,              false,      false,      false,             true }, // 4    Pick Up
            {   true ,       false,       false,     true ,      false,      false,              false,      false,      false,             true }, // 5    Weapon Change
            {   false,       false,       false,     false,      false,      false,              false,      false,      false,             false}, // 6    Roll
            {   false,       false,       true ,     false,      false,      false,              true ,      true ,      true ,             false}, // 7    Focue Mode
            {   false,       false,       false,     false,      false,      false,              false,      false,      false,             false}, // 8    Can't  Everything
            {   false,       false,       false,     false,      false,      false,              true ,      false,      false,             false}, // 9    Damaged

        };

        conHp = maxHp;

        StartCoroutine(IsGrounding());
        StartCoroutine(ContinuousAttackJudgment());
    }

    private void Update()
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
        // 현재 공격 딜레이인가?
        if (isFinishAttackDelay)
            return;

        // 연속공격 판정시간 초기화
        continuousAttackJudgmentConTime = continuousAttackJudgmentMaxTime;
        if (canContinuousAttack == false)
            StartCoroutine(ContinuousAttackJudgment());

        // 현재 상태에서 공격할 수 있으면..
        if (!CanChangeAction[conAction, 2])
            return;
        
        // 공격
        AttackStart();
    }

    // 마우스 우클릭으로 사용하는 집중모드, 각 무기 종류 간 다른 기술 사용
    public void FocusMode()
    {
        // 집중모드로 변경할 수 있는가?
        if (!CanChangeAction[conAction, 7])
            return;

        if (isFocusMode)
        {
            isFocusMode = false;

            // 딜레이 코루틴 시작
            skillManager.Focus();

            // CrossHair 제거
            crossHair.Destroy();

            // 이동 시작
            MoveStart();
        }
        else
        {
            // 딜레이 중인가
            if (!skillManager.CanFicus())
                return;

            switch (conWeaponType)
            {
                case Weapontype.Swing:

                    crossHair.Init(transform.forward);

                    isFocusMode = true;

                    // 이동을 멈춘다.
                    StopMove();

                    // 돌진 방향 조준
                    Rush();

                    break;

                case Weapontype.Shooting:

                    crossHair.Init(transform.forward);

                    isFocusMode = true;
                                       
                    // 이동을 멈춘다.
                    StopMove();

                    // 사격 위치 조준
                    Aim();
                    break;

                case Weapontype.Throwing:

                    crossHair.Init(transform.forward);

                    isFocusMode = true;

                    // 이동을 멈춘다.
                    StopMove();

                    // 던질 위치 조준
                    ThrowingAim();
                    break;

            }
        }
    }


    public bool PickUp()
    {
        // 현재 무기를 주울 수 있으면..
        if (!CanChangeAction[conAction, 4])
            return false;

        // 플레이어 주변에 획득 가능한 무기가 있으면
        if (!DropObjectScript.isThereWeaponAroundPlayer)
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
        if (conWeaponType == Weapontype.None)
            return false;

        DisarmStart();

        return true;
    }

    public bool WeaponChange(int index)
    {
        // 현재 무기 교체가 불가하면..
        if (!CanChangeAction[conAction, 5])
            return false;

        // 기존에 들고있는 무기로 교체하려하면..
        if (conWeaponIndex == index)
            return false;
        conWeaponIndex = index;

        WeaponChangeStart();

        return true;
    }

    public void RollLeft()
    {
        // 현재 상태에서 구르기를 할 수 있으면..
        if (!CanChangeAction[conAction, 6])
            return;

        // 구르기 쿨타임 확인
        if (skillManager.Rolling())
            RollStart(true, transform.right * -1);
    }

    public void RollRight()
    {
        // 현재 상태에서 구르기를 할 수 있으면..
        if (!CanChangeAction[conAction, 6])
            return;

        // 구르기 쿨타임 확인
        if (skillManager.Rolling())
            RollStart(false, transform.right);
    }

    public void StopRush()
    {
        if (attackMovingCoroutine != null)
            StopCoroutine(attackMovingCoroutine);
    }

    public int RecoveryHp(int value)
    {
        conHp += value;
        if (conHp > maxHp)
            conHp = maxHp;

        // HP Ui를 갱신
        hpBarScript.RecoveryHp(conHp, maxHp);

        return conHp;
    }

    public int DecreaseHp(int value)
    {
        conHp -= value;
        if (conHp < 0)
            conHp = 0;

        // HP Ui를 갱신
        hpBarScript.DecreaseHp(conHp, maxHp);

        return conHp;
    }

    public void TakeDamage(int damage, Vector3 knockBackDirection, float distance)
    {
        if (!canTakeDamage)
            return;

        // 집중 상태에서는 상태 해제
        if (isFocusMode)
        {
            isFocusMode = false;

            // 딜레이 코루틴 시작
            skillManager.Focus();

            // CrossHair 제거
            crossHair.Destroy();
        }

        // 타격 소리
        etcAudio.clip = damagedClip;
        etcAudio.Play();

        // 이동 멈춤
        StopMove();

        if (conWeaponType == Weapontype.Swing)
            swingWeapon.TrailOff();

        // 데미지 처리
        if (DecreaseHp(damage) == 0)
        // 체력 0 이하는 사망 처리
        {
            // 입력 무시
            InputManager.Instance.canInput = false;

            // 피격 판정 off
            canTakeDamage = false;

            // 피격 이펙트
            SystemManager.Instance.HitEffect(0.1f, 1.2f);
            SystemManager.Instance.CameraShake("Explosion", 0.1f, 50, 5);
            SystemManager.Instance.ChangeColorGrading(-100);

            // 공격 방향으로의 넉백
            StartCoroutine(KnockBackCoroutine(knockBackDirection, distance));

            // 사망 애니메이션
            animator.SetTrigger("Die");
            conAction = 8;

            Invoke("DiePanelOn", 2);

        }
        // 체력 0이 아닌 경우
        else
        {
            // 피격 이펙트
            SystemManager.Instance.HitEffect(0.1f, 1.2f);
            SystemManager.Instance.CameraShake("Explosion", 0.1f, 50, 5);
            SystemManager.Instance.ChangeColorGrading(((float)conHp / maxHp) * 100);

            // 공격 방향으로의 넉백
            StartCoroutine(KnockBackCoroutine(knockBackDirection, distance));
            
            animator.SetTrigger("Damaged");
            conAction = 9;
        }
    }
    
    public void ContinuousShootingOff()
    {
        continuousShootingOn = false;
    }

    public void MoveSoundPlay()
    {
        moveAudio.clip = footstepClip;
        moveAudio.Play();
    }

    public void JumpSoundPlay()
    {
        moveAudio.clip = jumpClip;
        moveAudio.Play();
    }

    public void RollingSoundPlay()
    {
        moveAudio.clip = rollingClip;
        moveAudio.Play();
    }

    public void WeaponSoundPlay()
    {
        weaponAudio.Play();
    }

    // ===================================================== private function ============================================================


    private void Moving()
    {
        if (CanChangeAction[conAction, 0])
        {
            // 앞뒤 이동계산
            if (Input.GetKey(KeyCode.W))
            {
                verticalSpeed += moveAcceleration * Time.deltaTime * 20;
                if (verticalSpeed > conMaxSpeed)
                    verticalSpeed = conMaxSpeed;

                isFront = true;
                isVerticalMove = true;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                verticalSpeed -= moveAcceleration * Time.deltaTime * 20;
                if (verticalSpeed < conMaxSpeed * -1)
                    verticalSpeed = conMaxSpeed * -1;

                isFront = false;
                isVerticalMove = true;
            }
            else
            {
                if (isFront)
                {
                    verticalSpeed -= moveDeceleration * Time.deltaTime * 20;
                    if (verticalSpeed < 0)
                        verticalSpeed = 0;
                }
                else
                {
                    verticalSpeed += moveDeceleration * Time.deltaTime * 20;
                    if (verticalSpeed > 0)
                        verticalSpeed = 0;
                }
                isVerticalMove = false;
            }

            // 좌우 이동계산
            if (Input.GetKey(KeyCode.A))
            {
                horizontalSpeed += moveAcceleration * Time.deltaTime * 20;
                if (horizontalSpeed > conMaxSpeed)
                    horizontalSpeed = conMaxSpeed;

                isLeft = true;
                isHorizontalMove = true;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                horizontalSpeed -= moveAcceleration * Time.deltaTime * 20;
                if (horizontalSpeed < conMaxSpeed * -1)
                    horizontalSpeed = conMaxSpeed * -1;

                isLeft = false;
                isHorizontalMove = true;
            }
            else
            {
                if (isLeft)
                {
                    horizontalSpeed -= moveDeceleration * Time.deltaTime * 20;
                    if (horizontalSpeed < 0)
                        horizontalSpeed = 0;
                }
                else
                {
                    horizontalSpeed += moveDeceleration * Time.deltaTime * 20;
                    if (horizontalSpeed > 0)
                        horizontalSpeed = 0;
                }

                isHorizontalMove = false;
            }
        }
        isMoving = isHorizontalMove || isVerticalMove;

        // 이동 방향 계산
        newMove = SystemManager.Instance.forward * verticalSpeed + SystemManager.Instance.left * horizontalSpeed;
        moveVector = Vector3.Lerp(beforeMove, newMove, Time.smoothDeltaTime * 20);



        // 이동 크기 계산
        moveMaginitude = moveVector.magnitude > conMaxSpeed ? conMaxSpeed : moveVector.magnitude;
        animator.SetFloat("Speed", moveMaginitude / runMaxSpeed);

        
        // 이전 프레임과 반대 방향으로 방향을 바꾸면 순간적으로 큰 감속
        if (Vector3.Dot(beforeMove, newMove) < 0)
            moveMaginitude = moveMaginitude / 5;
        beforeMove = newMove;

        // 이동 방향으로 회전
        Vector2 rot;
        if (isFocusMode)
        {
            Vector3 temp = crossHair.CrossHairWorldPosition();

            switch (conWeaponType)
            {
                case Weapontype.Shooting:
                    rot = new Vector2(transform.position.x - temp.x, -(shootingWeapon.transform.position.z - temp.z + 0.6f));
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, Quaternion.FromToRotation(Vector3.up, rot).eulerAngles.z, 0), Time.smoothDeltaTime * 20);
                    break;

                case Weapontype.Swing:
                case Weapontype.Throwing:
                    rot = new Vector2(transform.position.x - temp.x, -(transform.position.z - temp.z));
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, Quaternion.FromToRotation(Vector3.up, rot).eulerAngles.z, 0), Time.smoothDeltaTime * 20);
                    break;
            }
        }
        else
        {
            if(isMoving)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(newMove), Time.smoothDeltaTime * 20);
                //transform.rotation = Quaternion.LookRotation(newMove);
            }
        }



        // 이동
        Vector3 moveResult = (moveVector.normalized * moveMaginitude + Vector3.up * resultGravity);
        controller.Move(moveResult * Time.deltaTime);
    }

    private void AttackStart()
    {
        // 무기 타입에 따라..
        switch (conWeaponType)
        {
            // 무기가 없다.(주먹질)
            case Weapontype.None:
                // 기존의 이동을 멈춘다.
                StopMove();

                // 공격 판정 범위 On
                attackHitArea.CloseHitAreaOn(0.8f, 1f);

                animator.SetTrigger("Punch");
                conAction = 2;

                if (IsRunning())
                    attackMovingCoroutine = StartCoroutine(AttackMovingCoroutine(0.8f, false));
                else
                    attackMovingCoroutine = StartCoroutine(AttackMovingCoroutine(0.3f, false));

                break;


            // 몽둥이, 검 등의 무기
            case Weapontype.Swing:
                // 기존의 이동을 멈춘다.
                StopMove();

                // 공격 판정 범위 On
                attackHitArea.CloseHitAreaOn(swingWeapon.hitAreaHorizontal, swingWeapon.hitAreaVertical);

                // Trail 효과를 On
                swingWeapon.TrailOn();

                // 집중 상태에서의 공격에서는 앞으로 돌진
                if (isFocusMode)
                {
                    isFocusMode = false;

                    // 딜레이 코루틴 시작
                    skillManager.Focus();

                    // CrossHair 제거
                    crossHair.Destroy();
                    
                    float magnitude = Vector3.Distance(crossHair.CrossHairWorldPosition(), transform.position);
                    magnitude = Mathf.Min(magnitude, 10);
                    attackMovingCoroutine = StartCoroutine(AttackMovingCoroutine(magnitude - 1f, true));
                }
                else
                {
                    if(IsRunning())
                        attackMovingCoroutine = StartCoroutine(AttackMovingCoroutine(1.5f, false));
                    else
                        attackMovingCoroutine = StartCoroutine(AttackMovingCoroutine(0.5f, false));
                }

                // 공격
                animator.SetTrigger("Swing");
                conAction = 2;
                break;

            // 총 등의 무기
            case Weapontype.Shooting:
                // 기존의 이동을 멈춘다.
                StopMove();
                
                // 조준 중 공격
                if (conAction == 7)
                {
                    continuousShootingOn = true;

                    animator.SetTrigger("Shooting");
                    conAction = 8;
                }
                // 일반 공격
                else
                {
                    animator.SetTrigger("Shooting");
                    conAction = 2;
                }

                break;

            // 수류탄 등의 무기
            case Weapontype.Throwing:
                // 기존의 이동을 멈춘다.
                StopMove();

                // 조준해서 던진경우
                if(isFocusMode)
                {
                    // 딜레이 코루틴 시작
                    skillManager.Focus();

                    // CrossHair 제거
                    crossHair.Destroy();
                }

                animator.SetTrigger("Throwing");
                conAction = 2;

                break;
        }
    }

    private void AttackEnd()
    {
        if(canContinuousAttack && !isFinishAttackDelay)
        {
            // 무기 타입에 따라..
            switch (conWeaponType)
            {
                // 무기가 없다.(주먹질)
                case Weapontype.None:

                    // 앞으로 조금 이동
                    attackMovingCoroutine = StartCoroutine(AttackMovingCoroutine(0.3f, false));

                    // 공격
                    animator.SetTrigger("Punch");

                    break;


                // 몽둥이, 검 등의 무기
                case Weapontype.Swing:     
                    
                    // 무기 파괴 유무 검사
                    if (swingWeapon.conUsing <= 0)
                    {
                        // 파괴 이펙트 생성
                        GameObject temp = ObjectPullManager.Instance.GetInstanceByName("WeaponBreaking");
                        temp.transform.position = swingWeapon.transform.position + temp.transform.position;
                        temp.SetActive(true);

                        // 무기 파괴
                        swingWeapon.transform.SetParent(cantUseWeaponParent);
                        swingWeapon.transform.localPosition = Vector3.zero;
                        swingWeapon.gameObject.SetActive(false);
                        swingWeapon.DestroyWeapon();
                        swingWeapon = null;

                        // 현재 가지는 무기 정보 초기화
                        conWeaponType = Weapontype.None;

                        // slot을 비운다.
                        weaponSlotManager.ResetWeapon();

                        if (FinishAttackDelayCoroutine == null)
                            FinishAttackDelayCoroutine = StartCoroutine(AttackFinishCoroutine());

                        attackHitArea.CloseHitAreaOff();

                        // 이동 시작
                        MoveStart();
                    }
                    else
                    {
                        // 그 외엔 조금 이동
                        attackMovingCoroutine = StartCoroutine(AttackMovingCoroutine(0.5f, false));

                        // 공격
                        animator.SetTrigger("Swing");
                    }
                    break;
            }
        }
        // 공격 마무리
        else
        {
            // 무기 타입에 따라..
            switch (conWeaponType)
            {
                // 무기가 없다.(주먹질)
                case Weapontype.None:

                    if (FinishAttackDelayCoroutine != null)
                        FinishAttackDelayCoroutine = StartCoroutine(AttackFinishCoroutine());

                    // 공격 판정 범위 Off
                    attackHitArea.CloseHitAreaOff();

                    // 이동 시작
                    MoveStart();

                    break;


                // 몽둥이, 검 등의 무기
                case Weapontype.Swing:

                    if (swingWeapon.conUsing <= 0)
                    {
                        // 파괴 이펙트 생성
                        GameObject temp = ObjectPullManager.Instance.GetInstanceByName("WeaponBreaking");
                        temp.transform.position = swingWeapon.transform.position + temp.transform.position;
                        temp.SetActive(true);

                        // 무기 파괴
                        swingWeapon.transform.SetParent(cantUseWeaponParent);
                        swingWeapon.transform.localPosition = Vector3.zero;
                        swingWeapon.gameObject.SetActive(false);
                        swingWeapon.DestroyWeapon();
                        swingWeapon = null;

                        // 현재 가지는 무기 정보 초기화
                        conWeaponType = Weapontype.None;

                        // slot을 비운다.
                        weaponSlotManager.ResetWeapon();

                    }
                    else
                    {
                        // Trail Off
                        swingWeapon.TrailOff();
                    }

                    if (FinishAttackDelayCoroutine == null)
                        FinishAttackDelayCoroutine = StartCoroutine(AttackFinishCoroutine());

                    // 공격 판정 범위 Off
                    attackHitArea.CloseHitAreaOff();

                    // 이동 시작
                    MoveStart();

                    break;

                // 총 등의 무기
                case Weapontype.Shooting:
                    
                    shootingWeapon.ContinuousAttackEnd();

                    // 이동 시작
                    MoveStart();

                    break;
            }
        }
    }

    private void PunchHitMonster()
    {
        // 무기 사용 사운드
        WeaponSoundPlay();

        // 공격 영역에 들어온 몬스터 데미지 처리
        attackHitArea.CloseHit(true, 5, transform.forward, 0.5f);
    }

    private void SwingWeaponHitMonster()
    {
        // 무기 남은 사용량 감소
        swingWeapon.UsingWeapon();

        // 무기 사용 사운드
        WeaponSoundPlay();

        // 공격 영역에 들어온 몬스터 데미지 처리
        attackHitArea.CloseHit(false, swingWeapon.damage, transform.forward, 0.8f);
    }
    
    // 연속 공격의 마지막 공격 이후에는 약간의 딜레이
    private void AttackFinishEnd()
    {
        FinishAttackDelayCoroutine = StartCoroutine(AttackFinishCoroutine());
    }


    private IEnumerator AttackFinishCoroutine()
    {
        isFinishAttackDelay = true;

        float conTime = 0;
        float maxTime = 0.2f;
        while(conTime < maxTime)
        {
            conTime += Time.deltaTime;

            yield return null;
        }

        isFinishAttackDelay = false;
        FinishAttackDelayCoroutine = null;
    }

    private void InstanceProjectile()
    {
        if (shootingWeapon.conUsing != 0)
        {
            // 투사체를 생성하여 전방으로 공격
            shootingWeapon.InstanceProjectile(isFocusMode, transform.forward);

            // 격발 소리
            WeaponSoundPlay();
        }
    }

    private void ShootingEnd()
    {
        // 조준 중 공격은..
        if (conAction == 8)
        {
            // 마우스를 계속 누르고 있으면 연속 공격
            if(continuousShootingOn && shootingWeapon.CanAttack())
            {
                animator.SetTrigger("Shooting");
            }
            else
            {
                shootingWeapon.ContinuousAttackEnd();

                // 조준 상태로 돌아감
                animator.SetTrigger("Aim");
                conAction = 7;
            }
        }
        // 일반 공격
        else
        {
            // 이동 시작
            MoveStart();
        }
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

        // Jump 사운드 실행
        JumpSoundPlay();

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

    private void MoveStart()
    {
        // 이동 애니메이션 실행
        animator.ResetTrigger("Move");
        animator.SetTrigger("Move");
        conAction = 0;
    }
    
    private void StopMove()
    {
        verticalSpeed = 0;
        isVerticalMove = false;
        horizontalSpeed = 0;
        isHorizontalMove = false;

        conMaxSpeed = walkMaxSpeed;

        if (isAttackMovingCoroutineRunning)
        {
            StopCoroutine(attackMovingCoroutine);

            isAttackMovingCoroutineRunning = false;
            motionTrail.OffMotionTrail();
        }
    }

    private bool IsRunning()
    {
        if (moveMaginitude > runMaxSpeed * 0.7f + walkMaxSpeed * 0.3f)
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
                if (!isGroundAfterJump)
                {
                    isGroundAfterJump = true;

                    JumpSoundPlay();

                    // 어느정도 빠른 속도이면 멈추지 않고 계속 달린다
                    if (IsRunning())
                    {
                        dustParticle.Play();

                        MoveStart();
                    }
                    // 그 외는 점프 후 멈춘다.
                    else
                    {
                        dustParticle.Play();

                        MoveStart();
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

    private IEnumerator KnockBackCoroutine(Vector3 moveDirection, float distance)
    {
        // 넉백 반대 방향을 바라본다
        transform.forward = -moveDirection;

        float conTime = 0;
        float maxTime = 1;
        while (conTime < maxTime)
        {
            // controller.Move(transform.forward * distance * 10 * Time.deltaTime);
            controller.Move(moveDirection * distance * Time.deltaTime);

            conTime += Time.deltaTime * 10;
            yield return null;
        }
    }

    private IEnumerator AttackMovingCoroutine(float distance, bool motionTrailOn)
    {
        // Motion Trail On
        if (motionTrailOn)
            motionTrail.OnMotionTrail();

        isAttackMovingCoroutineRunning = true;

        float conTime = 0;
        float maxTime = 0.1f;

        while(conTime < maxTime)
        {
            controller.Move(transform.forward * distance * 10 * Time.deltaTime);

            conTime += Time.deltaTime;
            yield return null;
        }

        isAttackMovingCoroutineRunning = false;

        // Motion Trail Off
        if (motionTrailOn)
            motionTrail.OffMotionTrail();
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
    }

    private void PickUpStart()
    {
        // 이동을 멈춘다.
        StopMove();
        
        DropObjectScript.isThereWeaponAroundPlayer = false;

        // 아이템을 줍는 동작 실행
        animator.SetTrigger("PickUp");
        conAction = 4;
    }

    private void ArmWeapon()
    {
        Transform weapon;

        WeaponScript conWeapon = GetWeaponScript();

        // 현재 무기를 가지고 있다면
        if (conWeaponType != Weapontype.None)
        {
            // 사용하던 무기를 바닥에 놓는다.
            weapon = conWeapon.GetComponent<Transform>();
            weapon.SetParent(droppedWeaponParent);
            weapon.position = transform.position + transform.forward * 0.5f;
            weapon.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            // 무기를 최초 상태로 초기화
            conWeapon.ResetOwner();
            conWeapon.ChangeToDrop();
        }

        // 플레이어가 가지는 무기 정보를 새로운 무기 정보로 변경
        conWeapon = DropObjectScript.dropObject;
        conWeaponType = conWeapon.weaponType;
        SetWeaponInfo(conWeapon);
        weaponAudio.clip = conWeapon.weaponUsingSound;

        // 무기의 상태 및 정보를 플레이어가 가지는 상태로 변경
        conWeapon.SetOwner(this);
        conWeapon.ChangeToEquiped();

        // 새로운 무기를 손으로 이동
        weapon = conWeapon.transform;
        weapon.SetParent(handlingWeaponParent, true);
        weapon.localPosition = Vector3.zero;
        weapon.localRotation = Quaternion.Euler(0, 0, 0);
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
        
        WeaponScript conWeapon = GetWeaponScript();

        // 현재 무기를 가지고 있다면
        if (conWeaponType != Weapontype.None)
        {
            // 무기를 바닥에 놓는다.
            weapon = handlingWeaponParent.GetChild(0);
            weapon.SetParent(droppedWeaponParent);
            weapon.position = transform.position + transform.forward * 0.5f;
            weapon.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            // 무기를 최초 상태로 초기화
            conWeapon.ResetOwner();
            conWeapon.ChangeToDrop();
            weaponAudio.clip = punchClip;

            // 현재 무기 상태를 없음 으로 기록.
            conWeaponType = Weapontype.None;
            SetWeaponInfo(null);
        }
    }

    // 무기 교체 모션 실행
    private void WeaponChangeStart()
    {
        switch(conWeaponType)
        {
            case Weapontype.None:
                WeaponScript weapon = weaponSlotManager.GetWeapon(conWeaponIndex);

                if (weapon != null)
                {
                    switch (weapon.weaponType)
                    {
                        // 빈 손에서 등에 매고있는 무기를 드는 애니메이션 실행.
                        case Weapontype.Shooting:
                        case Weapontype.Swing:
                            animator.SetTrigger("WeaponChangeToBackWeapon");
                            conAction = 5;
                            break;

                        // 빈 손에서 배에 걸고있는 무기를 드는 애니메이션 실행.
                        case Weapontype.Throwing:
                            animator.SetTrigger("WeaponChangeToBellyWeapon");
                            conAction = 5;
                            break;
                    }
                }
                break;

            case Weapontype.Shooting:
            case Weapontype.Swing:
                animator.SetTrigger("WeaponChangeToBackWeapon");
                conAction = 5;
                break;

            case Weapontype.Throwing:
                animator.SetTrigger("WeaponChangeToBellyWeapon");
                conAction = 5;
                break;
        }
    }

    private void WeaponChangeToBackWeapon()
    {
        Transform weapon;

        // 현재 무기를 가지고 있다면
        if (conWeaponType != Weapontype.None)
        {
            // 무기를 등으로 이동
            weapon = handlingWeaponParent.GetChild(0);
            weapon.SetParent(backWeaponParent);
            weapon.localPosition = Vector3.zero;
            weapon.localRotation = Quaternion.Euler(0, 0, 0);
            weapon.GetComponent<WeaponScript>().ChangeToBack();
        }

        // 선택한 slot에 저장되어있는 무기를 가져온다.
        WeaponScript newWeapon = weaponSlotManager.GetWeapon(conWeaponIndex);

        // 가져오려는 slot에 물건이 없다면..
        if (newWeapon == null)
        {
            conWeaponType = Weapontype.None;
            weaponAudio.clip = punchClip;

        }
        else
        {
            // 무기를 배에서 얻는다면..
            if(newWeapon.weaponType == Weapontype.Throwing)
            {
                conWeaponType = Weapontype.None;
                
                if (weaponSlotManager.GetWeapon(conWeaponIndex).weaponType == Weapontype.Throwing)
                    animator.SetTrigger("WeaponChangeToBellyWeapon");
                else
                    animator.SetTrigger("WeaponChangeToBackWeapon");
            }
            // 무기를 등에서 얻는다면..
            else
            {
                // 무기를 손으로 이동
                weapon = newWeapon.transform;
                weapon.SetParent(handlingWeaponParent);
                weapon.localPosition = Vector3.zero;
                weapon.localRotation = Quaternion.Euler(0, 0, 0);
                weapon.GetComponent<WeaponScript>().ChangeToEquiped();

                // 새로운 무기로 정보를 변경
                conWeaponType = newWeapon.weaponType;
                SetWeaponInfo(newWeapon);
                weaponAudio.clip = newWeapon.weaponUsingSound;
            }
        }
    }

    private void WeaponChangeToBellyWeapon()
    {
        Transform weapon;

        // 현재 무기를 가지고 있다면
        if (conWeaponType != Weapontype.None)
        {
            // 무기를 배로 이동
            weapon = handlingWeaponParent.GetChild(0);
            weapon.SetParent(bellyWeaponParent);
            weapon.localPosition = Vector3.zero;
            weapon.localRotation = Quaternion.Euler(0, 0, 0);
            weapon.GetComponent<WeaponScript>().ChangeToBelly();
        }

        // 선택한 slot에 저장되어있는 무기를 가져온다.
        WeaponScript newWeapon = weaponSlotManager.GetWeapon(conWeaponIndex);

        // 가져오려는 slot에 물건이 없다면..
        if (newWeapon == null)
        {
            conWeaponType = Weapontype.None;
            weaponAudio.clip = punchClip;
        }
        else
        {
            // 무기를 등에서 얻는다면..
            if (newWeapon.weaponType == Weapontype.Swing || newWeapon.weaponType == Weapontype.Shooting)
            {
                conWeaponType = Weapontype.None;

                // 등에서 무기를 얻는 애니메이션을 실행
                if (weaponSlotManager.GetWeapon(conWeaponIndex).weaponType == Weapontype.Throwing)
                    animator.SetTrigger("WeaponChangeToBellyWeapon");
                else
                    animator.SetTrigger("WeaponChangeToBackWeapon");
            }
            // 무기를 배에서 얻는다면..
            else
            {
                // 무기를 손으로 이동
                weapon = weaponSlotManager.GetWeapon(conWeaponIndex).transform;
                weapon.SetParent(handlingWeaponParent);
                weapon.localPosition = Vector3.zero;
                weapon.localRotation = Quaternion.Euler(0, 0, 0);
                weapon.GetComponent<WeaponScript>().ChangeToEquiped();

                // 새로운 무기로 정보를 변경
                conWeaponType = newWeapon.weaponType;
                SetWeaponInfo(newWeapon);
                weaponAudio.clip = newWeapon.weaponUsingSound;
            }
        }
    }

    private void RollStart(bool isLeft, Vector3 moveDirection)
    {
        // 이동을 멈춘다.
        StopMove();

        // 구르기 간 피격 판정 Off
        canTakeDamage = false;

        // 구르기 사운드 플레이
        RollingSoundPlay();

        // 기존의 상태에서의 변화 체크
        if (isFocusMode)
        {
            isFocusMode = false;

            // 딜레이 코루틴 시작
            skillManager.Focus();

            // CrossHair 제거
            crossHair.Destroy();
        }

        if(conWeaponType == Weapontype.Swing)
            swingWeapon.TrailOff();

        // 구르기 시작 위치에 먼지 파티클 생성
        dustParticle.Play();

        // 구르는 애니메이션 실행
        animator.SetTrigger("Roll");
        conAction = 6;

        // 구르는 방향으로 회전
        if (isLeft)
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y - 90, 0);
        else
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y + 90, 0);

        // 구르기 모션 실행
        attackMovingCoroutine = StartCoroutine(AttackMovingCoroutine(3f, false));
    }

    private void RollEnd()
    {
        // 피격 판정 On
        canTakeDamage = true;

        // 이동
        MoveStart();
    }

    private void Aim()
    {
        animator.SetTrigger("Aim");
        conAction = 7;
    }

    private void Rush()
    {
        animator.SetTrigger("Rush");
        conAction = 7;
    }

    private void ThrowingAim()
    {
        animator.SetTrigger("ThrowingAim");
        conAction = 7;
    }

    private void Throwing()
    {
        throwingWeapon.transform.SetParent(cantUseWeaponParent);

        // 집중상태에서는 클릭한 좌표로 던진다.
        if (isFocusMode)
        {
            isFocusMode = false;

            float magnitude = Vector3.Distance(crossHair.CrossHairWorldPosition(), transform.position);
            throwingWeapon.Throw(transform.forward, magnitude * 39.5f);
        }
        // 집중상태가 아닌 경우 적당한 힘으로 전방에 던진다.
        else
            throwingWeapon.Throw(transform.forward, 200);
        
        // slot을 비운다.
        weaponSlotManager.ResetWeapon();

        // 현재 무기 정보 초기화
        conWeaponType = Weapontype.None;
        throwingWeapon = null;
    }

    private void SetWeaponInfo(WeaponScript weapon)
    {
        switch (conWeaponType)
        {
            case Weapontype.None:
                swingWeapon = null;
                shootingWeapon = null;
                throwingWeapon = null;
                break;
            case Weapontype.Swing:
                swingWeapon = (SwingWeaponScript)weapon;
                shootingWeapon = null;
                throwingWeapon = null;
                break;
            case Weapontype.Shooting:
                swingWeapon = null;
                shootingWeapon = (ShootingWeaponScript)weapon;
                throwingWeapon = null;
                break;
            case Weapontype.Throwing:
                swingWeapon = null;
                shootingWeapon = null;
                throwingWeapon = (ThrowingWeaponScript)weapon;
                break;
        }
    }

    private WeaponScript GetWeaponScript()
    {
        switch (conWeaponType)
        {
            case Weapontype.None:
                return null;
            case Weapontype.Swing:
                return swingWeapon;
            case Weapontype.Shooting:
                return shootingWeapon;
            case Weapontype.Throwing:
                return throwingWeapon;
            default:
                return null;
        }
    }

    private void DiePanelOn()
    {
        SystemManager.Instance.DiePanelOn();
    }
}
