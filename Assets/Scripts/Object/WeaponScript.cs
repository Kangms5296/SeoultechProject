using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapontype { None, Swing, Shooting, Throwing };

public class WeaponScript : MonoBehaviour
{
    [Header("Weapon Component")]
    public GameObject drop;
    public GameObject equiped;

    [Header("Weapon Info")]
    // 현재 이 아이템의 이름
    public string weaponName;
    // 현재 이 아이템의 종류
    public Weapontype weaponType;
    // 현재 이 아이템의 남은 사용량
    public int conUsing;
    // 현재 이 아이템의 최대 사용량
    public int maxUsing;
    // 현재 이 아이템의 공격력
    public int damage;

    [Header("In Use Swing")]
    public bool useWeaponTrail;
    public MeleeWeaponTrail meleeWeaponTrail;

    [Header("In Use Shooting")]
    public float recoil;                            // 총기 반동
    public float distance;                          // 투사체 거리
    public bool useMuzzle;
    public Transform muzzle;
    public string projectileName;
    private ParticleSystem muzzleFlash;

    // Start is called before the first frame update
    void Start()
    {
        conUsing = maxUsing;

        if(useMuzzle)
            muzzleFlash = muzzle.Find("Flash").GetComponent<ParticleSystem>();

    }

    public void ChangeToDrop()
    {
        equiped.SetActive(false);
        drop.SetActive(true);
    }

    public void ChangeToEquiped()
    {
        drop.SetActive(false);
        equiped.SetActive(true);
    }

    public void PreAttack()
    {
        if (useWeaponTrail)
            meleeWeaponTrail.Emit = true;
    }

    public void Attack(Vector3 attackVector)
    {
        if(useMuzzle)
        {
            // 투사체를 생성하여 발사
            GameObject temp = ObjectPullManager.GetInstanceByName(projectileName);
            if (temp != null)
            {
                ProjectileScript projectile = temp.GetComponent<ProjectileScript>();
                projectile.Init(muzzle.position, attackVector, damage, distance);
            }

            // 총구 이펙트
            muzzleFlash.Emit(1);
        }

    }

    public void PostAttack()
    {
        if (useWeaponTrail)
            meleeWeaponTrail.Emit = false;
    }
}
