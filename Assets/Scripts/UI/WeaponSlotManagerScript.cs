using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManagerScript : MonoBehaviour
{
    private List<WeaponSlotSctipt> weaponSlots;
    private int conIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // 모든 slot들을 가져온다.
        weaponSlots = new List<WeaponSlotSctipt>();
        for (int i = 0; i < transform.childCount; i++)
        {
            weaponSlots.Add(transform.GetChild(i).GetComponent<WeaponSlotSctipt>());
            weaponSlots[i].InitSlot();
        }

        weaponSlots[0].ActivateSlot();
        
    }

    public void ClickSlot(int index)
    {
        if(conIndex != index)
            weaponSlots[conIndex].DeactivateSlot();

        weaponSlots[index].ActivateSlot();
        conIndex = index;
    }

    public void SetWeapon(string weaponName, int conUsing, int maxUsing)
    {
        weaponSlots[conIndex].SetWeapon(weaponName, conUsing, maxUsing);
    }

    public void ResetWeapon()
    {
        weaponSlots[conIndex].ResetWeapon();
    }
}
