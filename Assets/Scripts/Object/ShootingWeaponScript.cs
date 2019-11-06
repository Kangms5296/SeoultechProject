using UnityEngine;

public class ShootingWeaponScript : WeaponScript
{
    [Header("Shooting Used")]
    public Transform muzzle;                        // 총구 위치
    private ParticleSystem muzzleFlash;             // 총구 이펙트
    public float shakeMagnitude;                    // 총기 반동


    public float distance;                          // 투사체 거리
    public string projectileName;                   // 투사체 종류
    public bool isProjectilePenetrating;            // 투사체 관통 여부
    public int maxContinuousShootingCount;          // 최대 투사체 연속 발사 횟수
    private int conContinuousShootingCount;         // 현제 투사체 연속 발사 횟수

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();

        muzzleFlash = muzzle.Find("Flash").GetComponent<ParticleSystem>();
    }

    public void InstanceProjectile(bool isContinuous, Vector3 attackVector)
    {
        if (conUsing > 0)
        {
            // 투사체를 생성
            GameObject temp = ObjectPullManager.Instance.GetInstanceByName(projectileName);

            // 발사
            if (temp != null)
            {
                if (isContinuous)
                    conContinuousShootingCount++;

                ProjectileScript projectile = temp.GetComponent<ProjectileScript>();
                projectile.Init(muzzle.position, attackVector, isProjectilePenetrating, damage, distance);
            }
            
            // 총구 이펙트
            muzzleFlash.Emit(1);
            
            // 총구 반동으로 인한 화면 덜림
            SystemManager.Instance.CameraShake("Shooting", 0.05f, 0.1f * shakeMagnitude, 0.08f * shakeMagnitude);
            
            // UI 남은 사용 횟수 수정
            UsingWeapon();
        }
    }

    public void ContinuousAttackEnd()
    {
        conContinuousShootingCount = 0;
    }

    public bool CanAttack()
    {
        if (conContinuousShootingCount >= maxContinuousShootingCount)
            return false;

        return true;
    }
}
