using UnityEngine;

public class ShootingWeaponScript : WeaponScript
{
    [Header("Shooting Used")]
    public Transform muzzle;                        // 총구 위치
    private ParticleSystem muzzleFlash;             // 총구 이펙트

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

    public override void PreAttack()
    {

    }

    public override void Attack(bool isFocusMode, Vector3 attackVector, float attackMagnitude)
    {
        if (conUsing > 0)
        {
            // 투사체를 생성
            GameObject temp = ObjectPullManager.GetInstanceByName(projectileName);

            // 발사
            if (temp != null)
            {
                if (isFocusMode)
                    conContinuousShootingCount++;

                ProjectileScript projectile = temp.GetComponent<ProjectileScript>();
                projectile.Init(muzzle.position, attackVector, isProjectilePenetrating, damage, distance);
            }
            
            // 총구 이펙트
            muzzleFlash.Emit(1);
            
            // UI 남은 사용 횟수 수정
            UsingWeapon();
        }
    }

    public override void PostAttack()
    {
        conContinuousShootingCount = 0;
    }

    // 최대 연속 사격 횟수로 무기 사용 가능을 체크한다.
    // 이를 통해 단발 사격과 점사 모드를 가능하게 할 수 있다.
    public override bool CanAttack()
    {
        if (conContinuousShootingCount >= maxContinuousShootingCount)
            return false;

        return true;
    }

    // Shooting 무기는 무기가 파괴되지 않는다.
    // 총알이 다 떨어지면 사용 모션만을 취한다.
    public override void DestroyWeapon()
    {

    }


}
