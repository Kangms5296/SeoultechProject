using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapontype { None, Swing, Shooting, Throwing };

public abstract class WeaponScript : MonoBehaviour
{
    [Header("Weapon Component")]
    public GameObject drop;
    public GameObject equiped;
    public GameObject back;
    public GameObject belly;
    protected GameObject conWeaponComponent;

    [Header("User Info")]
    public PlayerScript player;

    [Header("Weapon Info")]
    // 아이템의 이름
    public string weaponName;
    // 아이템의 종류
    public Weapontype weaponType;
    // 아이템의 남은 사용량
    public int conUsing;
    // 아이템의 최대 사용량
    public int maxUsing;
    // 아이템의 공격력
    public int damage;
    // 현재 아이템을 사용할 때 발생되는 이벤트
    public delegate void UsingWeaponEvent();
    public event UsingWeaponEvent usingWeaponEvent;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        // 초기 시작 시 사용 횟수를 최대로
        conUsing = maxUsing;

        // 초기 시작 시 Drop된 상태
        conWeaponComponent = drop;

        // 무기 사용 이벤트 생성 및 등록
        usingWeaponEvent = delegate { };
    }

    public void ChangeToDrop()
    {
        conWeaponComponent.SetActive(false);

        drop.SetActive(true);
        conWeaponComponent = drop;
    }

    public void ChangeToEquiped()
    {
        conWeaponComponent.SetActive(false);

        equiped.SetActive(true);
        conWeaponComponent = equiped;
    }

    public void ChangeToBack()
    {
        conWeaponComponent.SetActive(false);

        back.SetActive(true);
        conWeaponComponent = back;
    }

    public void ChangeToBelly()
    {
        conWeaponComponent.SetActive(false);

        belly.SetActive(true);
        conWeaponComponent = belly;
    }

    public void UsingWeapon()
    {
        conUsing--;
        
        usingWeaponEvent();
    }

    public void UseWeapon(PlayerScript player)
    {
        this.player = player;
    }

    public void Throwweapon()
    {
        player = null;
    }


    public abstract void PreAttack();
    public abstract void Attack(bool isFocusMode, Vector3 attackVector, float attackMagnitude);
    public abstract void PostAttack();
   

    public abstract void DestroyWeapon();


}
