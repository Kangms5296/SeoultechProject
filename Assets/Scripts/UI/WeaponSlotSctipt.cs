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

    private string weaponName;  // 등록한 아이템 이름
    private int conUsing;       // 남은 사용 횟수
    private int maxUsing;       // 최대 사용 횟수

    public void InitSlot()
    {
        activatedColor = new Color(1, 1, 1, 80 / 255.0f);
        activatedSize = 0.8f;
        activatedWeaponImageAlpha = 200 / 255.0f;

        deactivatedColor = new Color(0, 0, 0, 120 / 255.0f);
        deactivatedSize = 0.6f;
        deactivatedWeaponImageAlpha = 120 / 255.0f;
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

    public void SetWeapon(string weaponName, int conUsing, int maxUsing)
    {
        // 새로운 무기를 slot에 추가.
        this.weaponName = weaponName;

        // slot 이미지를 변경
        emptyText.enabled = false;
        weaponImage.sprite = ResourcesObjectCaching.GetWeaponSprite(weaponName);
        weaponImage.enabled = true;

        // 남은 사용 횟수를 변경
        this.conUsing = conUsing;
        this.maxUsing = maxUsing;
        usingText.text = string.Format("{0:D2} / {1:D2}", conUsing, maxUsing);
    }

    public void ResetWeapon()
    {
        weaponName = "";

        emptyText.enabled = true;
        weaponImage.enabled = false;

        conUsing = 0;
        maxUsing = 0;
        usingText.text = string.Format("{0:D2} / {1:D2}", conUsing, maxUsing);
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
