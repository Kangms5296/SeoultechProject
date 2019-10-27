using System.Collections.Generic;
using UnityEngine;

public class AttackHitAreaScript : MonoBehaviour
{
    public List<MonsterScript> inHitAreaMonster = new List<MonsterScript>();

    public void CloseHitAreaOn(float horizontal, float vertical)
    {
        transform.localScale = new Vector3(horizontal, 1, vertical);
        gameObject.SetActive(true);
    }

    public void CloseHitAreaOff()
    {
        // 공격 범위 몬스터 정보를 초기화
        foreach(MonsterScript monster in inHitAreaMonster)
            monster.isHitTarget = false;
        inHitAreaMonster = new List<MonsterScript>();

        gameObject.SetActive(false);
    }

    public void CloseHit(bool isPunch, int damage, Vector3 knockBackDirection, float knockBackMagnitude)
    {
        foreach (MonsterScript monster in inHitAreaMonster)
            if (monster.isHitTarget)
            {
                // 데미지 처리
                monster.TakeDamage(damage, knockBackDirection, knockBackMagnitude);

                if (isPunch)
                {
                    // 타격 효과
                    SystemManager.Instance.HitEffect(false, 0.5f);

                    // 타격 이펙트
                    GameObject effect = ObjectPullManager.GetInstanceByName("PunchHit");
                    Vector3 effectPos = monster.transform.position + new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));
                    effect.transform.position = effectPos;
                    effect.SetActive(true);
                }
                else
                {
                    // 타격 효과
                    SystemManager.Instance.HitEffect(true, 1);

                    // 타격 이펙트
                    GameObject effect = ObjectPullManager.GetInstanceByName("Hit");
                    Vector3 effectPos = monster.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                    effect.transform.position = effectPos;
                    effect.SetActive(true);
                }

            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster"))
        {
            MonsterScript monster = other.GetComponent<MonsterScript>();

            // 기존에 영역에 들어있는 몬스터가 다시 처리되면 무시
            if (monster.isHitTarget)
                return;
            monster.isHitTarget = true;

            inHitAreaMonster.Add(monster);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterScript monster = other.GetComponent<MonsterScript>();

            monster.isHitTarget = false;
        }
    }
}
