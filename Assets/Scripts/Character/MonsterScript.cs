using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterScript : MonoBehaviour, ICharacterScript
{
    private Rigidbody rigid;
    private BoxCollider coll;
    private Animator anim;
    private SkinnedMeshRenderer meshRenderer;
    private NavMeshAgent agent;

    // Player Info
    [HideInInspector] public bool isPlayerInAttackDetectArea;
    [HideInInspector] public bool isPlayerInAttackArea;
    private PlayerScript playerScript;
    private Transform playerTrans;

    // Change Material Color
    private bool isChangingMaterialColorCoroutineOn;
    private Coroutine changeMaterialColorCoroutine;
    private Color normalMaterialColor;
    private Color hitMaterialColor;

    // KnockBack
    private bool isMovingCoroutineOn;
    private Coroutine movingCoroutine;

    // HP Bar
    private MonsterHpBarScript hpBarScript;

    // FSM
    public enum State{ Trace, Attack };
    private State state;
    private Coroutine fsmCoroutine;

    private Vector3 targetPos;


    [Header("Monster Info")]
    public int damage;                                      // 캐릭터 데미지
    public int maxHp;                                       // 캐릭터 최대 체력
    public int conHp;                                       // 캐릭터 현재 체력
    [HideInInspector] public bool isDie;                    // 캐릭터 사망 여부
    public bool isShortDistanceAttack;                      // 캐릭터 근거리 공격 여부

    [Header("Short Distance Attack")]
    public MonsterAttackAreaScript attackAreaScript;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        agent = GetComponent<NavMeshAgent>();

        playerScript = FindObjectOfType<PlayerScript>();
        playerTrans = playerScript.transform;

        normalMaterialColor = new Color(0, 0, 0);
        hitMaterialColor = new Color(0.4f, 0, 0);
        
        conHp = maxHp;
        hpBarScript = ObjectPullManager.GetInstanceByName("MonsterHp").GetComponent<MonsterHpBarScript>();
        hpBarScript.Init(transform);

        StartCoroutine(FSM());
    }

    // ------------------------------------------------------------------------------------------- public function -------------------------------------------------------------------------------------------

    public void TakeDamage(int damage, Vector3 knockBackDirection, float distance)
    {
        // 일시적으로 색 변화
        ChangeMaterialColor(hitMaterialColor, 0.05f);

        // 데미지 처리
        if (DecreaseHp(damage) == 0)
            // 체력 0 이하는 사망 처리
            Die();
        // 체력 0이 아닌 경우
        else
        {
            // 공격 방향으로의 넉백
            KnockBack(knockBackDirection, distance);

            // 피격 애니메이션 실행
            ChangeAnimation("Take Damage");
        }
    }

    // 체력 회복
    public int RecoveryHp(int value)
    {
        conHp += value;
        if (conHp > maxHp)
            conHp = maxHp;

        return conHp;
    }

    // 체력 감소
    public int DecreaseHp(int value)
    {
        conHp -= value;
        if (conHp < 0)
            conHp = 0;

        hpBarScript.DecreaseHp((float)conHp / maxHp);

        return conHp;
    }

    public void Die()
    {
        StopCoroutine(fsmCoroutine);

        StartCoroutine(DieCoroutine());
    }
    
    public void KnockBack(Vector3 knockBackDirection, float distance)
    {
        // 넉백 방향으로 회전
        Quaternion q = Quaternion.LookRotation(-knockBackDirection);
        transform.rotation = q;

        // 넉백 방향으로 이동
        if (isMovingCoroutineOn)
            StopCoroutine(movingCoroutine);
        movingCoroutine = StartCoroutine(MovingCoroutine(knockBackDirection, distance));
    }

    public void ChangeMaterialColor(Color color, float maxTime)
    {
        if (isChangingMaterialColorCoroutineOn)
            StopCoroutine(changeMaterialColorCoroutine);
        changeMaterialColorCoroutine = StartCoroutine(ChangeMaterialColorCoroutine(color, maxTime));
    }

    public void ChangeAnimation(string animName)
    {
        AnimatorStateInfo conAnimInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 기존에 재생되는 애니메이션이므로, 시작점으로 롤백
        if (conAnimInfo.IsName(animName))
        {
        }
        // 새로운 애니메이션으로 전환
        else
            anim.SetTrigger(animName);
    }

    public void StartFSM()
    {
        fsmCoroutine = StartCoroutine(FSM());
    }

    // ------------------------------------------------------------------------------------------- private function -------------------------------------------------------------------------------------------

    private IEnumerator FSM()
    {
        // 플레이어를 추적하면서 시작
        state = State.Trace;

        while (!isDie)
        {
            fsmCoroutine = StartCoroutine(state.ToString() + "Coroutine");
            yield return fsmCoroutine;
        }
    }

    private IEnumerator MovingCoroutine(Vector3 moveDirection, float distance)
    {
        isMovingCoroutineOn = true;

        float conTime = 0;
        float maxTime = 1;
        while (conTime < maxTime)
        {
            // controller.Move(transform.forward * distance * 10 * Time.deltaTime);
            rigid.AddForce(moveDirection * distance, ForceMode.Impulse);

            conTime += Time.deltaTime * 10;
            yield return null;
        }

        isMovingCoroutineOn = false;
    }

    private IEnumerator ChangeMaterialColorCoroutine(Color color, float maxTime)
    {
        isChangingMaterialColorCoroutineOn = true;

        // 피격 직후 하얗게
        meshRenderer.material.SetColor("_EmissionColor", hitMaterialColor);

        float conTime = 0;
        while (conTime < maxTime)
        {
            conTime += Time.deltaTime;
            yield return null;
        }
        // 다시 원래대로
        meshRenderer.material.SetColor("_EmissionColor", normalMaterialColor);

        isChangingMaterialColorCoroutineOn = true;

    }

    private IEnumerator DieCoroutine()
    {
        // 공격 판정 삭제
        coll.enabled = false;
        
        // 사망 처리
        isDie = true;
        // 사망 애니메이션 처리
        anim.SetTrigger("Die");

        // 2초대기
        float conTime = 0;
        while(conTime < 2)
        {
            conTime += Time.deltaTime;
            yield return null;
        }

        // HP Bar 삭제
        hpBarScript.Die();

        // 1초동안 서서히 캐릭터 삭제
        conTime = 0;
        while (conTime < 3)
        {
            conTime += Time.deltaTime;
            meshRenderer.material.SetFloat("_Cutoff", conTime / 3);
            meshRenderer.material.SetFloat("_Thick", conTime / 3);
            yield return null;
        }
        meshRenderer.material.SetFloat("_Cutoff", 1);
        meshRenderer.material.SetFloat("_Thick", 1);

        // 캐릭터 삭제
        gameObject.SetActive(false);
    }

    private IEnumerator TraceCoroutine()
    {
        anim.SetBool("Walk Forward", true);
        agent.isStopped = false;
   
        // 공격 범위 내 플레이어가 있을때까지 추적
        while (!isPlayerInAttackDetectArea)
        {
            agent.SetDestination(playerTrans.position);
            yield return null;
        }
        targetPos = playerTrans.position;

        // 정지
        anim.SetBool("Walk Forward", true);
        agent.isStopped = true;

        // 현재 위치에서 플레이어를 바라볼 때까지 회전
        Vector3 startForward = transform.forward;
        Vector3 vectorToTarget = (targetPos - transform.position).normalized;
        float conTime = 0;
        float maxTime = 1f;
        while(conTime < maxTime)
        {
            transform.forward = Vector3.Lerp(startForward, vectorToTarget, conTime);
            conTime += Time.deltaTime * 3;
            yield return null;
        }
        

        anim.SetBool("Walk Forward", false);

        // 다음 행동을 공격으로 변환
        state = State.Attack;
    }

    private IEnumerator AttackCoroutine()
    {
        // 공격 시작 모션
        anim.SetTrigger("Cast Spell");

        // 공격 시작 모션이 끝날수 있도록 일정시간 대기
        float conTime = 0;
        float maxTime = 1.5f;
        while (conTime < maxTime)
        {
            conTime += Time.deltaTime;
            yield return null;
        }

        // 근거리 공격 처리
        if (isShortDistanceAttack)
        {
            // 공격 모션 실행
            anim.SetTrigger("Stab Attack");
            
            // 공격 간 전방이동 벡터 계산
            Vector3 movePos = transform.position + transform.forward;
            
            // 2초간 전방이동
            conTime = 0;
            maxTime = anim.GetCurrentAnimatorStateInfo(0).length;
            Debug.Log(maxTime);
            while (conTime < maxTime)
            {
                transform.position = Vector3.Lerp(transform.position, movePos, conTime / maxTime);

                conTime += Time.deltaTime;
                yield return null;
            }
            transform.position = movePos;
        }

        state = State.Trace;
    }

    private void AttackAreaOn()
    {
        // 공격 처리 On
        attackAreaScript.AttackAreaOn();
    }

    private void AttackAreaOff()
    {
        // 공격 처리 Off
        attackAreaScript.AttackAreaOff();
    }
}
