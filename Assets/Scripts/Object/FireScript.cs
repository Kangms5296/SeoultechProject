using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    // 데미지
    public int damage;

    // 공격 주기
    public float damageDelay;

    // 불이 활성화되는 시간
    public float liveTime;
    
    // 공격을 줄 수 있는가?
    private bool isLive;

    public Light fireLight;
    private float lightSize;

    // 공격 범위 내 Monster 정보
    private List<MonsterScript> inHitAreaMonster;


    private void OnEnable()
    {
        inHitAreaMonster = new List<MonsterScript>();

        StartCoroutine(Burning());
        StartCoroutine(Damaging());
    }


    IEnumerator Damaging()
    {
        float conTime;

        isLive = true;
        while (isLive)
        {
            conTime = 0;

            while (conTime < damageDelay)
            {
                conTime += Time.deltaTime;
                yield return null;
            }

            if (inHitAreaMonster.Count != 0)
            {
                foreach (MonsterScript monster in inHitAreaMonster)
                {
                    // 대기하는 동안 사망한 몬스터는 처리x
                    if (monster.isDie)
                        continue;

                    // 데미지 처리
                    monster.TakeDamage(damage, new Vector3(monster.transform.position.x - transform.position.x, 0, monster.transform.position.z - transform.position.z), 0.1f);

                    yield return null;
                }
            }
        }
    }

    IEnumerator Burning()
    {
        ParticleSystem particle = GetComponent<ParticleSystem>();

        StartCoroutine(FireLightOn(0.2f, 6));

        float conTime = 0;
        while(conTime < liveTime)
        {
            conTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(FireLightOff(0.2f, 6));

        isLive = false;
        particle.Stop();

    }

    IEnumerator FireLightOn(float time, float size)
    {
        float temp;

        float conTime = time * (lightSize / size);
        while (conTime < time)
        {
            temp = size * (conTime / time);

            fireLight.range = temp;
            lightSize = temp;
            
            conTime += Time.deltaTime;
            yield return null;
        }

        fireLight.range = size;
        lightSize = size;
    }

    IEnumerator FireLightOff(float time, float size)
    {
        float temp;

        float conTime = time * (lightSize / size);
        while (conTime > 0)
        {
            temp = size * (conTime / time);

            fireLight.range = temp;
            lightSize = temp;
            
            conTime -= Time.deltaTime;
            yield return null;
        }

        fireLight.range = 0;
        lightSize = 0;

        // 파티클 종료까지 대기
        StartCoroutine(WaitForEnd());
    }

    IEnumerator WaitForEnd()
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
        else if (other.CompareTag("Player"))
        {

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
        else if (other.CompareTag("Player"))
        {

        }
    }

}
