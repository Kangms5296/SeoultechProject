using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObjectScript : MonoBehaviour
{
    // 플레이어 주변에 무기가 있는가?
    public static bool isThereWeaponAroundPlayer = false;
    // 플레이어 주변에 있는 무기 정보
    public static WeaponScript dropObject;

    // Press F
    [HideInInspector] public GameObject pressF;

    private WeaponScript parentWeaponScript;

    private void Start()
    {
        parentWeaponScript = transform.parent.GetComponent<WeaponScript>();
    }

    private void OnDisable()
    {
        if (pressF != null)
            pressF.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Vector3 position = transform.position;

            // 'Press F' 가 표시된 Text를 Drop된 오브젝트의 위치로 이동
            pressF = ObjectPullManager.Instance.GetInstanceByName("Press F");
            pressF.transform.position = new Vector3(position.x, position.y + 0.8f, position.z);
            pressF.SetActive(true);

            // 현재 주위에 있는 오브젝트의 정보를 임시 저장
            dropObject = parentWeaponScript;
            isThereWeaponAroundPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 'Press F' 가 표시된 Text 삭제
            pressF.SetActive(false);
            
            isThereWeaponAroundPlayer = false;
            //dropObject = null;
        }
    }
}
