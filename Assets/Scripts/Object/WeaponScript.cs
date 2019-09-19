using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        conUsing = maxUsing;

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
}
