using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class MonsterScript : MonoBehaviour, ICharacterScript
{
    protected Rigidbody rigid;
    protected BoxCollider coll;
    protected Animator anim;
    protected SkinnedMeshRenderer meshRenderer;
    protected NavMeshAgent agent;

    // Player Info
    [HideInInspector] public bool isPlayerInAttackDetectArea;
    protected PlayerScript playerScript;
    protected Transform playerTrans;

    // Change Material Color
    protected bool isChangingMaterialColorCoroutineOn;
    protected Coroutine changeMaterialColorCoroutine;
    protected Color normalMaterialColor;
    protected Color hitMaterialColor;

    // KnockBack
    protected bool isMovingCoroutineOn;
    protected Coroutine movingCoroutine;

    // HP Bar
    protected MonsterHpBarScript hpBarScript;

    // FSM
    protected Coroutine fsmCoroutine;

    protected Vector3 targetPos;

    [Header("Monster Info")]
    public int damage;                                      // 캐릭터 데미지
    public int maxHp;                                       // 캐릭터 최대 체력
    public int conHp;                                       // 캐릭터 현재 체력
    [HideInInspector] public bool isDie;                    // 캐릭터 사망 여부
    public float height;                                    // HP 바를 띄울 높이
    public AudioSource audioSource;
    public AudioClip dieCip;


    // Start is called before the first frame update
    private void Awake()
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
    }

    private void OnEnable()
    {
        // 이전에 변경된 정보를 다시 원래대로
        coll.enabled = true;
        conHp = maxHp;
        isDie = false;
        isPlayerInAttackDetectArea = false;

        // 사망으로 인해 삭제된 마테리얼 원래대로
        meshRenderer.material.SetFloat("_Cutoff", 0);
        meshRenderer.material.SetFloat("_Thick", 0);

        // 새로 HP Bar를 붙여서
        hpBarScript = ObjectPullManager.Instance.GetInstanceByName("MonsterHp").GetComponent<MonsterHpBarScript>();
        hpBarScript.Init(transform, height);

        // 행동 시작
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
        {
            audioSource.clip = dieCip;
            audioSource.Play();

            // 현재 하는 행동을 중단
            StopCoroutine(fsmCoroutine);
            
            // 이동을 제한
            agent.isStopped = true;
            
            // 사망 처리
            StartCoroutine(DieCoroutine());
        }
        // 체력 0이 아닌 경우
        else
        {
            // 피격 이펙트
            SystemManager.Instance.HitEffect(0.07f, 0.5f);

            // 혈흔 파티클 생성
            GameObject blood = ObjectPullManager.Instance.GetInstanceByName("Blood");
            blood.transform.position = transform.position;
            blood.transform.forward = -knockBackDirection.normalized;
            blood.SetActive(true);

            // 공격 방향으로의 넉백
            //KnockBack(knockBackDirection, distance);

            // 피격 애니메이션 실행
            //ChangeAnimation("Take Damage");
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
    
    // 넉백
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

    // ------------------------------------------------------------------------------------------- private function -------------------------------------------------------------------------------------------


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

        // 몬스터가 죽었음을 알린다.
        RoundManager.Instance.MonsterDie();

        // 캐릭터 삭제
        gameObject.SetActive(false);
    }
    
    protected abstract IEnumerator FSM();
}
