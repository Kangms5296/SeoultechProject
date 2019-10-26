using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingWeaponScript : WeaponScript
{
    [Header("Throwing Used")]
    public ThrowObejctScript throwingScript;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    public override void PreAttack()
    {
    }

    public override void Attack(bool isFocusMode, Vector3 attackVector, float attackMagnitude)
    {
        if (conUsing > 0)
        {
            throwingScript.Init(attackVector, attackMagnitude);

            UsingWeapon();
        }
    }

    public override void PostAttack()
    {
    }

    public override void DestroyWeapon()
    {

    }

    public override bool CanAttack()
    {
        return true;
    }
}
