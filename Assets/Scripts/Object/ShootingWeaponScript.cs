using UnityEngine;

public class ShootingWeaponScript : WeaponScript
{
    [Header("Shooting Used")]
    public float recoil;                            // 총기 반동
    public Transform muzzle;                        // 총구 위치
    public float distance;                          // 투사체 거리
    public string projectileName;                   // 투사체 종류
    private ParticleSystem muzzleFlash;             // 총구 이펙트

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();

        muzzleFlash = muzzle.Find("Flash").GetComponent<ParticleSystem>();
    }

    public override void PreAttack()
    {

    }

    public override void Attack(bool isFocusMode, Vector3 attackVector, float attackMagnitude)
    {
        if (conUsing > 0)
        {
            // 투사체를 생성
            GameObject temp = ObjectPullManager.GetInstanceByName(projectileName);

            // 지정 발사
            if (temp != null)
            {
                float resultRecoil;
                float maxRecoil;

                // 반동 계산(조준 시 반동이 작아진다.)
                if (isFocusMode)
                {
                    resultRecoil = Random.Range(0, recoil * 0.3f);
                    maxRecoil = recoil * 0.3f;
                }
                else
                {
                    resultRecoil = Random.Range(0, recoil);
                    maxRecoil = recoil;
                }

                ProjectileScript projectile = temp.GetComponent<ProjectileScript>();
                projectile.Init(muzzle.position, attackVector + transform.right * resultRecoil + -transform.right * (maxRecoil - resultRecoil), damage, distance);
            }
            
            // 총구 이펙트
            muzzleFlash.Emit(1);
            
            UsingWeapon();
        }
    }

    public override void PostAttack()
    {

    }

    public override void DestroyWeapon()
    {

    }
}
