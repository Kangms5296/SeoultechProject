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
    public Text conCount;

    private Color activatedColor;
    private float activatedSize;

    private Color deactivatedColor;
    private float deactivatedSize;

    private Coroutine ActivatingCoroutine = null;

    public void InitSlot()
    {
        activatedColor = new Color(1, 1, 1, 80 / 255.0f);
        activatedSize = 0.8f;

        deactivatedColor = new Color(0, 0, 0, 120 / 255.0f);
        deactivatedSize = 0.6f;
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

    private IEnumerator Activating()
    {
        float conSize = weaponSlot.localScale.x;
        float conColor = frame.color.r;
        
        float conTime = conSize / activatedSize;
        while (conTime < 1)
        {
            weaponSlot.localScale = new Vector3(conTime * activatedSize, conTime * activatedSize, 1);
            frame.color = new Color(conTime, conTime, conTime, conTime * activatedColor.a);
            background.color = new Color(conTime, conTime, conTime, conTime * activatedColor.a);

            conTime += Time.deltaTime * 1.2f;
            yield return null;
        }
        weaponSlot.localScale = new Vector3(activatedSize, activatedSize, 1);
        frame.color = activatedColor;
        background.color = activatedColor;

        ActivatingCoroutine = null;
    }

    private IEnumerator Deactivating()
    {
        float conSize = weaponSlot.localScale.x;
        float conColor = frame.color.r;

        float conTime = conSize / deactivatedSize;
        while ((1 - conTime) > 0)
        {
            weaponSlot.localScale = new Vector3((1 - conTime) * deactivatedSize, (1 - conTime) * deactivatedSize, 1);
            frame.color = new Color((1 - conTime), (1 - conTime), (1 - conTime), (1 - conTime) * deactivatedColor.a);
            background.color = new Color((1 - conTime), (1 - conTime), (1 - conTime), (1 - conTime) * deactivatedColor.a);

            conTime += Time.deltaTime * 1.2f;
            yield return null;
        }
        weaponSlot.localScale = new Vector3(deactivatedSize, deactivatedSize, 1);
        frame.color = deactivatedColor;
        background.color = deactivatedColor;

        ActivatingCoroutine = null;
    }
}
