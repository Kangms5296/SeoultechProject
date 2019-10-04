using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public PlayerScript player;

    public WeaponSlotManagerScript weaponSlotManager;

    // Update is called once per frame
    void Update()
    {
        // 스페이스바 클릭(점프)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.Jump();
        }

        // Shift 키 버튼을 누르고 있으면 달리기 시작
        if (Input.GetKey(KeyCode.LeftShift))
        {
            player.Run();
        }
        // Shift 키 버튼을 때고 있으면 걷기 시작
        else
        {
            player.Walk();
        }

        // 마우스 왼쪽 키 클릭(공격)
        if (Input.GetMouseButtonDown(0))
        {
            player.Attack();
        }

        // 마우스 오른쪽 키 누르면(조건부 기술)
        if (Input.GetMouseButtonDown(1))
        {
            // 집중모드
            player.FocusMode();
        }

        // F 버튼 클릭(무기 줍기)
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 플레이어의 무기를 바닥의 무기와 교체
            bool isOk = player.PickUp();

            // weapon Slot 정보를 수정
            if (isOk)
                weaponSlotManager.SetWeapon(DropObjectScript.dropObject);
        }

        // X 버튼 클릭(무기 버리기)
        if (Input.GetKeyDown(KeyCode.X))
        {
            // 플레이어의 무기를 바닥에 버린다.
            bool isOk = player.Disarm();

            // weapon Slot 정보를 수정
            if (isOk)
                weaponSlotManager.ResetWeapon();
        }

        // 1 버튼 클릭(무기 변화)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 플레이어의 무기를 교체한다.
            bool isOk = player.WeaponChange(0);

            if (isOk)
            {
                // 클릭한 slot을 활성화
                weaponSlotManager.ClickSlot(0);


            }
        }

        // 2 버튼 클릭(무기 변화)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // 플레이어의 무기를 교체한다
            bool isOk = player.WeaponChange(1);

            if (isOk)
            {
                // 클릭한 slot을 활성화
                weaponSlotManager.ClickSlot(1);


            }
        }

        // 3 버튼 클릭(무기 변화)
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // 플레이어의 무기를 교체한다
            bool isOk = player.WeaponChange(2);

            if (isOk)
            {
                // 클릭한 slot을 활성화
                weaponSlotManager.ClickSlot(2);


            }
        }

        // 4 버튼 클릭(무기 변화)
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // 플레이어의 무기를 교체한다
            bool isOk = player.WeaponChange(3);

            if (isOk)
            {
                // 클릭한 slot을 활성화
                weaponSlotManager.ClickSlot(3);


            }
        }

        // Q 버튼 클릭(좌로 굴러)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            player.RollLeft();
        }

        // E 버튼 클릭(우로 굴러)
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.RollRight();
        }
    }
}
