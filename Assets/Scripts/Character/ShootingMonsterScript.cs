using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootingMonsterScript : MonsterScript
{
    public enum State { Trace, Attack, Ready };
    private State state;

    public string projectileName;
    public Transform projectileGenerationPos;
    public float projectileDistance;

    private bool isAttackMotion;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override IEnumerator FSM()
    {
        // 플레이어를 추적하면서 시작
        state = State.Trace;

        while (!isDie)
        {
            fsmCoroutine = StartCoroutine(state.ToString() + "Coroutine");
            yield return fsmCoroutine;
        }
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
        while (conTime < maxTime)
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

        // 공격 모션 실행
        isAttackMotion = true;
        anim.SetTrigger("Smash Attack");

        while (isAttackMotion)
            yield return null;

        state = State.Trace;
    }

    private void AttackMostionEnd()
    {
        isAttackMotion = false;
    }

    private void ShootingProjectile()
    {
        // 투사체를 생성
        GameObject temp = ObjectPullManager.GetInstanceByName(projectileName);

        // 발사
        if (temp != null)
        {
            ProjectileScript projectile = temp.GetComponent<ProjectileScript>();
            projectile.Init(projectileGenerationPos.position, transform.forward, false, damage, projectileDistance);
        }
    }
}
