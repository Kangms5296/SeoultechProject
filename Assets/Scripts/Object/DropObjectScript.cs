using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObjectScript : MonoBehaviour
{
    // 플레이어 주변에 무기가 있는가?
    public static bool isThereWeaponAroundPlayer = false;
    // 플레이어 주변에 있는 무기 정보
    public static WeaponScript dropObject;


    private WeaponScript parentWeaponScript;

    private void Start()
    {
        parentWeaponScript = transform.parent.GetComponent<WeaponScript>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // 'Press F' 가 표시된 Text를 Drop된 오브젝트의 위치로 이동
            Vector3 position = transform.position;
            WorldSpaceCanvasUIs.SetPosition("Press F", new Vector3(position.x, position.y + 0.8f, position.z));

            // 'Press F' 가 표시된 Text를 화면에 표시
            WorldSpaceCanvasUIs.SetActive("Press F", true);

            // 현재 주위에 있는 오브젝트의 정보를 임시 저장
            dropObject = parentWeaponScript;
            isThereWeaponAroundPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            WorldSpaceCanvasUIs.SetActive("Press F", false);
            
            isThereWeaponAroundPlayer = false;
            //dropObject = null;
        }
    }
}
