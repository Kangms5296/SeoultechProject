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
    public PlayerScript owner;

    [Header("Weapon Info")]
    // 무기 이름
    public string weaponName;
    // 무기 종류
    public Weapontype weaponType;
    // 무기 남은 사용량
    public int conUsing;
    // 무기 최대 사용량
    public int maxUsing;
    // 무기 공격력
    public int damage;
    // 무기가 배치된 slot
    [HideInInspector] public int slotIndex;
    // 현재 아이템을 사용할 때 발생되는 이벤트
    [HideInInspector] public delegate void UsingWeaponEvent();
    [HideInInspector] public event UsingWeaponEvent usingWeaponEvent;
    // 무기 사용간 효과음
    public AudioClip weaponUsingSound;

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

    public void SetOwner(PlayerScript player)
    {
        owner = player;
    }

    public void ResetOwner()
    {
        owner = null;
    }
}
