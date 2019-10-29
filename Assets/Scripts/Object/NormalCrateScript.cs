using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCrateScript : MonoBehaviour
{
    public Transform weaponSpawnPos;

    public WeaponCrateInstantiateObject[] weaponList;       // Crate에서 나올 아이템 종류

    private Transform dropedWeaponParent;                   // 생성한 아이템이 들어갈 Parent Transform

    private void Start()
    {
        int weaponListProbabilitySum = 0;
        foreach (WeaponCrateInstantiateObject weapon in weaponList)
            weaponListProbabilitySum += weapon.probability;

        int conProbabilitySum = 0;
        int probability = Random.Range(0, weaponListProbabilitySum) + 1;
        foreach (WeaponCrateInstantiateObject weapon in weaponList)
        {
            conProbabilitySum += weapon.probability;
            if (conProbabilitySum >= probability)
            {
                if (weapon.weapon == null)
                    continue;

                WeaponScript newWeapon = Instantiate(weapon.weapon, dropedWeaponParent);
                newWeapon.transform.position = weaponSpawnPos.position;
                newWeapon.transform.rotation = weaponSpawnPos.rotation;

                break;
            }
        }
    }
}
