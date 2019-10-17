using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotSctipt : MonoBehaviour
{
    [Header("Component")]
    public RectTransform weaponSlot;
    public Image frame;
    public Image background;
    public Image weaponImage;
    public Text emptyText;
    public Text usingText;

    // slot 배경이 활성화될 때의 색과 크기
    private Color activatedColor;
    private float activatedSize;
    private float activatedWeaponImageAlpha;

    // slot 배경이 비활성화될 때의 색과 크기
    private Color deactivatedColor;
    private float deactivatedSize;
    private float deactivatedWeaponImageAlpha;

    private Coroutine ActivatingCoroutine = null;
    
    private WeaponScript conWeapon;

    public void InitSlot()
    {
        activatedColor = new Color(1, 1, 1, 150 / 255.0f);
        activatedSize = 0.8f;
        activatedWeaponImageAlpha = 220 / 255.0f;

        deactivatedColor = new Color(0, 0, 0, 140 / 255.0f);
        deactivatedSize = 0.6f;
        deactivatedWeaponImageAlpha = 140 / 255.0f;
    }

    public void ActivateSlot()
    {
        if (ActivatingCoroutine != null)
            StopCoroutine(ActivatingCoroutine);

        ActivatingCoroutine = StartCoroutine(Activating());
    }

    public void DeactivateSlot()
    {
        if (ActivatingCoroutine != null)
            StopCoroutine(ActivatingCoroutine);

        ActivatingCoroutine = StartCoroutine(Deactivating());
    }

    public void SetWeapon(WeaponScript newWeapon)
    {
        // 새 무기를 등록
        conWeapon = newWeapon;

        // 등록한 무기로 slot 이미지를 변경
        emptyText.enabled = false;
        weaponImage.sprite = ResourcesObjectCaching.GetWeaponSprite(conWeapon.weaponName);
        weaponImage.enabled = true;

        // 등록한 무기의 남은 사용 횟수를 변경
        usingText.enabled = true;
        usingText.text = string.Format("{0:D2} / {1:D2}", conWeapon.conUsing, conWeapon.maxUsing);

        conWeapon.usingWeaponEvent -= ChangeUsing;
        conWeapon.usingWeaponEvent += ChangeUsing;
    }

    public void ChangeUsing()
    {
        Debug.Log(11133);
        usingText.text = string.Format("{0:D2} / {1:D2}", conWeapon.conUsing, conWeapon.maxUsing);
    }

    public void ResetWeapon()
    {
        conWeapon = null;

        emptyText.enabled = true;
        weaponImage.enabled = false;
        usingText.enabled = false;
    }

    public WeaponScript GetWeapon()
    {
        return conWeapon;
    }

    private IEnumerator Activating()
    {
        float conTime = (weaponSlot.localScale.x - deactivatedSize) / (activatedSize - deactivatedSize);
        while (conTime < 1)
        {
            weaponSlot.localScale = new Vector3((1 - conTime) * deactivatedSize + (conTime * activatedSize), (1 - conTime) * deactivatedSize + (conTime * activatedSize), 1);
            frame.color = new Color(conTime, conTime, conTime, (1 - conTime) * deactivatedColor.a + (conTime * activatedColor.a));
            background.color = new Color(conTime, conTime, conTime, (1 - conTime) * deactivatedColor.a + (conTime * activatedColor.a));
            weaponImage.color = new Color(0, 0, 0, (1 - conTime) * deactivatedWeaponImageAlpha + (conTime * activatedWeaponImageAlpha));
            emptyText.color = new Color(0, 0, 0, (1 - conTime) * deactivatedWeaponImageAlpha + (conTime * activatedWeaponImageAlpha));

            conTime += Time.deltaTime * 3f;
            yield return null;
        }
        weaponSlot.localScale = new Vector3(activatedSize, activatedSize, 1);
        frame.color = activatedColor;
        background.color = activatedColor;
        weaponImage.color = new Color(0, 0, 0, activatedWeaponImageAlpha);
        emptyText.color = new Color(0, 0, 0, activatedWeaponImageAlpha);

        ActivatingCoroutine = null;
    }

    private IEnumerator Deactivating()
    {
        float conTime = (weaponSlot.localScale.x - deactivatedSize) / (activatedSize - deactivatedSize);
        while (conTime > 0)
        {
            weaponSlot.localScale = new Vector3((1 - conTime) * deactivatedSize + (conTime * activatedSize), (1 - conTime) * deactivatedSize + (conTime * activatedSize), 1);
            frame.color = new Color(conTime, conTime, conTime, (1 - conTime) * deactivatedColor.a + (conTime * activatedColor.a));
            background.color = new Color(conTime, conTime, conTime, (1 - conTime) * deactivatedColor.a + (conTime * activatedColor.a));
            weaponImage.color = new Color(0, 0, 0, (1 - conTime) * deactivatedWeaponImageAlpha + (conTime * activatedWeaponImageAlpha));
            emptyText.color = new Color(0, 0, 0, (1 - conTime) * deactivatedWeaponImageAlpha + (conTime * activatedWeaponImageAlpha));

            conTime -= Time.deltaTime * 3f;
            yield return null;
        }
        weaponSlot.localScale = new Vector3(deactivatedSize, deactivatedSize, 1);
        frame.color = deactivatedColor;
        background.color = deactivatedColor;
        weaponImage.color = new Color(0, 0, 0, deactivatedWeaponImageAlpha);
        emptyText.color = new Color(0, 0, 0, deactivatedWeaponImageAlpha);

        ActivatingCoroutine = null;
    }
    
}
