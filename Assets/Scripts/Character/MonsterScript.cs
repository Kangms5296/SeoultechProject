using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterScript : MonoBehaviour, ICharacterScript
{
    private Rigidbody rigid;
    private BoxCollider coll;
    private Animator anim;
    private SkinnedMeshRenderer meshRenderer;

    // Change Material Color
    private bool isChangingMaterialColor = false;
    private Coroutine changeMaterialColorCoroutine;
    private Color normalMaterialColor;
    private Color hitMaterialColor;

    // KnockBack
    private bool isMovingCoroutineOn = false;
    private Coroutine movingCoroutine;

    // HP Bar
    private MonsterHpBarScript hpBarScript;

    [Header("Character Info")]
    public bool isDie;                                    // 사망 여부
    public int maxHp;                                     // 플레이어 최대 체력
    public int conHp;                                     // 플레이어 현재 체력

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        normalMaterialColor = new Color(0, 0, 0);
        hitMaterialColor = new Color(0.4f, 0, 0);
        
        conHp = maxHp;

        hpBarScript = ObjectPullManager.GetInstanceByName("MonsterHp").GetComponent<MonsterHpBarScript>();
        hpBarScript.Init(transform);
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
        if (isChangingMaterialColor)
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
        isChangingMaterialColor = true;

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

        isChangingMaterialColor = true;

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
}
