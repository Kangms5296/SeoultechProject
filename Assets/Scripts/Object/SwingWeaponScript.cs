using UnityEngine;

public class SwingWeaponScript : WeaponScript
{
    [Header("Swing Used")]
    public MeleeWeaponTrail meleeWeaponTrail;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    public override void PreAttack()
    {
        meleeWeaponTrail.Emit = true;
    }

    public override void Attack(bool isFocusMode, Vector3 attackVector, float attackMagnitude)
    {
        if (conUsing > 0)
        {
            UsingWeapon();
        }
    }

    public override void PostAttack()
    {
        meleeWeaponTrail.Emit = false;
    }
}
