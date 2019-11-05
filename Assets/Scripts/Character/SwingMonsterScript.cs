using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingMonsterScript : MonsterScript
{
    public enum State { Trace, Attack, Ready};
    private State state;

    public MonsterAttackAreaScript attackAreaScript;
    private bool attackMovingForward;

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
        anim.SetTrigger("Stab Attack");

        while (!attackMovingForward)
            yield return null;

        // 공격 간 전방이동 벡터 계산
        Vector3 movePos = transform.position + transform.forward;

        // 2초간 전방이동
        conTime = 0;
        maxTime = anim.GetCurrentAnimatorStateInfo(0).length;
        while (conTime < maxTime)
        {
            transform.position = Vector3.Lerp(transform.position, movePos, conTime / maxTime);

            conTime += Time.deltaTime;
            yield return null;
        }
        transform.position = movePos;


        state = State.Trace;
    }

    private void AttackAreaOn()
    {
        attackMovingForward = true;

        // 공격 처리 On
        attackAreaScript.AttackAreaOn();
    }

    private void AttackAreaOff()
    {
        attackMovingForward = false;

        // 공격 처리 Off
        attackAreaScript.AttackAreaOff();
    }
}
