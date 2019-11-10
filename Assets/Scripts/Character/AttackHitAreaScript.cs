using System.Collections.Generic;
using UnityEngine;

public class AttackHitAreaScript : MonoBehaviour
{
    // 공격 범위 내 Monster 정보
    private List<MonsterScript> inHitAreaMonster = new List<MonsterScript>();

    // 공격 범위 내 Crate 정보
    private List<BeatableObjectScript> inHitAreaObject = new List<BeatableObjectScript>();

    public void CloseHitAreaOn(float horizontal, float vertical)
    {
        transform.localScale = new Vector3(horizontal, 1, vertical);
        gameObject.SetActive(true);
    }

    public void CloseHitAreaOff()
    {
        // 공격 범위 몬스터 정보를 초기화
        inHitAreaMonster = new List<MonsterScript>();
        inHitAreaObject = new List<BeatableObjectScript>();

        gameObject.SetActive(false);
    }

    public void CloseHit(bool isPunch, int damage, Vector3 knockBackDirection, float knockBackMagnitude)
    {
        // 공격 범위의 모든 Monster들을 공격
        foreach (MonsterScript monster in inHitAreaMonster)
        {
            // 사망한 몬스터는 공격x
            if (monster.isDie)
                continue;

            // 데미지 처리
            monster.TakeDamage(damage, knockBackDirection, knockBackMagnitude);

            if (isPunch)
            {
                // 타격 효과
                SystemManager.Instance.HitEffect(0.05f, 0.5f);

                // 타격 이펙트
                GameObject effect = ObjectPullManager.Instance.GetInstanceByName("PunchHit");
                Vector3 effectPos = monster.transform.position + new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(0, 0.2f));
                effect.transform.position = effectPos;
                effect.SetActive(true);
            }
            else
            {
                // 타격 효과
                SystemManager.Instance.HitEffect(0.1f, 0.8f);
                SystemManager.Instance.SlowMode(0.2f, 0.4f);

                // 타격 이펙트
                GameObject effect = ObjectPullManager.Instance.GetInstanceByName("Hit");
                Vector3 effectPos = monster.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                effect.transform.position = effectPos;
                effect.SetActive(true);
            }
        }

        // 공격 범위의 모든 공격 가능 물건들을 공격
        foreach(BeatableObjectScript temp in inHitAreaObject)
        {
            // 부서진 오브젝트는 공격x
            if (temp.canHit)
            {
                temp.Hit();

                if (isPunch)
                {
                    // 타격 효과
                    SystemManager.Instance.HitEffect(0.05f, 0.3f);

                    // 타격 이펙트
                    GameObject effect = ObjectPullManager.Instance.GetInstanceByName("PunchHit");
                    Vector3 effectPos = transform.position + knockBackDirection * 0.5f;
                    effect.transform.position = effectPos;
                    effect.SetActive(true);
                }
                else
                {
                    // 타격 효과
                    SystemManager.Instance.HitEffect(0.1f, 0.8f);
                    SystemManager.Instance.SlowMode(0.2f, 0.4f);

                    // 타격 이펙트
                    GameObject effect = ObjectPullManager.Instance.GetInstanceByName("Hit");
                    Vector3 effectPos = transform.position + knockBackDirection * 0.8f;
                    effect.transform.position = effectPos;
                    effect.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster"))
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
    }
}
