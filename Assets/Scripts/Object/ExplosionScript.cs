using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    // 폭발 공격력
    public int damage;

    // 폭발 판정 콜라이더
    public Collider col;

    // 공격 범위 내 Monster 정보
    private List<MonsterScript> inHitAreaMonster;
    // 공격 범위 내 Crate 정보
    private List<BeatableObjectScript> inHitAreaObject;
    // 공격 범위 내 Player 정보
    private List<PlayerScript> inHitAreaPlayer;

    private void OnEnable()
    {
        inHitAreaMonster = new List<MonsterScript>();
        inHitAreaObject = new List<BeatableObjectScript>();
        inHitAreaPlayer = new List<PlayerScript>();

        StartCoroutine(Damaging());
    }
    
    IEnumerator Damaging()
    {
        // 폭발 효과
        SystemManager.Instance.HitEffect(0.1f, 1.2f);
        SystemManager.Instance.CameraShake("Explosion", 0.1f, 200, 10);
        
        col.enabled = true;

        // 데미지 처리 이전 잠시 대기(콜라이더 인식이 될 수 있는 최소한의 시간 대기)
        float conTime = 0;
        while(conTime < 0.05f)
        {
            conTime += Time.deltaTime;
            yield return null;
        }

        foreach(MonsterScript monster in inHitAreaMonster)
        {
            // 대기하는 동안 사망한 몬스터는 처리x
            if (monster.isDie)
                continue;

            // 데미지 처리
            monster.TakeDamage(damage, new Vector3(monster.transform.position.x - transform.position.x, 0, monster.transform.position.z - transform.position.z).normalized, 5f);

            yield return null;
        }

        foreach (BeatableObjectScript temp in inHitAreaObject)
        {
            // 상자가 부서질 때 까지 공격
            while (true)
            {
                if (!temp.canHit)
                    break;

                temp.Hit();
                yield return null;
            }
        }

        foreach (PlayerScript player in inHitAreaPlayer)
        {
            // 데미지 처리
            player.TakeDamage(damage, new Vector3(player.transform.position.x - transform.position.x, 0, player.transform.position.z - transform.position.z).normalized, 5f);

            yield return null;
        }


        col.enabled = false;

        // 파티클 종료까지 대기
        StartCoroutine(CheckIfAlive());
    }

    IEnumerator CheckIfAlive()
    {
        float conTime;
        while (true)
        {
            conTime = 0;
            while (conTime < 0.5f)
            {
                conTime += Time.deltaTime;
                yield return null;
            }

            if (!GetComponent<ParticleSystem>().IsAlive(true))
                gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterScript monster = other.GetComponent<MonsterScript>();

            // 기존에 영역에 들어있는 몬스터가 다시 처리되면 무시
            if (inHitAreaMonster.Contains(monster))
                return;

            inHitAreaMonster.Add(monster);
        }
        else if (other.CompareTag("Beatable Object"))
        {
            BeatableObjectScript temp = other.GetComponent<BeatableObjectScript>();

            // 기존에 공격 영역에 들어있는 물건이 다시 처리되면 무시
            if (inHitAreaObject.Contains(temp))
                return;

            inHitAreaObject.Add(temp);
        }
        else if (other.CompareTag("Player"))
        {
            PlayerScript player = other.GetComponent<PlayerScript>();

            // 기존에 영역에 들어있던 플레이어면 무시
            if (inHitAreaPlayer.Contains(player))
                return;

            inHitAreaPlayer.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterScript monster = other.GetComponent<MonsterScript>();

            // 기존에 영역에 들어있는 몬스터가 다시 처리되면 무시
            if (!inHitAreaMonster.Contains(monster))
                return;

            inHitAreaMonster.Remove(monster);
        }
        else if (other.CompareTag("Beatable Object"))
        {
            BeatableObjectScript temp = other.GetComponent<BeatableObjectScript>();

            // 기존에 공격 영역에 들어있는 물건이 다시 처리되면 무시
            if (!inHitAreaObject.Contains(temp))
                return;

            inHitAreaObject.Remove(temp);
        }
        else if(other.CompareTag("Player"))
        {
            PlayerScript player = other.GetComponent<PlayerScript>();

            // 영역에 없던 플레어어면 무시
            if (!inHitAreaPlayer.Contains(player))
                return;

            inHitAreaPlayer.Remove(player);
        }
    }
}
