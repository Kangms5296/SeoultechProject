using UnityEngine;

public class SwingWeaponScript : WeaponScript
{
    [Header("Swing Used")]
    public GameObject swingArea;
    private MeleeWeaponTrail meleeWeaponTrail;

    public float hitAreaHorizontal;       // 공격 x축 범위
    public float hitAreaVertical;         // 공격 z축 범위


    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();

        meleeWeaponTrail = swingArea.GetComponent<MeleeWeaponTrail>();
    }

    public void TrailOn()
    {
        meleeWeaponTrail.Emit = true;
    }

    public void TrailOff()
    {
        meleeWeaponTrail.Emit = false;
    }

    public void DestroyWeapon()
    {
        meleeWeaponTrail.ActiveFalseTrailObject();
    }
}
