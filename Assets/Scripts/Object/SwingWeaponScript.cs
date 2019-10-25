using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwingWeaponScript : WeaponScript
{
    [Header("Swing Used")]
    public GameObject swingArea;
    private MeleeWeaponTrail meleeWeaponTrail;
    private SwingWeaponCollider swingWeaponCollider;

    [HideInInspector] public Vector3 attackVector;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();

        meleeWeaponTrail = swingArea.GetComponent<MeleeWeaponTrail>();

        swingWeaponCollider = swingArea.GetComponent<SwingWeaponCollider>();
        swingWeaponCollider.Init(this);
    }

    public override void PreAttack()
    {
        meleeWeaponTrail.Emit = true;
    }

    public override void Attack(bool isFocusMode, Vector3 attackVector, float attackMagnitude)
    {
        if (conUsing > 0)
        {
            // 무기 사용량 감소
            UsingWeapon();

            // Hit 시 넉백 방향을 위해 공격 방향을 저장
            this.attackVector = attackVector;

            // 공격 판정 코루틴 실행
            StartCoroutine(HitAreaOn());
        }
    }

    public override void PostAttack()
    {
        meleeWeaponTrail.Emit = false;
    }

    public override void DestroyWeapon()
    {
        meleeWeaponTrail.ActiveFalseTrailObject();
    }

    private IEnumerator HitAreaOn()
    {
        swingWeaponCollider.attackCollider.enabled = true;

        float conTime = 0;
        float maxTime = 0.05f;
        while(conTime < maxTime)
        {
            conTime += Time.deltaTime;
            yield return null;
        }

        swingWeaponCollider.attackCollider.enabled = false;
    }
}
