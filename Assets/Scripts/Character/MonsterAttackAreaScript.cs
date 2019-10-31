using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackAreaScript : MonoBehaviour
{
    MonsterScript monsterScript;
    Collider attackAreaCollider;

    // 현재 공격으로 데미지 처리를 받은 플레이어 정보를 일시적으로 저장(콜라이더의 중복 충돌 처리 방지)
    private List<PlayerScript> inHitAreaPlayer;

    private void Start()
    {
        monsterScript = transform.parent.GetComponent<MonsterScript>();
        attackAreaCollider = GetComponent<Collider>();
    }

    public void AttackAreaOn()
    {
        inHitAreaPlayer = new List<PlayerScript>();

        attackAreaCollider.enabled = true;
    }

    public void AttackAreaOff()
    {
        attackAreaCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (monsterScript.isDie)
                return;

            PlayerScript player = other.GetComponent<PlayerScript>();

            // 이번 공격으로 이미 데미지 처리를 받은 경우(콜라이더의 충복 충돌 처리된 경우)는 무시
            if (inHitAreaPlayer.Contains(player))
                return;
            inHitAreaPlayer.Add(player);

            player.TakeDamage(monsterScript.damage, transform.forward, 7f);
        }
    }
}
