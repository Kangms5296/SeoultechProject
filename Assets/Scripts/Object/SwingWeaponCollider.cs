using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingWeaponCollider : MonoBehaviour
{
    public Transform effectInstancePos;


    private SwingWeaponScript weaponInfo;

    [HideInInspector]public Collider attackCollider;

    public void Init(SwingWeaponScript weaponInfo)
    {
        this.weaponInfo = weaponInfo;

        attackCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster"))
        {
            // 몬스터 데미지 처리
            MonsterScript monster = other.GetComponent<MonsterScript>();
            monster.TakeDamage(weaponInfo.damage, weaponInfo.attackVector, 0.5f);

            // 타격 효과
            SystemManager.Instance.HitEffect(0.6f);

            // 타격 이펙트
            GameObject effect = ObjectPullManager.GetInstanceByName("Hit");
            effect.transform.position = effectInstancePos.position;
            effect.SetActive(true);
        }
    }
}
